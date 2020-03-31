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

    public class GrnFormMeshNode : GrnNode
    {
        public Int32 MeshIndex
        {
            get;
            set;
        }

        public GrnFormMeshNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.FormMesh)
        {
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.BaseStream.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            this.MeshIndex = reader.ReadInt32();

            for (int i = 0; i < this.ChildNodes.Count; i++)
            {
                this.ChildNodes[i].ReadData(reader, directoryOffset);
            }
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            writer.Write(this.MeshIndex);

            for (int i = 0; i < this.ChildNodes.Count; i++)
            {
                this.ChildNodes[i].WriteData(writer);
            }
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("formMesh");
            doc.Add(root);
            
            root.Add(new XElement("meshIndex", this.MeshIndex));

            string fileName = System.IO.Path.Combine(folder, "formMesh.xml");
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
