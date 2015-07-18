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

    public class GrnRenderPassNode : GrnNode
    {
        public Int32 FormMeshIndex
        {
            get;
            set;
        }

        public Int32 MaterialIndex
        {
            get;
            set;
        }

        public GrnRenderPassNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.RenderPass)
        {
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            this.FormMeshIndex = reader.ReadInt32();
            this.MaterialIndex = reader.ReadInt32();

            for (int i = 0; i < this.ChildNodes.Count; i++)
            {
                this.ChildNodes[i].ReadData(reader, directoryOffset);
            }
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            writer.Write(this.FormMeshIndex);
            writer.Write(this.MaterialIndex);

            for (int i = 0; i < this.ChildNodes.Count; i++)
            {
                this.ChildNodes[i].WriteData(writer);
            }
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("renderPass");
            doc.Add(root);

            root.Add(new XElement("formMeshIndex", this.FormMeshIndex));
            root.Add(new XElement("materialIndex", this.MaterialIndex));

            string fileName = System.IO.Path.Combine(folder, "renderPass.xml");
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
