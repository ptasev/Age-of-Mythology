using AoMEngineLibrary.Graphics.Dds;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using GltfMeshBuilder = SharpGLTF.Geometry.MeshBuilder<
    SharpGLTF.Geometry.VertexTypes.VertexPositionNormal,
    SharpGLTF.Geometry.VertexTypes.VertexTexture1,
    SharpGLTF.Geometry.VertexTypes.VertexJoints4>;
using GltfVertexBuilder = SharpGLTF.Geometry.VertexBuilder<
    SharpGLTF.Geometry.VertexTypes.VertexPositionNormal,
    SharpGLTF.Geometry.VertexTypes.VertexTexture1,
    SharpGLTF.Geometry.VertexTypes.VertexJoints4>;

namespace AoMEngineLibrary.Graphics.Grn
{
    public class GrnGltfConverter
    {
        private const string TrackName = "Default";
        private static readonly byte[] blankImageData;
        private static readonly Matrix4x4 RotX90N;
        private static readonly Quaternion RotX90NQuat;

        static GrnGltfConverter()
        {
            RotX90N = Matrix4x4.CreateRotationX(MathF.PI * -0.5f);
            RotX90NQuat = Quaternion.CreateFromRotationMatrix(RotX90N);

            // Create 4x4 white texture;
            var ddsBlank = new DdsFile();
            ddsBlank.header.flags |= DdsHeader.Flags.DDSD_MIPMAPCOUNT | DdsHeader.Flags.DDSD_LINEARSIZE;
            ddsBlank.header.height = 4;
            ddsBlank.header.width = 4;
            ddsBlank.header.pitchOrLinearSize = 8;
            ddsBlank.header.depth = 1;
            ddsBlank.header.mipMapCount = 1;
            ddsBlank.header.ddspf.flags |= DdsPixelFormat.Flags.DDPF_FOURCC;
            ddsBlank.header.ddspf.fourCC = 827611204; // DXT1
            ddsBlank.bdata = new byte[8] { 0xFF, 0xFF, 0xFF, 0xFF, 0, 0, 0, 0 };
            using (var ms = new MemoryStream())
            {
                ddsBlank.Write(ms);
                ms.Flush();
                blankImageData = ms.ToArray();
            }
        }

        public GrnGltfConverter()
        {
        }

        public ModelRoot Convert(GrnFile grn, TextureManager textureManager)
        {
            var sceneBuilder = new SceneBuilder();

            var nodeBuilders = ConvertSkeleton(grn);

            if (grn.Meshes.Count > 0)
            {
                ConvertMeshes(grn, textureManager, sceneBuilder, nodeBuilders);
            }

            if (grn.Animation.Duration > 0)
            {
                ConvertAnimation(grn, nodeBuilders);
            }

            return sceneBuilder.ToGltf2();
        }

