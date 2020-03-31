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

    public class GrnRenderPassTrianglesNode : GrnNode
    {
        public Int32 Count
        {
            get { return this.TextureIndices.Count; }
        }
        public Dictionary<Int32, List<Int32>> TextureIndices
        {
            get;
            set;
        }

        public GrnRenderPassTrianglesNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.RenderPassTriangles)
        {
            this.TextureIndices = new Dictionary<int, List<int>>();
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.BaseStream.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            int count = reader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                int faceIndex = reader.ReadInt32();
                this.TextureIndices.Add(faceIndex, new List<int>());
                this.TextureIndices[faceIndex].Add(reader.ReadInt32());
                this.TextureIndices[faceIndex].Add(reader.ReadInt32());
                this.TextureIndices[faceIndex].Add(reader.ReadInt32());
            }
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            writer.Write(this.Count);
            foreach (KeyValuePair<int, List<int>> texInd in this.TextureIndices)
            {
                writer.Write(texInd.Key);
                writer.Write(texInd.Value[0]);
                writer.Write(texInd.Value[1]);
                writer.Write(texInd.Value[2]);
            }
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("rpTriangles", new XAttribute("count", this.Count));
            doc.Add(root);

            foreach (KeyValuePair<int, List<int>> texInd in this.TextureIndices)
            {
                XElement face = new XElement("face", new XAttribute("index", texInd.Key));
                root.Add(face);
                face.Add(new XElement("textureCoordinateIndex", texInd.Value[0]));
                face.Add(new XElement("textureCoordinateIndex", texInd.Value[1]));
                face.Add(new XElement("textureCoordinateIndex", texInd.Value[2]));
            }

            string fileName = System.IO.Path.Combine(folder, "rpTriangles.xml");
            using (FileStream stream = System.IO.File.Create(fileName))
            {
                doc.Save(stream);
            }
        }

        public override int GetWriteDataLength()
        {
            return 4 + 16 * this.Count;
        }
    }
}
