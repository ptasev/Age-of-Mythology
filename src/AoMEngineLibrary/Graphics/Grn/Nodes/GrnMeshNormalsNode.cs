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

    public class GrnMeshNormalsNode : GrnNode
    {
        public List<Vector3> Normals
        {
            get;
            set;
        }

        public GrnMeshNormalsNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.MeshNormals)
        {
            this.Normals = new List<Vector3>();
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.BaseStream.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            for (int i = 0; i < this.GetReadDataLength() / 12; ++i)
            {
                this.Normals.Add(reader.ReadVector3D());
            }
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            for (int i = 0; i < this.Normals.Count; ++i)
            {
                writer.Write(this.Normals[i]);
            }
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("meshNormals", new XAttribute("count", this.Normals.Count));
            doc.Add(root);
            for (int i = 0; i < this.Normals.Count; ++i)
            {
                root.Add(new XElement("normal", new XAttribute("index", i), this.Normals[i]));
            }

            string fileName = System.IO.Path.Combine(folder, "meshNormals.xml");
            using (FileStream stream = System.IO.File.Create(fileName))
            {
                doc.Save(stream);
            }
        }

        public override int GetWriteDataLength()
        {
            return 12 * this.Normals.Count;
        }
    }
}