        private NodeBuilder?[] ConvertSkeleton(GrnFile grn)
        {
            var mb = CreateBoneMesh();
            var rootNodeBuilder = new NodeBuilder("__GrnGltfContainer");
            var nodeBuilders = new NodeBuilder?[grn.Bones.Count];
            nodeBuilders[0] = rootNodeBuilder;

            // The parent index will always be higher than the child index
            for (int i = 0; i < grn.Bones.Count; ++i)
            {
                GrnBone bone = grn.Bones[i];
                if (bone.Name == "__Root")
                    continue;

                // Skip bones with the same name as the mesh since the mesh will create its own node
                if (!grn.Meshes.Exists(m => m.Name == bone.Name))
                {
                    var parent = nodeBuilders[bone.ParentIndex] ?? throw new InvalidDataException($"The parent of bone {bone.Name} cannot be null.");
                    NodeBuilder node = parent.CreateNode(bone.Name);
                    ConvertBone(bone, node);
                    nodeBuilders[i] = node;

                    // No longer add mesh to bone nodes since Blender will replace them with its bone mesh
                    //sceneBuilder.AddRigidMesh(mb, node);
                }
            }

            return nodeBuilders;
        }
        private MeshBuilder<VertexPosition, VertexColor1> CreateBoneMesh()
        {
            var mb = new MeshBuilder<VertexPosition, VertexColor1>("attachpointMesh");
            var pb = mb.UsePrimitive(MaterialBuilder.CreateDefault(), 3);

            // AoM dummy axes and colors
            //AddAxisToMesh(pb, new Vector3(-0.5f, 0.005f, 0.005f), new Vector4(0, 0, 1, 1));
            //AddAxisToMesh(pb, new Vector3(0.005f, 0.005f, 0.5f), new Vector4(0, 1, 0, 1));
            //AddAxisToMesh(pb, new Vector3(0.005f, -0.5f, 0.005f), new Vector4(1, 0, 0, 1));
            // Blender/3ds Max axes and colors
            AddAxisToMesh(pb, new Vector3(0.25f, 0.005f, 0.005f), new Vector4(1, 0, 0, 1));
            AddAxisToMesh(pb, new Vector3(0.005f, 0.25f, 0.005f), new Vector4(0, 1, 0, 1));
            AddAxisToMesh(pb, new Vector3(0.005f, 0.005f, 0.25f), new Vector4(0, 0, 1, 1));

            return mb;

            void AddAxisToMesh(IPrimitiveBuilder pb, Vector3 length, Vector4 color)
            {
                var verts = new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(length.X, 0, 0),
                    new Vector3(length.X, length.Y, 0),
                    new Vector3(0, length.Y, 0),
                    new Vector3(0, length.Y, length.Z),
                    new Vector3(length.X, length.Y, length.Z),
                    new Vector3(length.X, 0, length.Z),
                    new Vector3(0, 0, length.Z)
                };

                var vbs = verts.Select(v =>
                {
                    var vb = new VertexBuilder<VertexPosition, VertexColor1, VertexEmpty>();
                    vb.Geometry.Position = v;
                    vb.Material.Color = color;
                    return vb;
                }).ToList();

                var faces = new List<int>[]
                {
                    new List<int>() { 0, 2, 1 },
                    new List<int>() { 0, 3, 2 },
                    new List<int>() { 2, 3, 4 },
                    new List<int>() { 2, 4, 5 },
                    new List<int>() { 1, 2, 5 },
                    new List<int>() { 1, 5, 6 },
                    new List<int>() { 0, 7, 4 },
                    new List<int>() { 0, 4, 3 },
                    new List<int>() { 5, 4, 7 },
                    new List<int>() { 5, 7, 6 },
                    new List<int>() { 0, 6, 7 },
                    new List<int>() { 0, 1, 6 }
                };

                foreach (var face in faces)
                {
                    pb.AddTriangle(vbs[face[0]], vbs[face[1]], vbs[face[2]]);
                }
            }
        }
        private void ConvertBone(GrnBone bone, NodeBuilder nodeBuilder)
        {
            // Grn uses the Blender/3ds Max right handed coord system where X+ left, Y+ back, Z+ up
            // this is different from gltf so let's adjust
            bool adjustCoordSystem = bone.ParentIndex == 0;

            nodeBuilder.UseTranslation().Value = adjustCoordSystem ? Vector3.Transform(bone.Position, RotX90N) : bone.Position;
            nodeBuilder.UseRotation().Value = adjustCoordSystem ? Quaternion.Concatenate(bone.Rotation, RotX90NQuat) : bone.Rotation;
            nodeBuilder.UseScale().Value = new Vector3(bone.Scale.M11, bone.Scale.M22, bone.Scale.M33);
        }

        private void ConvertMeshes(GrnFile grn, TextureManager textureManager, SceneBuilder sceneBuilder, NodeBuilder?[] nodeBuilders)
        {
            var matIdMatBuilderMap = ConvertMaterials(grn, textureManager);

            for (int i = 0; i < grn.Meshes.Count; i++)
            {
                var mesh = grn.Meshes[i];
                var mb = ConvertMesh(mesh, matIdMatBuilderMap);
                //var nb = nodeBuilders.First(nb => nb.Name == mesh.Name) ?? throw new InvalidDataException($"Could not find node for mesh {mesh.Name}.");

                // Create the joint array with inverse bind matrix, and add mesh to scene
                var joints = mesh.BoneBindings.Select(bb =>
                {
                    var nb = nodeBuilders[bb.BoneIndex];
                    if (nb == null) throw new InvalidDataException($"A mesh ({mesh.Name}) skin cannot reference a null bone.");
                    return (nb, nb.GetInverseBindMatrix(Matrix4x4.Identity));
                }).ToArray();
                var inst = sceneBuilder.AddSkinnedMesh(mb, mb.Name, joints);
            }
        }
        private GltfMeshBuilder ConvertMesh(GrnMesh mesh, Dictionary<int, MaterialBuilder> matIdMatBuilderMap)
        {
            var primitives = from face in mesh.Faces
                             group face by face.MaterialIndex into faceGroup
                             select (Material: matIdMatBuilderMap[faceGroup.Key], Faces: faceGroup.ToList());

            var mb = new GltfMeshBuilder(mesh.Name);

            foreach (var prim in primitives)
            {
                var pb = mb.UsePrimitive(prim.Material);

                foreach (var face in prim.Faces)
                {
                    pb.AddTriangle(GetVertexBuilder(mesh, face, 0), GetVertexBuilder(mesh, face, 1), GetVertexBuilder(mesh, face, 2));
                }
            }

            return mb;
        }
        private GltfVertexBuilder GetVertexBuilder(GrnMesh mesh, GrnFace face, int index)
        {
            var vb = new GltfVertexBuilder();

            var vert = mesh.Vertices[face.Indices[index]];
            var norm = mesh.Normals[face.NormalIndices[index]];

            vb.Geometry = new VertexPositionNormal(Vector3.Transform(vert, RotX90N), Vector3.TransformNormal(norm, RotX90N));
            vb.Material.TexCoord = new Vector2(mesh.TextureCoordinates[face.TextureIndices[index]].X, 1 - mesh.TextureCoordinates[face.TextureIndices[index]].Y);

            var vertWeight = mesh.VertexWeights[face.Indices[index]];
            var vws = vertWeight.BoneIndices.Zip(vertWeight.Weights, (First, Second) => (First, Second)).Where(vw => vw.Second > 0).ToArray();
            if (vws.Length > 4) throw new NotSupportedException("A vertex cannot be bound to more than 4 bones.");
            vb.Skinning.SetWeights(SparseWeight8.Create(vws));

            return vb;
        }

