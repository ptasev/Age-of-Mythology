namespace AoMEngineLibrary.Graphics.Grn.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class GrnDataExtensionPropertyNode : GrnNode
    {
        public int StringTableIndex
        {
            get;
            set;
        }

        public GrnDataExtensionPropertyNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.DataExtensionProperty)
        {
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.BaseStream.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            this.StringTableIndex = reader.ReadInt32();

            for (int i = 0; i < this.ChildNodes.Count; i++)
            {
                this.ChildNodes[i].ReadData(reader, directoryOffset);
            }
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            writer.Write(this.StringTableIndex);

            for (int i = 0; i < this.ChildNodes.Count; i++)
            {
                this.ChildNodes[i].WriteData(writer);
            }
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("deProp");
            doc.Add(root);

            root.Add(new XElement("stringTableIndex", this.StringTableIndex));

            string fileName = System.IO.Path.Combine(folder, "deProp.xml");
            using (FileStream stream = System.IO.File.Create(fileName))
            {
                doc.Save(stream);
            }
        }

        public override int GetWriteDataLength()
        {
            return 4;
        }
    }
}
