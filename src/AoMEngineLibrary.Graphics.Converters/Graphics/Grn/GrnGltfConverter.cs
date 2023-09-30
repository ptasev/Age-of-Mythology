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
        private static readonly Matrix4x4 RotX90N;
        private static readonly Quaternion RotX90NQuat;

        static GrnGltfConverter()
        {
            RotX90N = Matrix4x4.CreateRotationX(MathF.PI * -0.5f);
            RotX90NQuat = Quaternion.CreateFromRotationMatrix(RotX90N);
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

        private static NodeBuilder?[] ConvertSkeleton(GrnFile grn)
        {
            var nodeBuilders = new NodeBuilder?[grn.Bones.Count];

            // The parent index will always be higher than the child index
            for (int i = 0; i < grn.Bones.Count; ++i)
            {
                GrnBone bone = grn.Bones[i];
                if (bone.Name == "__Root")
                    continue;

                // Skip bones with the same name as the mesh since the mesh will create its own node
                if (!grn.Meshes.Exists(m => m.Name == bone.Name))
                {
                    NodeBuilder node;
                    if (bone.ParentIndex == 0)
                    {
                        // bones that have root as parent now become top-level nodes
                        node = new NodeBuilder(bone.Name);
                    }
                    else
                    {
                        var parent = nodeBuilders[bone.ParentIndex] ?? throw new InvalidDataException($"The parent of bone {bone.Name} cannot be null.");
                        node = parent.CreateNode(bone.Name);
                    }
                    
                    ConvertBone(bone, node);
                    nodeBuilders[i] = node;
                }
            }

            return nodeBuilders;
        }
        
        private static void ConvertBone(GrnBone bone, NodeBuilder nodeBuilder)
        {
            // Grn uses the Blender/3ds Max right handed coord system where X+ left, Y+ back, Z+ up
            // this is different from gltf so let's adjust
            bool adjustCoordSystem = bone.ParentIndex == 0;

            nodeBuilder.UseTranslation().Value = adjustCoordSystem ? Vector3.Transform(bone.Position, RotX90N) : bone.Position;
            nodeBuilder.UseRotation().Value = adjustCoordSystem ? Quaternion.Concatenate(bone.Rotation, RotX90NQuat) : bone.Rotation;
            nodeBuilder.UseScale().Value = new Vector3(bone.Scale.M11, bone.Scale.M22, bone.Scale.M33);
        }

        private static void ConvertMeshes(GrnFile grn, TextureManager textureManager, SceneBuilder sceneBuilder,
            IReadOnlyList<NodeBuilder?> nodeBuilders)
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
                    if (nb == null)
                        throw new InvalidDataException($"A mesh ({mesh.Name}) skin cannot reference a null bone.");
                    return (nb, nb.GetInverseBindMatrix(Matrix4x4.Identity));
                }).ToArray();
                var inst = sceneBuilder.AddSkinnedMesh(mb, joints).WithName(mb.Name);
            }
        }

        private static GltfMeshBuilder ConvertMesh(GrnMesh mesh, IReadOnlyDictionary<int, MaterialBuilder> matIdMatBuilderMap)
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
        
        private static GltfVertexBuilder GetVertexBuilder(GrnMesh mesh, GrnFace face, int index)
        {
            var vb = new GltfVertexBuilder();

            var vert = mesh.Vertices[face.Indices[index]];
            var norm = mesh.Normals[face.NormalIndices[index]];

            vb.Geometry = new VertexPositionNormal(Vector3.Transform(vert, RotX90N), Vector3.TransformNormal(norm, RotX90N));
            vb.Material.TexCoord = new Vector2(mesh.TextureCoordinates[face.TextureIndices[index]].X, mesh.TextureCoordinates[face.TextureIndices[index]].Y);

            var vertWeight = mesh.VertexWeights[face.Indices[index]];
            var vws = vertWeight.BoneIndices.Zip(vertWeight.Weights).Where(vw => vw.Second > 0).ToArray();
            if (vws.Length > 4) throw new NotSupportedException("A vertex cannot be bound to more than 4 bones.");
            vb.Skinning.SetBindings(SparseWeight8.Create(vws));

            return vb;
        }

        private static Dictionary<int, MaterialBuilder> ConvertMaterials(GrnFile grn, TextureManager textureManager)
        {
            var textureImageMap = ConvertTextures(grn, textureManager);

            var matIdMatBuilderMap = new Dictionary<int, MaterialBuilder>();
            for (int i = 0; i < grn.Materials.Count; i++)
            {
                var mat = grn.Materials[i];
                var mb = new MaterialBuilder(mat.Name);
                mb.WithMetallicRoughnessShader();
                var cb = mb.UseChannel(KnownChannel.MetallicRoughness);
                cb.Parameters[KnownProperty.MetallicFactor] = 0.1f;
                cb.Parameters[KnownProperty.RoughnessFactor] = 0.5f;
                cb = mb.UseChannel(KnownChannel.BaseColor);
                cb.Parameters[KnownProperty.RGBA] = new Vector4(0.5f, 0.5f, 0.5f, 1);

                Texture? tex = null;
                if (textureImageMap.ContainsKey(mat.DiffuseTexture))
                {
                    var texData = textureImageMap[mat.DiffuseTexture];
                    var imageBuilder = texData.Image;
                    tex = texData.Texture;

                    var tb = cb.UseTexture();
                    tb.Name = imageBuilder.Name;
                    tb.WrapS = TextureWrapMode.REPEAT;
                    tb.WrapT = TextureWrapMode.REPEAT;
                    tb.WithPrimaryImage(imageBuilder);
                }

                if (mat.Name.Contains("xform", StringComparison.InvariantCultureIgnoreCase) ||
                    Path.GetFileName(mat.DiffuseTexture.FileName).Contains("xform", StringComparison.InvariantCultureIgnoreCase) ||
                    !(tex?.AlphaTest ?? true))
                {
                    mb.WithAlpha(SharpGLTF.Materials.AlphaMode.OPAQUE);
                }
                else
                {
                    mb.WithAlpha(SharpGLTF.Materials.AlphaMode.MASK);
                }

                matIdMatBuilderMap.Add(i, mb);
            }

            return matIdMatBuilderMap;
        }

        private static Dictionary<GrnTexture, (ImageBuilder Image, Texture? Texture)> ConvertTextures(GrnFile grn,
            TextureManager textureManager)
        {
            var textureImageMap = new Dictionary<GrnTexture, (ImageBuilder Image, Texture? Texture)>();
            for (int i = 0; i < grn.Textures.Count; ++i)
            {
                var texture = grn.Textures[i];
                if (!string.IsNullOrWhiteSpace(texture.FileName) && !textureImageMap.ContainsKey(texture))
                {
                    // Remove everything after first parenthesis
                    string imageFile = Path.GetFileName(texture.FileName);
                    int parenthIndex = imageFile.IndexOf('(');
                    if (parenthIndex >= 0)
                        imageFile = imageFile.Remove(parenthIndex);

                    // Create a memory image
                    ImageBuilder imageBuilder;
                    Texture? tex = null;
                    try
                    {
                        var filePath = textureManager.GetTexturePath(imageFile);
                        tex = textureManager.GetTexture(filePath);
                        using (var ms = new MemoryStream())
                        {
                            tex.Images[0][0].SaveAsPng(ms);
                            var memImage = new MemoryImage(ms.ToArray());
                            imageBuilder = ImageBuilder.From(memImage, texture.Name);
                        }
                    }
                    catch
                    {
                        // TODO: log exception

                        // Write minimal image
                        var image = new Image<L8>(1, 1, new L8(128));
                        using (var ms = new MemoryStream())
                        {
                            image.SaveAsPng(ms);
                            var memImage = new MemoryImage(ms.ToArray());
                            imageBuilder = ImageBuilder.From(memImage, texture.Name);
                        }
                    }

                    textureImageMap.Add(texture, (imageBuilder, tex));
                }
            }

            return textureImageMap;
        }

        private static void ConvertAnimation(GrnFile grn, IReadOnlyList<NodeBuilder?> nodeBuilders)
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
