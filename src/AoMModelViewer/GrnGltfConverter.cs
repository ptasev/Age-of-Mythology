using AoMEngineLibrary.Graphics.Dds;
using AoMEngineLibrary.Graphics.Grn;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Controls;
using GltfMeshBuilder = SharpGLTF.Geometry.MeshBuilder<
    SharpGLTF.Geometry.VertexTypes.VertexPositionNormal,
    SharpGLTF.Geometry.VertexTypes.VertexTexture1,
    SharpGLTF.Geometry.VertexTypes.VertexJoints4>;
using GltfVertexBuilder = SharpGLTF.Geometry.VertexBuilder<
    SharpGLTF.Geometry.VertexTypes.VertexPositionNormal,
    SharpGLTF.Geometry.VertexTypes.VertexTexture1,
    SharpGLTF.Geometry.VertexTypes.VertexJoints4>;

namespace AoMModelViewer
{
    public class GrnGltfConverter
    {
        private const string TrackName = "Default";
        private static readonly byte[] blankImageData;

        static GrnGltfConverter()
        {
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

        public ModelRoot Convert(GrnFile grn)
        {
            var sceneBuilder = new SceneBuilder();
            var rootNodeBuilder = new NodeBuilder("root");

            // Grn uses the Blender/3ds Max right handed coord system where X+ left, Y+ back, Z+ up
            // this is different from gltf so let's adjust
            rootNodeBuilder.UseRotation().Value = Quaternion.CreateFromRotationMatrix(Matrix4x4.CreateRotationX(MathF.PI * -0.5f));

            var nodeBuilders = ConvertSkeleton(grn, sceneBuilder, rootNodeBuilder);

            if (grn.Meshes.Count > 0)
            {
                ConvertMeshes(grn, sceneBuilder, nodeBuilders);
            }

            if (grn.Animation.Duration > 0)
            {
                ConvertAnimation(grn, nodeBuilders);
            }

            return sceneBuilder.ToGltf2();
        }

        private NodeBuilder?[] ConvertSkeleton(GrnFile grn, SceneBuilder sceneBuilder, NodeBuilder nodeBuilder)
        {
            var mb = CreateBoneMesh();

            var nodeBuilders = new NodeBuilder?[grn.Bones.Count];
            nodeBuilders[0] = nodeBuilder;
            // The parent index will always be higher than the child index
            for (int i = 0; i < grn.Bones.Count; ++i)
            {
                GrnBone bone = grn.Bones[i];
                if (bone.Name == "__Root")
                    continue;

                if (!grn.Meshes.Exists(m => m.Name == bone.Name))
                {
                    var parent = nodeBuilders[bone.ParentIndex] ?? throw new InvalidDataException($"The parent of bone {bone.Name} cannot be null.");
                    var node = ConvertBone(bone, parent);
                    nodeBuilders[i] = node;
                    sceneBuilder.AddRigidMesh(mb, node);
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
        private NodeBuilder ConvertBone(GrnBone bone, NodeBuilder nodeBuilder)
        {
            var nb = nodeBuilder.CreateNode(bone.Name);

            var localMatrix = GetBoneLocalMatrix(bone);
            nb.LocalMatrix = localMatrix;

            return nb;
        }
        private Matrix4x4 GetBoneLocalMatrix(GrnBone bone)
        {
            var transMat = Matrix4x4.CreateTranslation(bone.Position);
            var rotMat = Matrix4x4.CreateFromQuaternion(bone.Rotation);
            var scaleMat = Matrix4x4.CreateScale(bone.Scale.M11, bone.Scale.M22, bone.Scale.M33);
            var res = scaleMat * rotMat * transMat;

            return res;
        }

        private void ConvertMeshes(GrnFile grn, SceneBuilder sceneBuilder, NodeBuilder?[] nodeBuilders)
        {
            var matIdMatBuilderMap = ConvertMaterials(grn);

            for (int i = 0; i < grn.Meshes.Count; i++)
            {
                var mesh = grn.Meshes[i];
                var mb = ConvertMesh(mesh, matIdMatBuilderMap);
                //var nb = nodeBuilders.First(nb => nb.Name == mesh.Name) ?? throw new InvalidDataException($"Could not find node for mesh {mesh.Name}.");

                var joints = mesh.BoneBindings.Select(bb => nodeBuilders[bb.BoneIndex]).ToArray();
                if (joints.Any(j => j == null)) throw new InvalidDataException($"A mesh ({mesh.Name}) skin cannot reference a null bone.");
                var inst = sceneBuilder.AddSkinnedMesh(mb, Matrix4x4.Identity, joints);
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

            vb.Geometry = new VertexPositionNormal(vert, norm);
            vb.Material.TexCoord = new Vector2(mesh.TextureCoordinates[face.TextureIndices[index]].X, 1 - mesh.TextureCoordinates[face.TextureIndices[index]].Y);

            var vertWeight = mesh.VertexWeights[face.Indices[index]];
            var vws = vertWeight.BoneIndices.Zip(vertWeight.Weights).Where(vw => vw.Second > 0).ToArray();
            if (vws.Length > 4) throw new NotSupportedException("A vertex cannot be bound to more than 4 bones.");
            vb.Skinning.SetWeights(SparseWeight8.Create(vws));

            return vb;
        }

        private Dictionary<int, MaterialBuilder> ConvertMaterials(GrnFile grn)
        {
            var matIdMatBuilderMap = new Dictionary<int, MaterialBuilder>();

            for (int i = 0; i < grn.Materials.Count; i++)
            {
                var mat = grn.Materials[i];
                var mb = new MaterialBuilder(mat.Name);
                mb.WithMetallicRoughnessShader();
                var cb = mb.UseChannel(KnownChannel.MetallicRoughness);
                cb.Parameter = new Vector4(0.2f, 0.5f, 0, 0);
                cb = mb.UseChannel(KnownChannel.BaseColor);

                if (!string.IsNullOrWhiteSpace(mat.DiffuseTexture.FileName))
                {
                    string imageFile = Path.GetFileName(mat.DiffuseTexture.FileName);
                    MemoryImage memImage;
                    if (File.Exists(imageFile))
                    {
                        memImage = new MemoryImage(File.ReadAllBytes(imageFile));
                    }
                    else
                    {
                        // Write minimal image
                        memImage = new MemoryImage(blankImageData);
                    }

                    var tb = cb.UseTexture();
                    tb.WrapS = TextureWrapMode.CLAMP_TO_EDGE;
                    tb.WrapT = TextureWrapMode.CLAMP_TO_EDGE;
                    tb.WithPrimaryImage(memImage);
                }

                matIdMatBuilderMap.Add(i, mb);
            }

            return matIdMatBuilderMap;
        }

        private void ConvertAnimation(GrnFile grn, NodeBuilder?[] nodeBuilders)
        {
            for (int i = 0; i < grn.Animation.BoneTracks.Count; ++i)
            {
                if (grn.Bones[i].Name == "__Root")
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
                        tb.SetPoint(bone.PositionKeys[j], bone.Positions[j]);
                    }
                }

                if (bone.RotationKeys.Count > 0)
                {
                    var tb = node.UseRotation(TrackName);
                    for (int j = 0; j < bone.RotationKeys.Count; ++j)
                    {
                        tb.SetPoint(bone.RotationKeys[j], bone.Rotations[j]);
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
