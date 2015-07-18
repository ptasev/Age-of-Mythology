namespace AoMEngineLibrary.Graphics.Grn.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class GrnStringTableNode : GrnNode
    {
        public List<string> Strings
        {
            get;
            set;
        }

        public GrnStringTableNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.StringTable)
        {
            this.Strings = new List<string>();
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            Int32 numString = reader.ReadInt32();
            Int32 stringDataLength = reader.ReadInt32();

            Strings.Capacity = numString;
            for (int i = 0; i < numString; i++)
            {
                Strings.Add(reader.ReadString());
            }

            //reader.ReadBytes((-stringDataLength) & 3); // padding
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            writer.Write(this.Strings.Count);

            int strLength = 0;
            foreach (string s in this.Strings)
            {
                strLength += s.Length + 1;
            }
            writer.Write(strLength);

            for (int i = 0; i < this.Strings.Count; ++i)
            {
                writer.Write(this.Strings[i]);
            }

            byte[] padding = new byte[(-strLength) & 3];
            writer.Write(padding);
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("strings", new XAttribute("count", this.Strings.Count));
            doc.Add(root);
            for (int i = 0; i < this.Strings.Count; ++i)
            {
                root.Add(new XElement("string", new XAttribute("index", i), this.Strings[i]));
            }

            string fileName = System.IO.Path.Combine(folder, "strings.xml");
            using (FileStream stream = System.IO.File.Create(fileName))
            {
                doc.Save(stream);
            }
        }

        public override int GetWriteDataLength()
        {
            int strLength = 0;
            foreach (string s in this.Strings)
            {
                strLength += s.Length + 1;
            }

            return strLength + 8 + ((-strLength) & 3);
        }
    }
}
