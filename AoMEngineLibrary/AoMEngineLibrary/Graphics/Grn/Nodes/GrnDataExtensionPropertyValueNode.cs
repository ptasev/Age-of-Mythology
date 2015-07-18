namespace AoMEngineLibrary.Graphics.Grn.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class GrnDataExtensionPropertyValueNode : GrnNode
    {
        public int Unknown
        {
            get;
            set;
        }
        public int StringTableIndex
        {
            get;
            set;
        }

        public GrnDataExtensionPropertyValueNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.DataExtensionPropertyValue)
        {
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            this.Unknown = reader.ReadInt32();
            this.StringTableIndex = reader.ReadInt32();
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            writer.Write(this.Unknown);
            writer.Write(this.StringTableIndex);
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("dePropVal");
            doc.Add(root);

            root.Add(new XElement("unknown", this.Unknown));
            root.Add(new XElement("stringTableIndex", this.StringTableIndex));

            string fileName = System.IO.Path.Combine(folder, "dePropVal.xml");
            using (FileStream stream = System.IO.File.Create(fileName))
            {
                doc.Save(stream);
            }
        }

        public override int GetWriteDataLength()
        {
            return 8;
        }
    }
}
