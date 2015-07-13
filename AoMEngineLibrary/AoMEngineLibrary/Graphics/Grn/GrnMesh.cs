namespace AoMEngineLibrary.Graphics.Grn
{
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GrnMesh : Mesh
    {
        public GrnFile ParentFile { get; set; }
        public Int32 DataExtensionIndex { get; set; }
        public string Name
        {
            get
            {
                return this.ParentFile.GetDataExtensionObjectName(this.DataExtensionIndex);
            }
        }

        public List<VertexWeight> VertexWeights { get; set; }
        public List<GrnBoneBinding> BoneBindings { get; set; }

        public GrnMesh()
            : base()
        {
            this.VertexWeights = new List<VertexWeight>();
            this.BoneBindings = new List<GrnBoneBinding>();
        }
        public GrnMesh(GrnFile parentFile)
            : this()
        {
            this.ParentFile = parentFile;
        }

        public void Read(GrnBinaryReader reader, GrnNode meshNode, uint directoryOffset)
        {
            GrnNode meshVertices = meshNode.FindNodes(GrnNodeType.MeshVertices)[0];
            reader.Seek((int)(meshVertices.Offset + directoryOffset), SeekOrigin.Begin);
            for (int i = 0; i < meshVertices.GetDataLength() / 12; ++i)
            {
                this.Vertices.Add(reader.ReadVector3D());
            }

            GrnNode meshNormals = meshNode.FindNodes(GrnNodeType.MeshNormals)[0];
            reader.Seek((int)(meshNormals.Offset + directoryOffset), SeekOrigin.Begin);
            for (int i = 0; i < meshNormals.GetDataLength() / 12; ++i)
            {
                this.Normals.Add(reader.ReadVector3D());
            }

            GrnNode meshTexCoords = meshNode.FindNode(GrnNodeType.MeshField);
            if (meshTexCoords != null)
            {
                reader.Seek((int)(meshTexCoords.Offset + directoryOffset), SeekOrigin.Begin);
                reader.ReadInt32(); // unknown, maybe link to map object, or whether its UV or UVW
                for (int i = 0; i < meshTexCoords.GetDataLength() / 12; ++i)
                {
                    this.TextureCoordinates.Add(reader.ReadVector3D());
                }
            }

            GrnNode meshWeights = meshNode.FindNodes(GrnNodeType.MeshWeights)[0];
            reader.Seek((int)(meshWeights.Offset + directoryOffset), SeekOrigin.Begin);
            int weightsCount = reader.ReadInt32();
            reader.ReadInt32(); // unknown (12 Jormund.grn)
            reader.ReadInt32(); // unknown (2 Jormund.grn)
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

            Dictionary<int, List<int>> dd = new Dictionary<int, List<int>>();
            GrnNode meshTriangles = meshNode.FindNodes(GrnNodeType.MeshTriangles)[0];
            reader.Seek((int)(meshTriangles.Offset + directoryOffset), SeekOrigin.Begin);
            for (int i = 0; i < meshTriangles.GetDataLength() / 24; ++i)
            {
                this.Faces.Add(new Face());
                this.Faces[i].Indices.Add((Int16)reader.ReadInt32());
                this.Faces[i].Indices.Add((Int16)reader.ReadInt32());
                this.Faces[i].Indices.Add((Int16)reader.ReadInt32());
                this.Faces[i].NormalIndices.Add(reader.ReadInt32());
                this.Faces[i].NormalIndices.Add(reader.ReadInt32());
                this.Faces[i].NormalIndices.Add(reader.ReadInt32());
                if (!dd.ContainsKey(this.Faces[i].Indices[0]))
                {
                    dd.Add(this.Faces[i].Indices[0], new List<int>());
                }
                if (!dd.ContainsKey(this.Faces[i].Indices[1]))
                {
                    dd.Add(this.Faces[i].Indices[1], new List<int>());
                }
                if (!dd.ContainsKey(this.Faces[i].Indices[2]))
                {
                    dd.Add(this.Faces[i].Indices[2], new List<int>());
                }

                //dd[this.Faces[i].Indices[0]].Add(one);
                //dd[this.Faces[i].Indices[1]].Add(two);
                //dd[this.Faces[i].Indices[2]].Add(three);

                //if (one != this.Faces[i].Indices[0] || two != this.Faces[i].Indices[1] || three != this.Faces[i].Indices[2])
                //{
                //    two = 5;
                //}
                //if (one >= this.Vertices.Count || two >= this.Vertices.Count || three >= this.Vertices.Count)
                //{
                //    two = 5;
                //}
                //if (one >= this.Normals.Count || two >= this.Normals.Count || three >= this.Normals.Count)
                //{
                //    two = 5;
                //}
            }

            Dictionary<int, int> aaa = new Dictionary<int, int>();
            foreach (KeyValuePair<int, List<int>> dddd in dd)
            {
                foreach (int nIndex in dddd.Value)
                {

                }
                if (dddd.Value.Count > 1)
                {
                    aaa.Add(dddd.Key, dddd.Value.Count - 1);
                }
            }

            int tttt = 0;
            foreach (KeyValuePair<int, int> a in aaa)
            {
                tttt += a.Value;
            }

            GrnNode dataExtRef = meshNode.FindNodes(GrnNodeType.DataExtensionReference)[0];
            reader.Seek((int)(dataExtRef.Offset + directoryOffset), SeekOrigin.Begin);
            this.DataExtensionIndex = reader.ReadInt32() - 1;
        }
        public void ReadFormMeshBones(GrnBinaryReader reader, List<GrnNode> formMeshBones, uint directoryOffset)
        {
            this.BoneBindings = new List<GrnBoneBinding>(formMeshBones.Count);
            for (int i = 0; i < formMeshBones.Count; ++i)
            {
                reader.Seek((int)(formMeshBones[i].Offset + directoryOffset), SeekOrigin.Begin);
                this.BoneBindings.Add(new GrnBoneBinding());
                this.BoneBindings[i].BoneIndex = reader.ReadInt32();
                this.BoneBindings[i].Unknown = reader.ReadSingle();
                this.BoneBindings[i].OBBMin = reader.ReadVector3D();
                this.BoneBindings[i].OBBMax = reader.ReadVector3D();
            }
        }
        public void ReadRenderPassTriangles(GrnBinaryReader reader, int matIndex, GrnNode renderPassTriangles, uint directoryOffset)
        {
            reader.Seek((int)(renderPassTriangles.Offset + directoryOffset), SeekOrigin.Begin);
            int count = reader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                int faceIndex = reader.ReadInt32();
                this.Faces[faceIndex].MaterialIndex = (Int16)matIndex;
                this.Faces[faceIndex].TextureIndices.Add(reader.ReadInt32());
                this.Faces[faceIndex].TextureIndices.Add(reader.ReadInt32());
                this.Faces[faceIndex].TextureIndices.Add(reader.ReadInt32());
            }
        }

        public void CalculateUniqueMap()
        {
            HashSet<Tuple<int, int>> unique = new HashSet<Tuple<int, int>>();
            HashSet<Tuple<int, int>> unique2 = new HashSet<Tuple<int, int>>();

            foreach (Face f in this.Faces)
            {
                unique.Add(new Tuple<int, int>(f.Indices[0], f.NormalIndices[0]));
                unique.Add(new Tuple<int, int>(f.Indices[1], f.NormalIndices[1]));
                unique.Add(new Tuple<int, int>(f.Indices[2], f.NormalIndices[2]));

                unique2.Add(new Tuple<int, int>(f.Indices[0], f.TextureIndices[0]));
                unique2.Add(new Tuple<int, int>(f.Indices[1], f.TextureIndices[1]));
                unique2.Add(new Tuple<int, int>(f.Indices[2], f.TextureIndices[2]));
            }

            unique.Add(new Tuple<int, int>(1, 1));
        }
    }
}
