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

    public class GrnMeshTrianglesNode : GrnNode
    {
        public List<Face> Faces
        {
            get;
            set;
        }

        public GrnMeshTrianglesNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.MeshTriangles)
        {
            this.Faces = new List<Face>();
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            for (int i = 0; i < this.GetReadDataLength() / 24; ++i)
            {
                this.Faces.Add(new Face());
                this.Faces[i].Indices.Add((Int16)reader.ReadInt32());
                this.Faces[i].Indices.Add((Int16)reader.ReadInt32());
                this.Faces[i].Indices.Add((Int16)reader.ReadInt32());
                this.Faces[i].NormalIndices.Add(reader.ReadInt32());
                this.Faces[i].NormalIndices.Add(reader.ReadInt32());
                this.Faces[i].NormalIndices.Add(reader.ReadInt32());
            }
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            for (int i = 0; i < this.Faces.Count; ++i)
            {
                for (int j = 0; j < this.Faces[i].Indices.Count; ++j)
                {
                    writer.Write((Int32)this.Faces[i].Indices[j]);
                }
                for (int j = 0; j < this.Faces[i].NormalIndices.Count; ++j)
                {
                    writer.Write(this.Faces[i].NormalIndices[j]);
                }
            }
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("meshTriangles", new XAttribute("count", this.Faces.Count));
            doc.Add(root);

            for (int i = 0; i < this.Faces.Count; ++i)
            {
                XElement face = new XElement("face", new XAttribute("index", i));
                root.Add(face);
                for (int j = 0; j < this.Faces[i].Indices.Count; ++j)
                {
                    face.Add(new XElement("index", this.Faces[i].Indices[j]));
                }
                for (int j = 0; j < this.Faces[i].NormalIndices.Count; ++j)
                {
                    face.Add(new XElement("normalIndex", this.Faces[i].NormalIndices[j]));
                }
            }

            string fileName = System.IO.Path.Combine(folder, "meshTriangles.xml");
            using (FileStream stream = System.IO.File.Create(fileName))
            {
                doc.Save(stream);
            }
        }

        public override int GetWriteDataLength()
        {
            return 24 * this.Faces.Count;
        }
    }
}
