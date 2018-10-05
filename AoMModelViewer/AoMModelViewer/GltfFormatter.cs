using AoMEngineLibrary.Graphics.Brg;
using AoMEngineLibrary.Graphics.Model;
using glTFLoader.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AoMModelViewer
{
    public class GltfFormatter
    {
        Gltf gltf;
        readonly List<Accessor> accessors;
        readonly List<BufferView> bufferViews;

        public GltfFormatter()
        {
            this.gltf = new Gltf();
            accessors = new List<Accessor>();
            bufferViews = new List<BufferView>();
        }

        public Gltf FromBrg(BrgFile brg, Stream bufferStream)
        {
            // TODO clear class fields

            gltf.Asset = new Asset();
            gltf.Asset.Version = "2.0";

            Scene scene = new Scene();
            scene.Nodes = new int[] { 0 };

            gltf.Scenes = new[] { scene }; 
            gltf.Scene = 0;
            
            Node node = new Node();
            node.Mesh = 0;
            node.Name = "node";

            gltf.Nodes = new[] { node };

            //FromBrgMesh(brg.Meshes[0]);

            // Create materials / textures
            Sampler sampler = new Sampler();
            sampler.MinFilter = Sampler.MinFilterEnum.LINEAR;
            sampler.MagFilter = Sampler.MagFilterEnum.LINEAR;
            sampler.WrapS = Sampler.WrapSEnum.CLAMP_TO_EDGE;
            sampler.WrapT = Sampler.WrapTEnum.CLAMP_TO_EDGE;
            gltf.Samplers = new Sampler[] { sampler };

            List<glTFLoader.Schema.Image> images = new List<Image>();
            List<glTFLoader.Schema.Texture> textures = new List<glTFLoader.Schema.Texture>();
            List<glTFLoader.Schema.Material> materials = new List<glTFLoader.Schema.Material>();
            Dictionary<int, int> brgMatGltfMatIndexMap = new Dictionary<int, int>();
            var uniqueTextures = from mat in brg.Materials
                                 group mat by mat.DiffuseMap into matGroup
                                 select matGroup;
            foreach (var matGroup in uniqueTextures)
            {
                int texIndex = textures.Count;
                bool hasTex = false;
                if (!string.IsNullOrWhiteSpace(matGroup.Key))
                {
                    Image im = new Image();
                    im.Uri = matGroup.Key + ".png";

                    glTFLoader.Schema.Texture tex = new glTFLoader.Schema.Texture();
                    tex.Source = images.Count;
                    tex.Sampler = 0;

                    images.Add(im);
                    textures.Add(tex);
                    hasTex = true;
                }

                foreach (var brgMat in brg.Materials)
                {
                    glTFLoader.Schema.Material mat = new glTFLoader.Schema.Material();
                    mat.PbrMetallicRoughness = new MaterialPbrMetallicRoughness();
                    mat.PbrMetallicRoughness.MetallicFactor = 0.2f;
                    mat.PbrMetallicRoughness.RoughnessFactor = 0.5f;

                    if (hasTex)
                    {
                        mat.PbrMetallicRoughness.BaseColorTexture = new TextureInfo();
                        mat.PbrMetallicRoughness.BaseColorTexture.Index = texIndex;
                    }
                    else
                    {
                        mat.PbrMetallicRoughness.BaseColorFactor =
                            new float[] { brgMat.DiffuseColor.R, brgMat.DiffuseColor.G, brgMat.DiffuseColor.B, brgMat.Opacity };
                    }

                    brgMatGltfMatIndexMap.Add(brgMat.Id, materials.Count);
                    materials.Add(mat);
                }
            }
            gltf.Images = images.ToArray();
            gltf.Textures = textures.ToArray();
            gltf.Materials = materials.ToArray();

            // Create primitives from first brg mesh
            // TODO: check if there is at least 1 mesh, and 1 face
            var primitives = (from face in brg.Meshes[0].Faces
                             group face by face.MaterialIndex into faceGroup
                             select new BrgMeshPrimitive(faceGroup.ToList(), brgMatGltfMatIndexMap[faceGroup.Key])).ToList();

            // Load mesh data
            glTFLoader.Schema.Mesh mesh = new glTFLoader.Schema.Mesh();
            mesh.Primitives = new MeshPrimitive[primitives.Count];
            for (int i = 0; i < mesh.Primitives.Length; ++i)
            {
                mesh.Primitives[i] = new MeshPrimitive();
            }
            for (int j = 0; j < primitives.Count; ++j)
            {
                primitives[j].Serialize(mesh.Primitives[j], brg.Meshes[0], this, bufferStream);
            }
            for (int i = 0; i < brg.Meshes[0].MeshAnimations.Count; ++i)
            {
                for (int j = 0; j < primitives.Count; ++j)
                {
                    primitives[j].Serialize(mesh.Primitives[j], (BrgMesh)brg.Meshes[0].MeshAnimations[i], this, bufferStream);
                }
            }
            gltf.Meshes = new[] { mesh };

            // Create Animation
            if (brg.Meshes[0].MeshAnimations.Count > 0)
            {
                for (int i = 0; i < mesh.Primitives.Length; ++i)
                {
                    mesh.Primitives[i].Targets = primitives[i].Targets.ToArray();
                }

                mesh.Weights = new float[brg.Meshes[0].MeshAnimations.Count];

                gltf.Animations = new[] { CreateAnimation(brg.Animation, mesh.Weights.Length, bufferStream) };
            }

            // Create buffer stream
            gltf.BufferViews = bufferViews.ToArray();
            gltf.Accessors = accessors.ToArray();
            var buffer = new glTFLoader.Schema.Buffer();
            gltf.Buffers = new[] { buffer };
            buffer.ByteLength = (int)bufferStream.Length;
            buffer.Uri = "dataBuffer.bin";

            return gltf;
        }

        private glTFLoader.Schema.Animation CreateAnimation(AoMEngineLibrary.Graphics.Model.Animation animation, int weightCount, Stream bufferStream)
        {
            var sampler = new AnimationSampler();
            sampler.Interpolation = AnimationSampler.InterpolationEnum.LINEAR;

            CreateKeysBuffer(animation, bufferStream);
            sampler.Input = accessors.Count - 1;

            CreateWeightsBuffer(animation, weightCount, bufferStream);
            sampler.Output = accessors.Count - 1;

            var target = new AnimationChannelTarget();
            target.Node = 0;
            target.Path = AnimationChannelTarget.PathEnum.weights;

            var channel = new AnimationChannel();
            channel.Sampler = 0;
            channel.Target = target;

            var gltfAnimation = new glTFLoader.Schema.Animation();
            gltfAnimation.Samplers = new[] { sampler };
            gltfAnimation.Channels = new[] { channel };

            return gltfAnimation;
        }


        private void CreateKeysBuffer(AoMEngineLibrary.Graphics.Model.Animation animation, Stream bufferStream)
        {
            long bufferViewOffset;
            float min = float.MaxValue;
            float max = float.MinValue;
            using (BinaryWriter writer = new BinaryWriter(bufferStream, Encoding.UTF8, true))
            {
                // padding
                writer.Write(new byte[(-bufferStream.Length) & (PaddingBytes(Accessor.ComponentTypeEnum.FLOAT) - 1)]);
                bufferViewOffset = bufferStream.Length;

                for (int i = 0; i < animation.MeshKeys.Count; ++i)
                {
                    min = Math.Min(min, animation.MeshKeys[i]);
                    max = Math.Max(max, animation.MeshKeys[i]);

                    writer.Write(animation.MeshKeys[i]);
                }
            }

            BufferView posBufferView = new BufferView();
            posBufferView.Buffer = 0;
            posBufferView.ByteLength = animation.MeshKeys.Count * 4;
            posBufferView.ByteOffset = (int)bufferViewOffset;
            posBufferView.Name = "keysBufferView";

            Accessor posAccessor = new Accessor();
            posAccessor.BufferView = bufferViews.Count;
            posAccessor.ByteOffset = 0;
            posAccessor.ComponentType = Accessor.ComponentTypeEnum.FLOAT;
            posAccessor.Count = animation.MeshKeys.Count;
            posAccessor.Max = new[] { max };
            posAccessor.Min = new[] { min };
            posAccessor.Name = "keysBufferViewAccessor";
            posAccessor.Type = Accessor.TypeEnum.SCALAR;

            bufferViews.Add(posBufferView);
            accessors.Add(posAccessor);
        }
        private void CreateWeightsBuffer(AoMEngineLibrary.Graphics.Model.Animation animation, int weightCount, Stream bufferStream)
        {
            long bufferViewOffset;
            using (BinaryWriter writer = new BinaryWriter(bufferStream, Encoding.UTF8, true))
            {
                // padding
                writer.Write(new byte[(-bufferStream.Length) & (PaddingBytes(Accessor.ComponentTypeEnum.FLOAT) - 1)]);
                bufferViewOffset = bufferStream.Length;

                for (int i = 0; i < weightCount; ++i)
                {
                    writer.Write(0.0f);
                }

                for (int i = 1; i < animation.MeshKeys.Count; ++i)
                {
                    for (int j = 0; j < weightCount; ++j)
                    {
                        writer.Write(i == j + 1 ? 1.0f : 0.0f);
                    }
                }
            }

            BufferView posBufferView = new BufferView();
            posBufferView.Buffer = 0;
            posBufferView.ByteLength = animation.MeshKeys.Count * weightCount * 4;
            posBufferView.ByteOffset = (int)bufferViewOffset;
            posBufferView.Name = "weightsBufferView";

            Accessor posAccessor = new Accessor();
            posAccessor.BufferView = bufferViews.Count;
            posAccessor.ByteOffset = 0;
            posAccessor.ComponentType = Accessor.ComponentTypeEnum.FLOAT;
            posAccessor.Count = animation.MeshKeys.Count * weightCount;
            posAccessor.Max = new[] { 1.0f };
            posAccessor.Min = new[] { 0.0f };
            posAccessor.Name = "weightsBufferViewAccessor";
            posAccessor.Type = Accessor.TypeEnum.SCALAR;

            bufferViews.Add(posBufferView);
            accessors.Add(posAccessor);
        }

        private int PaddingBytes(Accessor.ComponentTypeEnum componentType)
        {
            switch (componentType)
            {
                case Accessor.ComponentTypeEnum.BYTE:
                case Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                    return 1;
                case Accessor.ComponentTypeEnum.SHORT:
                case Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                    return 2;
                case Accessor.ComponentTypeEnum.UNSIGNED_INT:
                case Accessor.ComponentTypeEnum.FLOAT:
                default:
                    return 4;
            }
        }

        private void FromBrgMesh(BrgMesh brgMesh)
        {
            Vector3 max = new Vector3(float.MinValue);
            Vector3 min = new Vector3(float.MaxValue);
            using (FileStream fs = File.Open("posBuffer.bin", FileMode.Create, FileAccess.Write, FileShare.Read))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                foreach (Vector3 vec in brgMesh.Vertices)
                {
                    max.X = Math.Max(max.X, vec.X);
                    max.Y = Math.Max(max.Y, vec.Y);
                    max.Z = Math.Max(max.Z, vec.Z);

                    min.X = Math.Min(min.X, vec.X);
                    min.Y = Math.Min(min.Y, vec.Y);
                    min.Z = Math.Min(min.Z, vec.Z);

                    writer.Write(vec.X);
                    writer.Write(vec.Y);
                    writer.Write(vec.Z);
                }
            }

            glTFLoader.Schema.Buffer posBuffer = new glTFLoader.Schema.Buffer();
            posBuffer.ByteLength = brgMesh.Vertices.Count * 12;
            posBuffer.Uri = "posBuffer.bin";

            BufferView posBufferView = new BufferView();
            posBufferView.Buffer = 0;
            posBufferView.ByteLength = posBuffer.ByteLength;
            posBufferView.ByteOffset = 0;
            posBufferView.ByteStride = 12;
            posBufferView.Name = "posBufferView";
            posBufferView.Target = BufferView.TargetEnum.ARRAY_BUFFER;

            Accessor posAccessor = new Accessor();
            posAccessor.BufferView = 0;
            posAccessor.ByteOffset = 0;
            posAccessor.ComponentType = Accessor.ComponentTypeEnum.FLOAT;
            posAccessor.Count = brgMesh.Vertices.Count;
            posAccessor.Max = new[] { max.X, max.Y, max.Z };
            posAccessor.Min = new[] { min.X, min.Y, min.Z };
            posAccessor.Name = "posBufferViewAccessor";
            posAccessor.Type = Accessor.TypeEnum.VEC3;

            short faceMin = short.MaxValue;
            short faceMax = short.MinValue;
            using (FileStream fs = File.Open("indexBuffer.bin", FileMode.Create, FileAccess.Write, FileShare.Read))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                foreach (var face in brgMesh.Faces)
                {
                    faceMin = Math.Min(faceMin, face.Indices[0]);
                    faceMin = Math.Min(faceMin, face.Indices[1]);
                    faceMin = Math.Min(faceMin, face.Indices[2]);

                    faceMax = Math.Max(faceMax, face.Indices[0]);
                    faceMax = Math.Max(faceMax, face.Indices[1]);
                    faceMax = Math.Max(faceMax, face.Indices[2]);

                    writer.Write(face.Indices[0]);
                    writer.Write(face.Indices[1]);
                    writer.Write(face.Indices[2]);
                }
            }

            glTFLoader.Schema.Buffer indexBuffer = new glTFLoader.Schema.Buffer();
            indexBuffer.ByteLength = brgMesh.Faces.Count * 6;
            indexBuffer.Uri = "indexBuffer.bin";

            BufferView indexBufferView = new BufferView();
            indexBufferView.Buffer = 1;
            indexBufferView.ByteLength = indexBuffer.ByteLength;
            indexBufferView.ByteOffset = 0;
            indexBufferView.Name = "indexBufferView";
            indexBufferView.Target = BufferView.TargetEnum.ELEMENT_ARRAY_BUFFER;

            Accessor indexAccessor = new Accessor();
            indexAccessor.BufferView = 1;
            indexAccessor.ByteOffset = 0;
            indexAccessor.ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
            indexAccessor.Count = brgMesh.Faces.Count * 3;
            indexAccessor.Max = new[] { (float)faceMax };
            indexAccessor.Min = new[] { (float)faceMin };
            indexAccessor.Name = "indexBufferViewAccessor";
            indexAccessor.Type = Accessor.TypeEnum.SCALAR;

            gltf.Buffers = new[] { posBuffer, indexBuffer };
            gltf.BufferViews = new[] { posBufferView, indexBufferView };
            gltf.Accessors = new[] { posAccessor, indexAccessor };

            MeshPrimitive meshPrimitive = new MeshPrimitive();
            meshPrimitive.Attributes = new Dictionary<string, int>();
            meshPrimitive.Attributes.Add("POSITION", 0);
            meshPrimitive.Indices = 1;
            meshPrimitive.Mode = MeshPrimitive.ModeEnum.TRIANGLES;

            var mesh = new glTFLoader.Schema.Mesh();
            mesh.Name = "mesh";
            mesh.Primitives = new[] { meshPrimitive };

            gltf.Meshes = new[] { mesh };

            Node node = new Node();
            node.Mesh = 0;
            node.Name = "node";

            gltf.Nodes = new[] { node };
        }

        private class BrgMeshPrimitive
        {
            public int MaterialIndex { get; }
            public List<Dictionary<string, int>> Targets { get; }
            public List<Face> Faces { get; }
            public List<short> Indices { get; }

            public BrgMeshPrimitive(List<Face> faces, int materialIndex)
            {
                Faces = faces;
                if (faces.Count > 0) MaterialIndex = materialIndex;
                Targets = new List<Dictionary<string, int>>();

                Dictionary<short, short>  mapNewIndices = new Dictionary<short, short>();
                Indices = new List<short>();
                foreach (Face face in Faces)
                {
                    for (int i = 0; i < face.Indices.Count; ++i)
                    {
                        short oldIndex = face.Indices[i];

                        if (mapNewIndices.ContainsKey(oldIndex))
                        {
                            face.Indices[i] = mapNewIndices[oldIndex];
                        }
                        else
                        {
                            short newIndex = (short)Indices.Count;
                            mapNewIndices.Add(oldIndex, newIndex);
                            face.Indices[i] = newIndex;
                            Indices.Add(oldIndex);
                        }
                    }
                }
            }

            public void Serialize(MeshPrimitive primitive, BrgMesh mesh, GltfFormatter formatter, Stream bufferStream)
            {
                if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                {
                    primitive.Mode = MeshPrimitive.ModeEnum.TRIANGLES;
                    CreateIndexBuffer(mesh, formatter, bufferStream);
                    primitive.Indices = formatter.accessors.Count - 1;
                    primitive.Material = MaterialIndex;

                    primitive.Attributes = CreateAttributes(mesh, formatter, bufferStream);
                }
                else
                {
                    Targets.Add(CreateAttributes(mesh, formatter, bufferStream));
                }
            }

            private Dictionary<string, int> CreateAttributes(BrgMesh mesh, GltfFormatter formatter, Stream bufferStream)
            {
                var attributes = new Dictionary<string, int>();

                CreatePositionBuffer(mesh, formatter, bufferStream);
                int posAccessor = formatter.accessors.Count - 1;
                attributes.Add("POSITION", posAccessor);

                CreateNormalBuffer(mesh, formatter, bufferStream);
                int normAccessor = formatter.accessors.Count - 1;
                attributes.Add("NORMAL", normAccessor);

                if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                {
                    CreateTexcoordBuffer(mesh, formatter, bufferStream);
                    int texcoordAccessor = formatter.accessors.Count - 1;
                    attributes.Add("TEXCOORD_0", texcoordAccessor);
                }

                return attributes;
            }
            private void CreateIndexBuffer(BrgMesh mesh, GltfFormatter formatter, Stream bufferStream)
            {
                long bufferViewOffset;
                short faceMin = short.MaxValue;
                short faceMax = short.MinValue;
                using (BinaryWriter writer = new BinaryWriter(bufferStream, Encoding.UTF8, true))
                {
                    // padding
                    writer.Write(new byte[(-bufferStream.Length) & (PaddingBytes(Accessor.ComponentTypeEnum.UNSIGNED_SHORT) - 1)]);
                    bufferViewOffset = bufferStream.Length;

                    foreach (var face in Faces)
                    {
                        faceMin = Math.Min(faceMin, face.Indices[0]);
                        faceMin = Math.Min(faceMin, face.Indices[1]);
                        faceMin = Math.Min(faceMin, face.Indices[2]);

                        faceMax = Math.Max(faceMax, face.Indices[0]);
                        faceMax = Math.Max(faceMax, face.Indices[1]);
                        faceMax = Math.Max(faceMax, face.Indices[2]);

                        writer.Write(face.Indices[0]);
                        writer.Write(face.Indices[1]);
                        writer.Write(face.Indices[2]);
                    }
                }
                
                BufferView indexBufferView = new BufferView();
                indexBufferView.Buffer = 0;
                indexBufferView.ByteLength = Faces.Count * 6;
                indexBufferView.ByteOffset = (int)bufferViewOffset;
                indexBufferView.Name = "indexBufferView";
                indexBufferView.Target = BufferView.TargetEnum.ELEMENT_ARRAY_BUFFER;

                Accessor indexAccessor = new Accessor();
                indexAccessor.BufferView = formatter.bufferViews.Count;
                indexAccessor.ByteOffset = 0;
                indexAccessor.ComponentType = Accessor.ComponentTypeEnum.UNSIGNED_SHORT;
                indexAccessor.Count = Faces.Count * 3;
                indexAccessor.Max = new[] { (float)faceMax };
                indexAccessor.Min = new[] { (float)faceMin };
                indexAccessor.Name = "indexBufferViewAccessor";
                indexAccessor.Type = Accessor.TypeEnum.SCALAR;

                formatter.bufferViews.Add(indexBufferView);
                formatter.accessors.Add(indexAccessor);
            }
            private void CreatePositionBuffer(BrgMesh mesh, GltfFormatter formatter, Stream bufferStream)
            {
                long bufferViewOffset;
                Vector3 max = new Vector3(float.MinValue);
                Vector3 min = new Vector3(float.MaxValue);
                using (BinaryWriter writer = new BinaryWriter(bufferStream, Encoding.UTF8, true))
                {
                    // padding
                    writer.Write(new byte[(-bufferStream.Length) & (PaddingBytes(Accessor.ComponentTypeEnum.FLOAT) - 1)]);
                    bufferViewOffset = bufferStream.Length;

                    foreach (int index in Indices)
                    {
                        Vector3 vec = mesh.Vertices[index];

                        max.X = Math.Max(max.X, vec.X);
                        max.Y = Math.Max(max.Y, vec.Y);
                        max.Z = Math.Max(max.Z, vec.Z);

                        min.X = Math.Min(min.X, vec.X);
                        min.Y = Math.Min(min.Y, vec.Y);
                        min.Z = Math.Min(min.Z, vec.Z);

                        writer.Write(vec.X);
                        writer.Write(vec.Y);
                        writer.Write(vec.Z);
                    }
                }

                BufferView posBufferView = new BufferView();
                posBufferView.Buffer = 0;
                posBufferView.ByteLength = Indices.Count * 12;
                posBufferView.ByteOffset = (int)bufferViewOffset;
                posBufferView.ByteStride = 12;
                posBufferView.Name = "posBufferView";
                posBufferView.Target = BufferView.TargetEnum.ARRAY_BUFFER;

                Accessor posAccessor = new Accessor();
                posAccessor.BufferView = formatter.bufferViews.Count;
                posAccessor.ByteOffset = 0;
                posAccessor.ComponentType = Accessor.ComponentTypeEnum.FLOAT;
                posAccessor.Count = Indices.Count;
                posAccessor.Max = new[] { max.X, max.Y, max.Z };
                posAccessor.Min = new[] { min.X, min.Y, min.Z };
                posAccessor.Name = "posBufferViewAccessor";
                posAccessor.Type = Accessor.TypeEnum.VEC3;

                formatter.bufferViews.Add(posBufferView);
                formatter.accessors.Add(posAccessor);
            }
            private void CreateNormalBuffer(BrgMesh mesh, GltfFormatter formatter, Stream bufferStream)
            {
                long bufferViewOffset;
                Vector3 max = new Vector3(float.MinValue);
                Vector3 min = new Vector3(float.MaxValue);
                using (BinaryWriter writer = new BinaryWriter(bufferStream, Encoding.UTF8, true))
                {
                    // padding
                    writer.Write(new byte[(-bufferStream.Length) & (PaddingBytes(Accessor.ComponentTypeEnum.FLOAT) - 1)]);
                    bufferViewOffset = bufferStream.Length;

                    foreach (int index in Indices)
                    {
                        Vector3 vec = mesh.Normals[index];
                        vec = vec / vec.Length();

                        max.X = Math.Max(max.X, vec.X);
                        max.Y = Math.Max(max.Y, vec.Y);
                        max.Z = Math.Max(max.Z, vec.Z);

                        min.X = Math.Min(min.X, vec.X);
                        min.Y = Math.Min(min.Y, vec.Y);
                        min.Z = Math.Min(min.Z, vec.Z);

                        writer.Write(vec.X);
                        writer.Write(vec.Y);
                        writer.Write(vec.Z);
                    }
                }

                BufferView posBufferView = new BufferView();
                posBufferView.Buffer = 0;
                posBufferView.ByteLength = Indices.Count * 12;
                posBufferView.ByteOffset = (int)bufferViewOffset;
                posBufferView.ByteStride = 12;
                posBufferView.Name = "normalBufferView";
                posBufferView.Target = BufferView.TargetEnum.ARRAY_BUFFER;

                Accessor posAccessor = new Accessor();
                posAccessor.BufferView = formatter.bufferViews.Count;
                posAccessor.ByteOffset = 0;
                posAccessor.ComponentType = Accessor.ComponentTypeEnum.FLOAT;
                posAccessor.Count = Indices.Count;
                posAccessor.Max = new[] { max.X, max.Y, max.Z };
                posAccessor.Min = new[] { min.X, min.Y, min.Z };
                posAccessor.Name = "normalBufferViewAccessor";
                posAccessor.Type = Accessor.TypeEnum.VEC3;

                formatter.bufferViews.Add(posBufferView);
                formatter.accessors.Add(posAccessor);
            }
            private void CreateTexcoordBuffer(BrgMesh mesh, GltfFormatter formatter, Stream bufferStream)
            {
                long bufferViewOffset;
                using (BinaryWriter writer = new BinaryWriter(bufferStream, Encoding.UTF8, true))
                {
                    // padding
                    writer.Write(new byte[(-bufferStream.Length) & (PaddingBytes(Accessor.ComponentTypeEnum.FLOAT) - 1)]);
                    bufferViewOffset = bufferStream.Length;

                    foreach (int index in Indices)
                    {
                        Vector3 vec = mesh.TextureCoordinates[index];

                        writer.Write(vec.X);
                        writer.Write(1.0f - vec.Y);
                    }
                }

                BufferView posBufferView = new BufferView();
                posBufferView.Buffer = 0;
                posBufferView.ByteLength = Indices.Count * 8;
                posBufferView.ByteOffset = (int)bufferViewOffset;
                posBufferView.ByteStride = 8;
                posBufferView.Name = "uvBufferView";
                posBufferView.Target = BufferView.TargetEnum.ARRAY_BUFFER;

                Accessor posAccessor = new Accessor();
                posAccessor.BufferView = formatter.bufferViews.Count;
                posAccessor.ByteOffset = 0;
                posAccessor.ComponentType = Accessor.ComponentTypeEnum.FLOAT;
                posAccessor.Count = Indices.Count;
                posAccessor.Name = "uvBufferViewAccessor";
                posAccessor.Type = Accessor.TypeEnum.VEC2;

                formatter.bufferViews.Add(posBufferView);
                formatter.accessors.Add(posAccessor);
            }

            private int PaddingBytes(Accessor.ComponentTypeEnum componentType)
            {
                switch (componentType)
                {
                    case Accessor.ComponentTypeEnum.BYTE:
                    case Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                        return 1;
                    case Accessor.ComponentTypeEnum.SHORT:
                    case Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                        return 2;
                    case Accessor.ComponentTypeEnum.UNSIGNED_INT:
                    case Accessor.ComponentTypeEnum.FLOAT:
                    default:
                        return 4;
                }
            }
        }
    }
}
