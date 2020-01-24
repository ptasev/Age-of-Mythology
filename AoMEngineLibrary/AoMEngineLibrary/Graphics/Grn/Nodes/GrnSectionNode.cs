namespace AoMEngineLibrary.Graphics.Grn.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GrnSectionNode : GrnNode
    {

        public GrnSectionNode(GrnNode? parentNode, GrnNodeType nodeType)
            : base(parentNode, nodeType)
        {

        }

        protected override void Read(GrnBinaryReader reader)
        {
            reader.ReadBytes(4);
            this.Offset = reader.ReadUInt32();
            reader.ReadBytes(8);

            int currentPosition = (Int32)reader.BaseStream.Position;
            reader.Seek((int)this.Offset, System.IO.SeekOrigin.Begin);
            this.NumTotalChildNodes = reader.ReadInt32();
            reader.ReadBytes(12);

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

            reader.Seek(currentPosition, System.IO.SeekOrigin.Begin);
        }
        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            for (int i = 0; i < this.ChildNodes.Count; i++)
            {
                this.ChildNodes[i].ReadData(reader, (int)this.Offset);
            }
        }

        public override void Write(GrnBinaryWriter writer)
        {
            writer.Write((uint)this.NodeType);
            writer.Write(0);
            writer.Write(this.Offset);
            writer.Write(0L);
        }
        public override void WriteData(GrnBinaryWriter writer)
        {
            writer.Write(this.NumTotalChildNodes);
            if (this.NodeType == GrnNodeType.NullFrameDirectory)
            {
                writer.Write(1240936);
                writer.Write(1240936);
                writer.Write(1241292);
            }
            else
            {
                writer.Write(0);
                writer.Write(this.ParentNode?.FirstChild?.Offset ??
                    throw new InvalidDataException("Cannot write data of a SectionNode if it has no parent, or siblings"));
                writer.Write(0);
            }

            foreach (GrnNode child in this.ChildNodes)
            {
                child.Write(writer);
            }

            foreach (GrnNode child in this.ChildNodes)
            {
                child.WriteData(writer);
            }
        }

        public override uint UpdateOffset(uint offset)
        {
            uint lastOffset = base.UpdateOffset(16 + 12 * (uint)this.NumTotalChildNodes);
            this.Offset = offset;

            return lastOffset + offset;
        }

        public override void CreateFolder(string parentDirectory, int fileIndex)
        {
            string newDirectory = System.IO.Path.Combine(parentDirectory, this.NodeType.ToString());
            System.IO.Directory.CreateDirectory(newDirectory);

            for (int i = 0; i < this.ChildNodes.Count; i++)
            {
                this.ChildNodes[i].CreateFolder(newDirectory, i);
            }
        }
    }
}