        private Dictionary<int, MaterialBuilder> ConvertMaterials(GrnFile grn, TextureManager textureManager)
        {
            var textureImageMap = ConvertTextures(grn, textureManager);

            var matIdMatBuilderMap = new Dictionary<int, MaterialBuilder>();
            for (int i = 0; i < grn.Materials.Count; i++)
            {
                var mat = grn.Materials[i];
                var mb = new MaterialBuilder(mat.Name);
                mb.WithMetallicRoughnessShader();
                var cb = mb.UseChannel(KnownChannel.MetallicRoughness);
                cb.Parameter = new Vector4(0.2f, 0.5f, 0, 0);
                cb = mb.UseChannel(KnownChannel.BaseColor);

                if (textureImageMap.ContainsKey(mat.DiffuseTexture))
                {
                    MemoryImage memImage = textureImageMap[mat.DiffuseTexture];

                    var tb = cb.UseTexture();
                    tb.WrapS = TextureWrapMode.REPEAT;
                    tb.WrapT = TextureWrapMode.REPEAT;
                    tb.WithPrimaryImage(memImage);
                }

                matIdMatBuilderMap.Add(i, mb);
            }

            return matIdMatBuilderMap;
        }

        private Dictionary<GrnTexture, MemoryImage> ConvertTextures(GrnFile grn, TextureManager textureManager)
        {
            var textureImageMap = new Dictionary<GrnTexture, MemoryImage>();
            for (int i = 0; i < grn.Textures.Count; ++i)
            {
                var texture = grn.Textures[i];
                if (!string.IsNullOrWhiteSpace(texture.FileName))
                {
                    // Remove everything after first parenthesis
                    string imageFile = Path.GetFileName(texture.FileName);
                    int parenthIndex = imageFile.IndexOf('(');
                    imageFile = imageFile.Remove(parenthIndex);

                    // Create a memory image
                    MemoryImage memImage;
                    try
                    {
                        var filePath = textureManager.GetTexturePath(imageFile);
                        var images = textureManager.GetTexture(filePath);
                        using (var ms = new MemoryStream())
                        {
                            images[0][0].SaveAsPng(ms);
                            memImage = new MemoryImage(ms.ToArray());
                        }
                    }
                    catch
                    {
                        // TODO: log exception

                        // Write minimal image
                        var image = new SixLabors.ImageSharp.Image<L8>(1, 1, new L8(128));
                        using (var ms = new MemoryStream())
                        {
                            image.SaveAsPng(ms);
                            memImage = new MemoryImage(ms.ToArray());
                        }
                    }

                    textureImageMap.Add(texture, memImage);
                }
            }

            return textureImageMap;
        }

        private void ConvertAnimation(GrnFile grn, NodeBuilder?[] nodeBuilders)
        {
            for (int i = 0; i < grn.Animation.BoneTracks.Count; ++i)
            {
                GrnBone gbone = grn.Bones[i];
                bool adjustCoordSystem = gbone.ParentIndex == 0;
                if (gbone.Name == "__Root")
                {
                    continue;
                }

                GrnBoneTrack bone = grn.Animation.BoneTracks[i];
                NodeBuilder? node = nodeBuilders[i];
                if (node == null) continue;

                if (bone.PositionKeys.Count > 0)
                {
                    var tb = node.UseTranslation(TrackName);
                    for (int j = 0; j < bone.PositionKeys.Count; ++j)
                    {
                        tb.SetPoint(bone.PositionKeys[j], adjustCoordSystem ? Vector3.Transform(bone.Positions[j], RotX90N) : bone.Positions[j]);
                    }
                }

                if (bone.RotationKeys.Count > 0)
                {
                    var tb = node.UseRotation(TrackName);
                    for (int j = 0; j < bone.RotationKeys.Count; ++j)
                    {
                        tb.SetPoint(bone.RotationKeys[j], adjustCoordSystem ? Quaternion.Concatenate(bone.Rotations[j], RotX90NQuat) : bone.Rotations[j]);
                    }
                }

                if (bone.ScaleKeys.Count > 0)
                {
                    var tb = node.UseScale(TrackName);
                    for (int j = 0; j < bone.ScaleKeys.Count; ++j)
                    {
                        var scaleVec = new Vector3(bone.Scales[j].M11, bone.Scales[j].M22, bone.Scales[j].M33);
                        tb.SetPoint(bone.ScaleKeys[j], scaleVec);
                    }
                }
            }
        }
    }
}
