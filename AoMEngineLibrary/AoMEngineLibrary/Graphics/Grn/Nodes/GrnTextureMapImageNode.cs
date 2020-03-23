namespace AoMEngineLibrary.Graphics.Grn.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class GrnTextureMapImageNode : GrnNode
    {
        public Int32 Width
        {
            get;
            set;
        }

        public Int32 Height
        {
            get;
            set;
        }

        public Int32 Unknown
        {
            get;
            set;
        }

        public GrnTextureMapImageNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.TextureMapImage)
        {
            this.Unknown = 7;
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.BaseStream.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            this.Width = reader.ReadInt32();
            this.Height = reader.ReadInt32();
            this.Unknown = reader.ReadInt32();
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            writer.Write(this.Width);
            writer.Write(this.Height);
            writer.Write(this.Unknown);
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("texMapImage");
            doc.Add(root);

            root.Add(new XElement("width", this.Width));
            root.Add(new XElement("height", this.Height));
            root.Add(new XElement("unknown", this.Unknown));

            string fileName = System.IO.Path.Combine(folder, "texMapImage.xml");
            using (FileStream stream = System.IO.File.Create(fileName))
            {
                doc.Save(stream);
            }
        }

        public override int GetWriteDataLength()
        {
            return 12;
        }
    }
}
