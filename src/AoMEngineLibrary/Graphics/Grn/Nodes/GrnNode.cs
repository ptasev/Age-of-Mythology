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
            GrnNode? node = null;
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
            else if (nodeType == GrnNodeType.NullTerminator)
            {
                // Null-terminator has no data
                node = new GrnNode(parentNode, nodeType);
            }
            else if (nodeType == GrnNodeType.DataExtensionReference)
            {
                node = new GrnDataExtensionReferenceNode(parentNode);
            }
            else if (parentNode.NodeType == GrnNodeType.VersionFrameDirectory)
            {
                if (nodeType == GrnNodeType.VersionSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.StandardFrameDirectory)
            {
                if (nodeType == GrnNodeType.DataExtensionSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.VectorChannelSection)
                {
                    // I haven't seen this node have data yet
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.TransformChannelSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.SkeletonSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.MeshSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.TextureSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.MaterialSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.FormSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.ModelSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.AnimationSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.VersionSection)
            {
                if (nodeType == GrnNodeType.ExporterVersion)
                {
                    // Has data (3 ints) and a child
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.RuntimeVersion)
                {
                    // Has data (4 ints)
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.ExporterVersion)
            {
                if (nodeType == GrnNodeType.ModelerAxisSystem)
                {
                    // Has data (3 floats for origin, 3 for right vec, 3 for up vec, 3 for back vec)
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.DataExtensionSection)
            {
                if (nodeType == GrnNodeType.DataExtension)
                {
                    // Has data (2 ints), has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.DataExtension)
            {
                if (nodeType == GrnNodeType.DataExtensionPropertySection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.DataExtensionPropertySection)
            {
                if (nodeType == GrnNodeType.DataExtensionProperty)
                {
                    node = new GrnDataExtensionPropertyNode(parentNode);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.DataExtensionProperty)
            {
                if (nodeType == GrnNodeType.DataExtensionValueSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.DataExtensionValueSection)
            {
                if (nodeType == GrnNodeType.DataExtensionPropertyValue)
                {
                    node = new GrnDataExtensionPropertyValueNode(parentNode);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.TransformChannelSection)
            {
                if (nodeType == GrnNodeType.TransformChannel)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.TransformChannel)
            {
            }
            else if (parentNode.NodeType == GrnNodeType.MeshSection)
            {
                if (nodeType == GrnNodeType.Mesh)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.Mesh)
            {
                if (nodeType == GrnNodeType.MeshVertexSetSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.MeshWeights)
                {
                    node = new GrnMeshWeightsNode(parentNode);
                }
                else if (nodeType == GrnNodeType.MeshTriangles)
                {
                    node = new GrnMeshTrianglesNode(parentNode);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.MeshVertexSetSection)
            {
                if (nodeType == GrnNodeType.MeshVertexSet)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
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
                else if (nodeType == GrnNodeType.MeshFieldSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.MeshFieldSection)
            {
                if (nodeType == GrnNodeType.MeshField)
                {
                    node = new GrnMeshFieldNode(parentNode);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.SkeletonSection)
            {
                if (nodeType == GrnNodeType.Skeleton)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.Skeleton)
            {
                if (nodeType == GrnNodeType.BoneSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.BoneSection)
            {
                if (nodeType == GrnNodeType.Bone)
                {
                    node = new GrnBoneNode(parentNode);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.TextureSection)
            {
                if (nodeType == GrnNodeType.TextureMap)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.TextureMap)
            {
                if (nodeType == GrnNodeType.TextureImageSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.TextureImageSection)
            {
                if (nodeType == GrnNodeType.TextureMapImage)
                {
                    node = new GrnTextureMapImageNode(parentNode);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.MaterialSection)
            {
                if (nodeType == GrnNodeType.Material)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.Material)
            {
                if (nodeType == GrnNodeType.MaterialSimpleDiffuseTexture)
                {
                    node = new GrnMaterialSimpleDiffuseTextureNode(parentNode);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.FormSection)
            {
                if (nodeType == GrnNodeType.Form)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.Form)
            {
                if (nodeType == GrnNodeType.FormSkeletonSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.FormMeshSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.FormSkeletonSection)
            {
                if (nodeType == GrnNodeType.FormSkeleton)
                {
                    // Has data (1 int), has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.FormSkeleton)
            {
                if (nodeType == GrnNodeType.FormBoneChannels)
                {
                    node = new GrnFormBoneChannelsNode(parentNode);
                }
                else if (nodeType == GrnNodeType.FormPoseWeights)
                {
                    // I haven't seen this node have data yet
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.FormMeshSection)
            {
                if (nodeType == GrnNodeType.FormMesh)
                {
                    node = new GrnFormMeshNode(parentNode);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.FormMesh)
            {
                if (nodeType == GrnNodeType.FormVertexSetWeights)
                {
                    // I haven't seen this node have data yet
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.FormMeshBoneSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.FormMeshBoneSection)
            {
                if (nodeType == GrnNodeType.FormMeshBone)
                {
                    node = new GrnFormMeshBoneNode(parentNode);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.ModelSection)
            {
                if (nodeType == GrnNodeType.Model)
                {
                    // Has data (1 int), has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.Model)
            {
                if (nodeType == GrnNodeType.RenderPassSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.RenderPassSection)
            {
                if (nodeType == GrnNodeType.RenderPass)
                {
                    node = new GrnRenderPassNode(parentNode);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.RenderPass)
            {
                if (nodeType == GrnNodeType.RenderPassFieldSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.RenderPassTriangles)
                {
                    node = new GrnRenderPassTrianglesNode(parentNode);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.RenderPassFieldSection)
            {
                if (nodeType == GrnNodeType.RenderPassFieldConstant)
                {
                    // Has data (3 floats), no children
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.RenderPassFieldAssignment)
                {
                    // Has data (1 int), no children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.AnimationSection)
            {
                if (nodeType == GrnNodeType.Animation)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.Animation)
            {
                if (nodeType == GrnNodeType.AnimationVectorTrackSection)
                {
                    // I haven't seen this node have data yet
                    node = new GrnNode(parentNode, nodeType);
                }
                else if (nodeType == GrnNodeType.AnimationTransformTrackSection)
                {
                    // No data, has children
                    node = new GrnNode(parentNode, nodeType);
                }
            }
            else if (parentNode.NodeType == GrnNodeType.AnimationTransformTrackSection)
            {
                if (nodeType == GrnNodeType.AnimationTransformTrackKeys)
                {
                    node = new GrnAnimationTransformTrackKeysNode(parentNode);
                }
            }

            if (node == null)
                throw new NotImplementedException($"The library doesn't support grn with {nodeType} nodes.");

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
