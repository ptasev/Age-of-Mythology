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

    public class GrnMeshFieldNode : GrnNode
    {
        public Int32 Unknown { get; set; }
        public List<Vector3D> TextureCoordinates
        {
            get;
            set;
        }

        public GrnMeshFieldNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.MeshField)
        {
            this.Unknown = 3;
            this.TextureCoordinates = new List<Vector3D>();
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            this.Unknown = reader.ReadInt32(); // maybe count for whether its UV or UVW
            for (int i = 0; i < this.GetReadDataLength() / 12; ++i)
            {
                this.TextureCoordinates.Add(reader.ReadVector3D());
            }
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            writer.Write(this.Unknown);
            for (int i = 0; i < this.TextureCoordinates.Count; ++i)
            {
                writer.Write(this.TextureCoordinates[i]);
            }
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("meshField", new XAttribute("count", this.TextureCoordinates.Count));
            doc.Add(root);

            root.Add(new XElement("unknown", this.Unknown));
            for (int i = 0; i < this.TextureCoordinates.Count; ++i)
            {
                root.Add(new XElement("texCoord", new XAttribute("index", i), this.TextureCoordinates[i]));
            }

            string fileName = System.IO.Path.Combine(folder, "meshField.xml");
            using (FileStream stream = System.IO.File.Create(fileName))
            {
                doc.Save(stream);
            }
        }

        public override int GetWriteDataLength()
        {
            return 4 + 12 * this.TextureCoordinates.Count;
        }
    }
}
