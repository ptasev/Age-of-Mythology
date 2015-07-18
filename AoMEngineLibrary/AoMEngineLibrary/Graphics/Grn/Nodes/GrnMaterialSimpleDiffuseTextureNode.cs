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

    public class GrnMaterialSimpleDiffuseTextureNode : GrnNode
    {
        public Int32 Unknown
        {
            get;
            set;
        }

        public Int32 TextureMapIndex
        {
            get;
            set;
        }

        public Int32 Unknown2
        {
            get;
            set;
        }

        public GrnMaterialSimpleDiffuseTextureNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.MaterialSimpleDiffuseTexture)
        {
            Unknown2 = 1;
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            this.Unknown = reader.ReadInt32();
            this.TextureMapIndex = reader.ReadInt32();
            this.Unknown2 = reader.ReadInt32();
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            writer.Write(this.Unknown);
            writer.Write(this.TextureMapIndex);
            writer.Write(this.Unknown2);
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("matDiffTex");
            doc.Add(root);

            root.Add(new XElement("unknown", this.TextureMapIndex));
            root.Add(new XElement("textureMapIndex", this.TextureMapIndex));
            root.Add(new XElement("unknown2", this.Unknown2));

            string fileName = System.IO.Path.Combine(folder, "matDiffTex.xml");
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
