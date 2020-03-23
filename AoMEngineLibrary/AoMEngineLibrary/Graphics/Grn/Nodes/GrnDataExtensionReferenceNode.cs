namespace AoMEngineLibrary.Graphics.Grn.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class GrnDataExtensionReferenceNode : GrnNode
    {
        public int DataExtensionIndex
        {
            get;
            set;
        }

        public GrnDataExtensionReferenceNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.DataExtensionReference)
        {
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.BaseStream.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            this.DataExtensionIndex = reader.ReadInt32();
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            writer.Write(this.DataExtensionIndex);
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("deRef");
            doc.Add(root);

            root.Add(new XElement("dataExtensionIndex", this.DataExtensionIndex));

            string fileName = System.IO.Path.Combine(folder, "deRef.xml");
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
