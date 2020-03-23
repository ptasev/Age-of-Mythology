namespace AoMEngineLibrary.Graphics.Grn.Nodes
{
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class GrnFormBoneChannelsNode : GrnNode
    {
        public List<Int32> TransformChannelIndices
        {
            get;
            set;
        }

        public GrnFormBoneChannelsNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.FormBoneChannels)
        {
            this.TransformChannelIndices = new List<int>();
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.BaseStream.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            for (int i = 0; i < this.GetReadDataLength() / 4; ++i)
            {
                this.TransformChannelIndices.Add(reader.ReadInt32());
            }
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            for (int i = 0; i < this.TransformChannelIndices.Count; ++i)
            {
                writer.Write(this.TransformChannelIndices[i]);
            }
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("fBoneChan", 
                new XAttribute("count", this.TransformChannelIndices.Count));
            doc.Add(root);
            for (int i = 0; i < this.TransformChannelIndices.Count; ++i)
            {
                root.Add(new XElement("transformChannelIndex", 
                    new XAttribute("index", i), this.TransformChannelIndices[i]));
            }

            string fileName = System.IO.Path.Combine(folder, "fBoneChan.xml");
            using (FileStream stream = System.IO.File.Create(fileName))
            {
                doc.Save(stream);
            }
        }

        public override int GetWriteDataLength()
        {
            return 4 * this.TransformChannelIndices.Count;
        }
    }
}
