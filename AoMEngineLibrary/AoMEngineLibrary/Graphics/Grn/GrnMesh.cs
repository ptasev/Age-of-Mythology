namespace AoMEngineLibrary.Graphics.Grn
{
    using AoMEngineLibrary.Graphics.Grn.Nodes;
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

        public void Read(GrnNode meshNode)
        {
            this.Vertices = meshNode.FindNode<GrnMeshVerticesNode>(GrnNodeType.MeshVertices).Vertices;
            this.Normals = meshNode.FindNode<GrnMeshNormalsNode>(GrnNodeType.MeshNormals).Normals;
            this.TextureCoordinates = meshNode.FindNode<GrnMeshFieldNode>(GrnNodeType.MeshField).TextureCoordinates;
            this.VertexWeights = meshNode.FindNode<GrnMeshWeightsNode>(GrnNodeType.MeshWeights).VertexWeights;
            this.Faces = meshNode.FindNode<GrnMeshTrianglesNode>(GrnNodeType.MeshTriangles).Faces;
            this.DataExtensionIndex = meshNode.FindNode<GrnDataExtensionReferenceNode>(GrnNodeType.DataExtensionReference).DataExtensionIndex - 1;
        }
        public void Write(GrnNode meshSecNode)
        {
            GrnNode meshNode = new GrnNode(meshSecNode, GrnNodeType.Mesh);
            meshSecNode.AppendChild(meshNode);
            GrnNode meshVertSetSecNode = new GrnNode(meshNode, GrnNodeType.MeshVertexSetSection);
            meshNode.AppendChild(meshVertSetSecNode);
            GrnNode meshVertSetNode = new GrnNode(meshVertSetSecNode, GrnNodeType.MeshVertexSet);
            meshVertSetSecNode.AppendChild(meshVertSetNode);

            GrnMeshVerticesNode mVertNode = new GrnMeshVerticesNode(meshVertSetNode);
            mVertNode.Vertices = this.Vertices;
            meshVertSetNode.AppendChild(mVertNode);
            GrnMeshNormalsNode mNormNode = new GrnMeshNormalsNode(meshVertSetNode);
            mNormNode.Normals = this.Normals;
            meshVertSetNode.AppendChild(mNormNode);
            GrnNode meshFieldSecNode = new GrnNode(meshVertSetNode, GrnNodeType.MeshFieldSection);
            meshVertSetNode.AppendChild(meshFieldSecNode);
            GrnMeshFieldNode mFieldNode = new GrnMeshFieldNode(meshFieldSecNode);
            mFieldNode.TextureCoordinates = this.TextureCoordinates;
            meshFieldSecNode.AppendChild(mFieldNode);

            GrnMeshWeightsNode mWeightNode = new GrnMeshWeightsNode(meshNode);
            mWeightNode.VertexWeights = this.VertexWeights;
            meshNode.AppendChild(mWeightNode);
            GrnMeshTrianglesNode mTriNode = new GrnMeshTrianglesNode(meshNode);
            mTriNode.Faces = this.Faces;
            meshNode.AppendChild(mTriNode);
            GrnDataExtensionReferenceNode mDERefNode = new GrnDataExtensionReferenceNode(meshNode);
            mDERefNode.DataExtensionIndex = this.DataExtensionIndex + 1;
            meshNode.AppendChild(mDERefNode);
        }
        public void ReadFormMeshBones(List<GrnFormMeshBoneNode> formMeshBones)
        {
            this.BoneBindings = new List<GrnBoneBinding>(formMeshBones.Count);
            for (int i = 0; i < formMeshBones.Count; ++i)
            {
                this.BoneBindings.Add(new GrnBoneBinding());
                this.BoneBindings[i].BoneIndex = formMeshBones[i].BoneIndex;
                this.BoneBindings[i].Unknown = formMeshBones[i].Unknown;
                this.BoneBindings[i].OBBMin = formMeshBones[i].OBBMin;
                this.BoneBindings[i].OBBMax = formMeshBones[i].OBBMax;
            }
        }
        public void WriteFormMeshBones(GrnNode fMeshBoneSecNode)
        {
            for (int i = 0; i < this.BoneBindings.Count; ++i)
            {
                GrnFormMeshBoneNode fMeshBoneNode = new GrnFormMeshBoneNode(fMeshBoneSecNode);
                fMeshBoneNode.BoneIndex = this.BoneBindings[i].BoneIndex;
                fMeshBoneNode.Unknown = this.BoneBindings[i].Unknown;
                fMeshBoneNode.OBBMin = this.BoneBindings[i].OBBMin;
                fMeshBoneNode.OBBMax = this.BoneBindings[i].OBBMax;
                fMeshBoneSecNode.AppendChild(fMeshBoneNode);
            }
        }
        public void ReadRenderPassTriangles(int matIndex, GrnRenderPassTrianglesNode renderPassTriangles)
        {
            foreach (KeyValuePair<int, List<int>> rendPassTri in renderPassTriangles.TextureIndices)
            {
                this.Faces[rendPassTri.Key].TextureIndices = rendPassTri.Value;
                this.Faces[rendPassTri.Key].MaterialIndex = (Int16)matIndex;
            }
        }
        public void WriteRenderPass(GrnNode rendPassSecNode, int meshIndex)
        {
            Dictionary<int, List<int>> matFaceMap = new Dictionary<int, List<int>>();
            for (int i = 0; i < this.Faces.Count; ++i)
            {
                if (!matFaceMap.ContainsKey(this.Faces[i].MaterialIndex))
                {
                    matFaceMap.Add(this.Faces[i].MaterialIndex, new List<int>());
                }
                matFaceMap[this.Faces[i].MaterialIndex].Add(i);
            }
            foreach (KeyValuePair<int, List<int>> matFace in matFaceMap)
            {
                GrnRenderPassNode rendPassNode = new GrnRenderPassNode(rendPassSecNode);
                rendPassNode.FormMeshIndex = meshIndex;
                rendPassNode.MaterialIndex = matFace.Key + 1;
                rendPassSecNode.AppendChild(rendPassNode);

                GrnNode rpFieldSecNode = new GrnNode(rendPassNode, GrnNodeType.RenderPassFieldSection);
                rendPassNode.AppendChild(rpFieldSecNode);
                GrnNode rpFieldConstNode = new GrnNode(rpFieldSecNode, GrnNodeType.RenderPassFieldConstant);
                rpFieldConstNode.Data = new byte[] { 0x00, 0x00, 0x80, 0x3F,
                    0x00, 0x00, 0x80, 0x3F,
                    0x00, 0x00, 0x80, 0x3F };
                rpFieldSecNode.AppendChild(rpFieldConstNode);
                GrnNode rpFieldAssNode = new GrnNode(rpFieldSecNode, GrnNodeType.RenderPassFieldAssignment);
                rpFieldAssNode.Data = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                rpFieldSecNode.AppendChild(rpFieldAssNode);

                GrnRenderPassTrianglesNode rpTriNode = new GrnRenderPassTrianglesNode(rendPassNode);
                foreach (int faceIndex in matFace.Value)
                {
                    rpTriNode.TextureIndices.Add(faceIndex, this.Faces[faceIndex].TextureIndices);
                }
                rendPassNode.AppendChild(rpTriNode);
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
