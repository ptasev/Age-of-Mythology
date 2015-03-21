namespace AoMEngineLibrary.Graphics.Grn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GrnNode
    {
        public GrnNode ParentNode
        {
            get;
            set;
        }
        public GrnNode PreviousSibling
        {
            get;
            internal set;
        }
        public GrnNode NextSibling
        {
            get;
            internal set;
        }
        public GrnNode FirstChild
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
            protected set;
        }
        public List<GrnNode> ChildNodes
        {
            get;
            set;
        }

        private byte[] data;

        public GrnNode(GrnNode parentNode, GrnNodeType nodeType)
        {
            this.ParentNode = parentNode;
            this.PreviousSibling = null;
            this.NextSibling = null;
            this.NodeType = nodeType;
            this.Offset = 0;
            this.ChildNodes = new List<GrnNode>();
            this.data = new byte[0];
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
        internal virtual void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            Int32 outputLength = this.GetDataLength();
            if (outputLength > 0)
            {
                reader.Seek((int)this.Offset + directoryOffset, System.IO.SeekOrigin.Begin);
                data = reader.ReadBytes(outputLength);
            }

            for (int i = 0; i < this.ChildNodes.Count; i++)
            {
                this.ChildNodes[i].ReadData(reader, directoryOffset);
            }
        }

        public static GrnNode ReadByNodeType(GrnBinaryReader reader, GrnNode parentNode, GrnNodeType nodeType)
        {
            GrnNode node;
            if (nodeType == GrnNodeType.FileDirectory)
            {
                node = new GrnMainNode();
            }
            else if (nodeType == GrnNodeType.VersionFrameDirectory ||
                nodeType == GrnNodeType.StandardFrameDirectory ||
                nodeType == GrnNodeType.NullFrameDirectory)
            {
                node = new GrnSectionNode(parentNode, nodeType);
            }
            else
            {
                node = new GrnNode(parentNode, nodeType);
            }

            node.Read(reader);
            return node;
        }

        public virtual void CreateFolder(string parentDirectory, int fileIndex)
        {
            string newDirectory = System.IO.Path.Combine(parentDirectory, fileIndex + this.NodeType.ToString());
            System.IO.Directory.CreateDirectory(newDirectory);

            if (this.data.Length > 0)
            {
                string fileName = System.IO.Path.Combine(newDirectory, "data.bin");
                using (MiscUtil.IO.EndianBinaryWriter writer =
                    new MiscUtil.IO.EndianBinaryWriter(new MiscUtil.Conversion.LittleEndianBitConverter(),
                        System.IO.File.Create(fileName)))
                {
                    writer.Write(this.data);
                }
            }

            for (int i = 0; i < this.ChildNodes.Count; i++)
            {
                this.ChildNodes[i].CreateFolder(newDirectory, i);
            }
        }

        public int GetDataLength()
        {
            int temp;
            if (this.NodeType == GrnNodeType.DataExtensionPropertyValue)
            {
                temp = 5;
            }

            if (this.ChildNodes.Count > 0)
            {
                return (int)(this.ChildNodes[0].Offset - this.Offset);
            }
            else if (this.NextSibling != null)
            {
                return (int)(this.NextSibling.Offset - this.Offset);
            }

            GrnNode parentNode = this.ParentNode;
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
    }
}
