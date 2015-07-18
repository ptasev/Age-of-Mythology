namespace AoMEngineLibrary.Graphics.Grn.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GrnMainNode : GrnNode
    {
        public UInt32 FileLength
        {
            get;
            set;
        }

        public GrnMainNode()
            : base(null, GrnNodeType.FileDirectory)
        {
            this.FileLength = 0;
        }

        protected override void Read(GrnBinaryReader reader)
        {
            this.NumTotalChildNodes = reader.ReadInt32();

            reader.ReadBytes(8);
            this.FileLength = reader.ReadUInt32();
            reader.ReadBytes(12);

            for (int i = 0; i < NumTotalChildNodes; i++)
            {
                GrnNodeType nodeType = (GrnNodeType)reader.ReadInt32();
                GrnNode childNode = GrnNode.ReadByNodeType(reader, this, nodeType);
                this.ChildNodes.Add(childNode);
                if (i > 0)
                {
                    childNode.PreviousSibling = this.ChildNodes[i - 1];
                    childNode.PreviousSibling.NextSibling = childNode;
                }
            }

            //this.ReadData(reader, 0);
        }
        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            for (int i = 0; i < this.ChildNodes.Count; i++)
            {
                this.ChildNodes[i].ReadData(reader, 0);
            }
        }

        public override void Write(GrnBinaryWriter writer)
        {
            this.UpdateNumTotalChildNodes();
            this.UpdateOffset(0);

            writer.Write((uint)this.NodeType);
            writer.Write(this.NumTotalChildNodes);
            writer.Write(new byte[8]);
            writer.Write(this.FileLength);
            writer.Write(new byte[12]);

            foreach (GrnNode child in this.ChildNodes)
            {
                child.Write(writer);
            }

            foreach (GrnNode child in this.ChildNodes)
            {
                child.WriteData(writer);
            }
        }

        public override void UpdateNumTotalChildNodes()
        {
            this.NumTotalChildNodes = this.ChildNodes.Count;

            for (int i = 0; i < this.ChildNodes.Count; ++i)
            {
                this.ChildNodes[i].UpdateNumTotalChildNodes();
            }
        }
        public override uint UpdateOffset(uint offset)
        {
            uint lastOffset = 64 + 32 + 20 * (uint)this.ChildNodes.Count;

            foreach (GrnNode child in this.ChildNodes)
            {
                lastOffset = child.UpdateOffset(lastOffset);
            }
            this.FileLength = lastOffset - 64;

            return 64;
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
