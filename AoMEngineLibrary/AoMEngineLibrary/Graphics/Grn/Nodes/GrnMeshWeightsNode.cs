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

    public class GrnMeshWeightsNode : GrnNode
    {
        public Int32 HighestBoneIndex { get; set; }
        public Int32 HighestVertexWeightCount { get; set; }
        public List<VertexWeight> VertexWeights
        {
            get;
            set;
        }

        public GrnMeshWeightsNode(GrnNode parentNode)
            : base(parentNode, GrnNodeType.MeshWeights)
        {
            this.HighestBoneIndex = 0;
            this.HighestVertexWeightCount = 0;
            this.VertexWeights = new List<VertexWeight>();
        }

        public override void ReadData(GrnBinaryReader reader, int directoryOffset)
        {
            reader.BaseStream.Seek((int)(this.Offset + directoryOffset), SeekOrigin.Begin);
            int weightsCount = reader.ReadInt32();
            this.HighestBoneIndex = reader.ReadInt32(); // unknown (12 Jormund.grn)
            this.HighestVertexWeightCount = reader.ReadInt32(); // unknown (2 Jormund.grn)
            for (int i = 0; i < weightsCount; ++i)
            {
                this.VertexWeights.Add(new VertexWeight());
                int boneCount = reader.ReadInt32();
                for (int j = 0; j < boneCount; ++j)
                {
                    this.VertexWeights[i].BoneIndices.Add(reader.ReadInt32());
                    this.VertexWeights[i].Weights.Add(reader.ReadSingle());
                }
            }
        }

        public override void WriteData(GrnBinaryWriter writer)
        {
            this.CalculateHighestStats();
            writer.Write(this.VertexWeights.Count);
            writer.Write(this.HighestBoneIndex);
            writer.Write(this.HighestVertexWeightCount);
            for (int i = 0; i < this.VertexWeights.Count; ++i)
            {
                writer.Write(this.VertexWeights[i].BoneIndices.Count);
                for (int j = 0; j < this.VertexWeights[i].BoneIndices.Count; ++j)
                {
                    writer.Write(this.VertexWeights[i].BoneIndices[j]);
                    writer.Write(this.VertexWeights[i].Weights[j]);
                }
            }
        }
        public void CalculateHighestStats()
        {
            for (int i = 0; i < this.VertexWeights.Count; ++i)
            {
                this.HighestVertexWeightCount = 
                    Math.Max(this.HighestVertexWeightCount, this.VertexWeights[i].BoneIndices.Count);
                for (int j = 0; j < this.VertexWeights[i].BoneIndices.Count; ++j)
                {
                    this.HighestBoneIndex = 
                        Math.Max(this.HighestBoneIndex, this.VertexWeights[i].BoneIndices[j]);
                }
            }
        }

        public override void CreateFolderFile(string folder)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("meshWeights", new XAttribute("count", this.VertexWeights.Count));
            doc.Add(root);

            root.Add(new XElement("highestBoneIndex", this.HighestBoneIndex));
            root.Add(new XElement("highestVertexWeightCount", this.HighestVertexWeightCount));
            for (int i = 0; i < this.VertexWeights.Count; ++i)
            {
                XElement vWeight = new XElement("vertex", 
                    new XAttribute("count", this.VertexWeights[i].BoneIndices.Count));
                root.Add(vWeight);
                for (int j = 0; j < this.VertexWeights[i].BoneIndices.Count; ++j)
                {
                    vWeight.Add(new XElement("vertexWeight", 
                        new XAttribute("boneIndex", this.VertexWeights[i].BoneIndices[j]),
                        new XAttribute("weight", this.VertexWeights[i].Weights[j])));
                }
            }

            string fileName = System.IO.Path.Combine(folder, "meshWeights.xml");
            using (FileStream stream = System.IO.File.Create(fileName))
            {
                doc.Save(stream);
            }
        }

        public override int GetWriteDataLength()
        {
            int ret = 12;

            for (int i = 0; i < this.VertexWeights.Count; ++i)
            {
                ret += 4;
                for (int j = 0; j < this.VertexWeights[i].BoneIndices.Count; ++j)
                {
                    ret += 8;
                }
            }

            return ret;
        }
    }
}
