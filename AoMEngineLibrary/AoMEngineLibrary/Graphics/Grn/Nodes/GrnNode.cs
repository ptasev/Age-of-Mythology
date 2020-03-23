namespace AoMEngineLibrary.Graphics.Grn.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GrnNode
    {
        public GrnNode? ParentNode
        {
            get;
            set;
        }
        public GrnNode? PreviousSibling
        {
            get;
            internal set;
        }
        public GrnNode? NextSibling
        {
            get;
            internal set;
        }
        public GrnNode? FirstChild
        {
            get
            {
                if (this.ChildNodes.Count > 0)
                {
                    return this.ChildNodes[0];
                }

                return null;
            }
        }

        public GrnNodeType NodeType
        {
            get;
            protected set;
        }
        public UInt32 Offset
        {
            get;
            set;
        }

        public Int32 NumTotalChildNodes
        {
            get;
            set;
        }
        public List<GrnNode> ChildNodes
        {
            get;
            set;
        }

        public byte[] Data
        {
            get;
            set;
        }

        public GrnNode(GrnNode? parentNode, GrnNodeType nodeType)
        {
            this.ParentNode = parentNode;
            this.PreviousSibling = null;
            this.NextSibling = null;
            this.NodeType = nodeType;
            this.Offset = 0;
            this.ChildNodes = new List<GrnNode>();
            this.Data = new byte[0];
        }

        public static GrnNode ReadByNodeType(GrnBinaryReader reader, GrnNode? parentNode, GrnNodeType nodeType)
        {
            GrnNode node;
            if (nodeType == GrnNodeType.FileDirectory)
            {
                node = new GrnMainNode();
            }
            else if (parentNode == null)
            {
                throw new InvalidOperationException($"Cannot create a node of type {nodeType} without a parent.");
            }
            else if (nodeType == GrnNodeType.VersionFrameDirectory ||
                nodeType == GrnNodeType.StandardFrameDirectory ||
                nodeType == GrnNodeType.NullFrameDirectory)
            {
                node = new GrnSectionNode(parentNode, nodeType);
            }
            else if (nodeType == GrnNodeType.StringTable)
            {
                node = new GrnStringTableNode(parentNode);
            }
            else if (parentNode.NodeType == GrnNodeType.MeshVertexSet)
            {
                if (nodeType == GrnNodeType.MeshVertices)
                {
                    node = new GrnMeshVerticesNode(parentNode);
                }
                else if (nodeType == GrnNodeType.MeshNormals)
                {
                    node = new GrnMeshNormalsNode(parentNode);
                }
                else
                {
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (nodeType == GrnNodeType.MeshField)
            {
                node = new GrnMeshFieldNode(parentNode);
            }
            else if (nodeType == GrnNodeType.MeshWeights)
            {
                node = new GrnMeshWeightsNode(parentNode);
            }
            else if (nodeType == GrnNodeType.MeshTriangles)
            {
                node = new GrnMeshTrianglesNode(parentNode);
            }
            else if (nodeType == GrnNodeType.DataExtensionProperty)
            {
                node = new GrnDataExtensionPropertyNode(parentNode);
            }
            else if (nodeType == GrnNodeType.DataExtensionPropertyValue)
            {
                node = new GrnDataExtensionPropertyValueNode(parentNode);
            }
            else if (nodeType == GrnNodeType.DataExtensionReference)
            {
                node = new GrnDataExtensionReferenceNode(parentNode);
            }
            else if (nodeType == GrnNodeType.Bone)
            {
                node = new GrnBoneNode(parentNode);
            }
            else if (nodeType == GrnNodeType.MaterialSimpleDiffuseTexture)
            {
                node = new GrnMaterialSimpleDiffuseTextureNode(parentNode);
            }
            else if (nodeType == GrnNodeType.FormBoneChannels)
            {
                node = new GrnFormBoneChannelsNode(parentNode);
            }
            else if (nodeType == GrnNodeType.FormMesh)
            {
                node = new GrnFormMeshNode(parentNode);
            }
            else if (nodeType == GrnNodeType.FormMeshBone)
            {
                node = new GrnFormMeshBoneNode(parentNode);
            }
            else if (nodeType == GrnNodeType.RenderPass)
            {
                node = new GrnRenderPassNode(parentNode);
            }
            else if (nodeType == GrnNodeType.RenderPassTriangles)
            {
                node = new GrnRenderPassTrianglesNode(parentNode);
            }
            else if (nodeType == GrnNodeType.AnimationTransformTrackKeys)
            {
                node = new GrnAnimationTransformTrackKeysNode(parentNode);
            }
            else if (nodeType == GrnNodeType.TextureMapImage)
            {
                node = new GrnTextureMapImageNode(parentNode);
            }
            else
            {
                node = new GrnNode(parentNode, nodeType);
            }

            node.Read(reader);
            return node;
        }
        protected virtual void Read(GrnBinaryReader reader)
        {
            this.Offset = reader.ReadUInt32();
            this.NumTotalChildNodes = reader.ReadInt32();

            int numChildren = this.NumTotalChildNodes;
            for (int i = 0; i < numChildren; i++)
            {
                GrnNodeType nodeType = (GrnNodeType)reader.ReadInt32();
                GrnNode childNode = GrnNode.ReadByNodeType(reader, this, nodeType);
                this.ChildNodes.Add(childNode);
                numChildren -= childNode.NumTotalChildNodes;
                if (i > 0)
                {
                    childNode.PreviousSibling = this.ChildNodes[i - 1];
                    childNode.PreviousSibling.NextSibling = childNode;
                }
            }
        }
        public virtual void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            Int32 outputLength = this.GetReadDataLength();
            if (outputLength > 0)
            {
                reader.BaseStream.Seek((int)this.Offset + directoryOffset, System.IO.SeekOrigin.Begin);
                Data = reader.ReadBytes(outputLength);
            }

            for (int i = 0; i < this.ChildNodes.Count; i++)
            {
                this.ChildNodes[i].ReadData(reader, directoryOffset);
            }
        }
        public virtual void Write(GrnBinaryWriter writer)
        {
            writer.Write((uint)this.NodeType);
            writer.Write(this.Offset);
            writer.Write(this.NumTotalChildNodes);

            foreach (GrnNode child in this.ChildNodes)
            {
                child.Write(writer);
            }
        }
        public virtual void WriteData(GrnBinaryWriter writer)
        {
            writer.Write(this.Data);

            foreach (GrnNode child in this.ChildNodes)
            {
                child.WriteData(writer);
            }
        }

        public virtual void CreateFolder(string parentDirectory, int fileIndex)
        {
            string newDirectory = System.IO.Path.Combine(parentDirectory, fileIndex + this.NodeType.ToString());
            System.IO.Directory.CreateDirectory(newDirectory);

            this.CreateFolderFile(newDirectory);

            for (int i = 0; i < this.ChildNodes.Count; i++)
            {
                this.ChildNodes[i].CreateFolder(newDirectory, i);
            }
        }
        public virtual void CreateFolderFile(string folder)
        {
            if (this.Data.Length > 0)
            {
                string fileName = System.IO.Path.Combine(folder, "data.bin");
                using (GrnBinaryWriter writer = new GrnBinaryWriter(System.IO.File.Create(fileName)))
                {
                    writer.Write(this.Data);
                }
            }
        }

        public int GetReadDataLength()
        {
            if (this.ChildNodes.Count > 0)
            {
                return (int)(this.ChildNodes[0].Offset - this.Offset);
            }
            else if (this.NextSibling != null)
            {
                return (int)(this.NextSibling.Offset - this.Offset);
            }

            GrnNode? parentNode = this.ParentNode;
            while (parentNode != null)
            {
                if (parentNode.NextSibling != null &&
                    parentNode.NodeType != GrnNodeType.NullTerminator &&
                    parentNode.NodeType != GrnNodeType.VersionFrameDirectory &&
                    parentNode.NodeType != GrnNodeType.StandardFrameDirectory &&
                    parentNode.NodeType != GrnNodeType.NullFrameDirectory &&
                    parentNode.NodeType != GrnNodeType.FileDirectory)
                {
                    return (int)(parentNode.NextSibling.Offset - this.Offset);
                }

                parentNode = parentNode.ParentNode;
            }

            return 0;
            //switch (this.NodeType)
            //{
            //    case GrnNodeType.DataExtensionProperty:
            //    case GrnNodeType.DataExtensionReference:
            //        return 4;
            //        break;
            //    case GrnNodeType.DataExtension:
            //    case GrnNodeType.DataExtensionPropertyValue:
            //        return 8;
            //        break;
            //    default:
            //        return 0;
            //        break;
            //}
        }
        public virtual int GetWriteDataLength()
        {
            return this.Data.Length;
        }

        public virtual void UpdateNumTotalChildNodes()
        {
            this.NumTotalChildNodes = this.ChildNodes.Count;

            for (int i = 0; i < this.ChildNodes.Count; ++i)
            {
                this.ChildNodes[i].UpdateNumTotalChildNodes();
                this.NumTotalChildNodes += this.ChildNodes[i].NumTotalChildNodes;
            }
        }
        public virtual uint UpdateOffset(uint offset)
        {
            this.Offset = offset;
            uint lastOffset = offset + (uint)this.GetWriteDataLength();

            foreach (GrnNode child in this.ChildNodes)
            {
                lastOffset = child.UpdateOffset(lastOffset);
            }

            return lastOffset;
        }

        public virtual void AppendChild(GrnNode node)
        {
            node.ParentNode = this;

            if (this.ChildNodes.Count > 0)
            {
                node.PreviousSibling = this.ChildNodes[this.ChildNodes.Count - 1];
                node.PreviousSibling.NextSibling = node;
            }

            this.ChildNodes.Add(node);
        }

        public virtual List<T> FindNodes<T>(GrnNodeType nodeType)
            where T : GrnNode
        {
            List<T> results = new List<T>();

            if (this.NodeType == nodeType)
            {
                results.Add((T)this);
            }

            for (int i = 0; i < this.ChildNodes.Count; ++i)
            {
                results.AddRange(this.ChildNodes[i].FindNodes<T>(nodeType));
            }

            return results;
        }
        public virtual T? FindNode<T>(GrnNodeType nodeType)
            where T : GrnNode
        {
            if (this.NodeType == nodeType)
            {
                return (T)this;
            }

            for (int i = 0; i < this.ChildNodes.Count; ++i)
            {
                T? result = this.ChildNodes[i].FindNode<T>(nodeType);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
