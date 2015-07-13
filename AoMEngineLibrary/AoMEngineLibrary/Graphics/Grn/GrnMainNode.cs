namespace AoMEngineLibrary.Graphics.Grn
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
        internal override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            for (int i = 0; i < this.ChildNodes.Count; i++)
            {
                this.ChildNodes[i].ReadData(reader, 0);
            }
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
