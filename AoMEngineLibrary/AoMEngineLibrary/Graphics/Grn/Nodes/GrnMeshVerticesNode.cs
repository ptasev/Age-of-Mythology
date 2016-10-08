namespace AoMEngineLibrary.Graphics.Grn.Nodes
{
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public class GrnMeshVerticesNode : GrnNode
    {
        public List<Vector3> Vertices
        {
            get;
            set;
        }

        public GrnMeshVerticesNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.MeshVertices)
        {
            this.Vertices = new List<Vector3>();
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            for (int i = 0; i < this.GetReadDataLength() / 12; ++i)
            {
                this.Vertices.Add(reader.ReadVector3D());
            }
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            for (int i = 0; i < this.Vertices.Count; ++i)
            {
                writer.Write(this.Vertices[i]);
            }
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("meshVertices", new XAttribute("count", this.Vertices.Count));
            doc.Add(root);
            for (int i = 0; i < this.Vertices.Count; ++i)
            {
                root.Add(new XElement("vertex", new XAttribute("index", i), this.Vertices[i]));
            }

            string fileName = System.IO.Path.Combine(folder, "meshVertices.xml");
            using (FileStream stream = System.IO.File.Create(fileName))
            {
                doc.Save(stream);
            }
        }

        public override int GetWriteDataLength()
        {
            return 12 * this.Vertices.Count;
        }
    }
}
