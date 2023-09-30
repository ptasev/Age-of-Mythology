using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
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
    SharpGLTF.Geometry.VertexTypes.VertexColor1Texture1,
    SharpGLTF.Geometry.VertexTypes.VertexEmpty>;
using GltfMeshBuilder2 = SharpGLTF.Geometry.MeshBuilder<
    SharpGLTF.Geometry.VertexTypes.VertexPositionNormal,
    SharpGLTF.Geometry.VertexTypes.VertexTexture1,
    SharpGLTF.Geometry.VertexTypes.VertexEmpty>;
using GltfMeshBuilder3 = SharpGLTF.Geometry.MeshBuilder<
    SharpGLTF.Geometry.VertexTypes.VertexPositionNormal,
    SharpGLTF.Geometry.VertexTypes.VertexColor1,
    SharpGLTF.Geometry.VertexTypes.VertexEmpty>;
using GltfMeshBuilder4 = SharpGLTF.Geometry.MeshBuilder<
    SharpGLTF.Geometry.VertexTypes.VertexPositionNormal,
    SharpGLTF.Geometry.VertexTypes.VertexEmpty,
    SharpGLTF.Geometry.VertexTypes.VertexEmpty>;
using GltfVertexBuilder = SharpGLTF.Geometry.VertexBuilder<
    SharpGLTF.Geometry.VertexTypes.VertexPositionNormal,
    SharpGLTF.Geometry.VertexTypes.VertexColor1Texture1,
    SharpGLTF.Geometry.VertexTypes.VertexEmpty>;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgGltfConverter
    {
        private const string TrackName = "Default";

        public BrgGltfConverter()
        {
        }

        public ModelRoot Convert(BrgFile brg, BrgGltfParameters parameters, TextureManager textureManager)
        {
            var sceneBuilder = new SceneBuilder();
            var rootNodeBuilder = new NodeBuilder($"{parameters.ModelName}Container");

            var matIdMatBuilderMap = new Dictionary<int, MaterialBuilder>();
            ConvertMaterials(brg, textureManager, matIdMatBuilderMap);

            ConvertMeshes(brg, parameters, sceneBuilder, rootNodeBuilder, matIdMatBuilderMap);

            ConvertDummies(brg, sceneBuilder, rootNodeBuilder);

            return sceneBuilder.ToGltf2();
        }

        private void ConvertDummies(BrgFile brg, SceneBuilder sceneBuilder, NodeBuilder nodeBuilder)
        {
            var mb = CreateDummyMesh();

            // Create hotspot
            var hnb = nodeBuilder.CreateNode("Dummy_hotspot");
            hnb.UseTranslation().Value = brg.Meshes[0].Header.HotspotPosition;
            if (brg.Meshes.Count > 1)
            {
                var ttb = hnb.UseTranslation().UseTrackBuilder(TrackName);
                ttb.SetPoint(brg.Animation.MeshKeys[0], brg.Meshes[0].Header.HotspotPosition);
                for (var i = 1; i < brg.Meshes.Count; ++i)
                {
                    ttb.SetPoint(brg.Animation.MeshKeys[i], brg.Meshes[i].Header.HotspotPosition);
                }
            }
            sceneBuilder.AddRigidMesh(mb, hnb);

            for (var i = 0; i < brg.Meshes[0].Dummies.Count; ++i)
            {
                var nb = ConvertDummy(brg, i, nodeBuilder);
                sceneBuilder.AddRigidMesh(mb, nb);
            }
        }
        
        private static MeshBuilder<VertexPosition, VertexColor1> CreateDummyMesh()
        {
            var mb = new MeshBuilder<VertexPosition, VertexColor1>("dummyMesh");
            var pb = mb.UsePrimitive(MaterialBuilder.CreateDefault(), 3);

            // Blender/3ds Max axes and colors
            AddAxisToMesh(pb, new Vector3(0.25f, 0.005f, 0.005f), new Vector4(1, 0, 0, 1));
            AddAxisToMesh(pb, new Vector3(0.005f, 0.005f, -0.25f), new Vector4(0, 1, 0, 1));
            AddAxisToMesh(pb, new Vector3(0.005f, 0.25f, 0.005f), new Vector4(0, 0, 1, 1));

            return mb;

            static void AddAxisToMesh(IPrimitiveBuilder pb, Vector3 length, Vector4 color)
            {
                var positions = new Vector3[]
                {
                    new(0, 0, 0),
                    new(length.X, 0, 0),
                    new(length.X, length.Y, 0),
                    new(0, length.Y, 0),
                    new(0, length.Y, length.Z),
                    new(length.X, length.Y, length.Z),
                    new(length.X, 0, length.Z),
                    new(0, 0, length.Z)
                };

                var vbs = positions.Select(v =>
                {
                    var vb = new VertexBuilder<VertexPosition, VertexColor1, VertexEmpty>();
                    vb.Geometry.Position = v;
                    vb.Material.Color = color;
                    return vb;
                }).ToList();

                var faces = new List<BrgFace>()
                {
                    new(0, 2, 1),
                    new(0, 3, 2),
                    new(2, 3, 4),
                    new(2, 4, 5),
                    new(1, 2, 5),
                    new(1, 5, 6),
                    new(0, 7, 4),
                    new(0, 4, 3),
                    new(5, 4, 7),
                    new(5, 7, 6),
                    new(0, 6, 7),
                    new(0, 1, 6)
                };

                foreach (var face in faces)
                {
                    pb.AddTriangle(vbs[face.A], vbs[face.B], vbs[face.C]);
                }
            }
        }
        
        private NodeBuilder ConvertDummy(BrgFile brg, int dummyIndex, NodeBuilder nodeBuilder)
        {
            var dummy = brg.Meshes[0].Dummies[dummyIndex];
            var nb = nodeBuilder.CreateNode("Dummy_" + dummy.Name);

            var worldMatrix = GetDummyWorldMatrix(dummy);
            Matrix4x4.Decompose(worldMatrix, out Vector3 scale, out Quaternion rot, out Vector3 trans);
            nb.UseTranslation().Value = trans;
            nb.UseRotation().Value = rot;
            nb.UseScale().Value = scale;

            // Only do the rest if animated
            if (brg.Meshes.Count > 1)
            {
                var ttb = nb.UseTranslation().UseTrackBuilder(TrackName);
                var rtb = nb.UseRotation().UseTrackBuilder(TrackName);
                var stb = nb.UseScale().UseTrackBuilder(TrackName);
                ttb.SetPoint(brg.Animation.MeshKeys[0], trans);
                rtb.SetPoint(brg.Animation.MeshKeys[0], rot);
                stb.SetPoint(brg.Animation.MeshKeys[0], scale);

                for (int i = 1; i < brg.Meshes.Count; ++i)
                {
                    dummy = brg.Meshes[i].Dummies[dummyIndex];
                    worldMatrix = GetDummyWorldMatrix(dummy);
                    Matrix4x4.Decompose(worldMatrix, out scale, out rot, out trans);
                    ttb.SetPoint(brg.Animation.MeshKeys[i], trans);
                    rtb.SetPoint(brg.Animation.MeshKeys[i], rot);
                    stb.SetPoint(brg.Animation.MeshKeys[i], scale);
                }
            }

            return nb;
        }
        
        private static Matrix4x4 GetDummyWorldMatrix(BrgDummy dummy)
        {
            var mat = new Matrix4x4
            {
                M11 = dummy.Right.X, M12 = dummy.Right.Y, M13 = dummy.Right.Z,
                M21 = dummy.Up.X, M22 = dummy.Up.Y, M23 = dummy.Up.Z,
                M31 = dummy.Forward.X, M32 = dummy.Forward.Y, M33 = dummy.Forward.Z,
                M41 = -dummy.Position.X, M42 = dummy.Position.Y, M43 = dummy.Position.Z, M44 = 1
            };
            return mat;
        }

        private void ConvertMeshes(BrgFile brg, BrgGltfParameters parameters, SceneBuilder sceneBuilder, NodeBuilder nodeBuilder, Dictionary<int, MaterialBuilder> matIdMatBuilderMap)
        {
            var primitives = from face in brg.Meshes[0].Faces
                             group face by face.MaterialIndex into faceGroup
                             select (Material: matIdMatBuilderMap[faceGroup.Key], Faces: faceGroup.ToList());

            IMeshBuilder<MaterialBuilder> mb = CreateMeshBuilder(brg.Meshes[0].Header.Flags);
            mb.Name = $"{parameters.ModelName}Mesh";
            ConvertMesh(brg.Meshes, primitives, mb);

            if (brg.Meshes.Count > 1)
            {
                // Create the node with morphing
                var nb = nodeBuilder.CreateNode($"{parameters.ModelName}Model");
                var instBuilder = sceneBuilder.AddRigidMesh(mb, nb);
                var morphWeights = new float[brg.Animation.MeshKeys.Count - 1];
                instBuilder.Content.UseMorphing().SetValue(morphWeights);

                // Animate morph weights
                var tb = instBuilder.Content.UseMorphing().UseTrackBuilder(TrackName);
                tb.SetPoint(brg.Animation.MeshKeys[0], morphWeights);
                for (var i = 1; i < brg.Animation.MeshKeys.Count; ++i)
                {
                    Array.Clear(morphWeights, 0, morphWeights.Length);
                    morphWeights[i - 1] = 1.0f;
                    tb.SetPoint(brg.Animation.MeshKeys[i], morphWeights);
                }
            }
            else
            {
                // Create the node
                var nb = nodeBuilder.CreateNode($"{parameters.ModelName}Model");
                var instBuilder = sceneBuilder.AddRigidMesh(mb, nb);
            }
        }
        
        private static IMeshBuilder<MaterialBuilder> CreateMeshBuilder(BrgMeshFlag flags)
        {
            bool hasColor = flags.HasFlag(BrgMeshFlag.AlphaChannel) || flags.HasFlag(BrgMeshFlag.ColorChannel);
            bool hasTex = flags.HasFlag(BrgMeshFlag.Texture1);

            if (hasColor && hasTex)
            {
                return new GltfMeshBuilder();
            }
            else if (hasTex)
            {
                return new GltfMeshBuilder2();
            }
            else if (hasColor)
            {
                return new GltfMeshBuilder3();
            }
            else
            {
                return new GltfMeshBuilder4();
            }
        }

        private void ConvertMesh(IReadOnlyList<BrgMesh> meshes, IEnumerable<(MaterialBuilder Material, List<BrgFace> Faces)> primitives, IMeshBuilder<MaterialBuilder> mb)
        {
            var mesh = meshes[0];
            var vertexBuilders = new Dictionary<int, IVertexBuilder>(mesh.Vertices.Count);
            foreach (var prim in primitives)
            {
                var pb = mb.UsePrimitive(prim.Material);

                foreach (var face in prim.Faces)
                {
                    if (!vertexBuilders.TryGetValue(face.A, out var vbA))
                    {
                        vbA = GetVertexBuilder(mesh, face.A);
                        vertexBuilders.Add(face.A, vbA);
                    }

                    if (!vertexBuilders.TryGetValue(face.B, out var vbB))
                    {
                        vbB = GetVertexBuilder(mesh, face.B);
                        vertexBuilders.Add(face.B, vbB);
                    }

                    if (!vertexBuilders.TryGetValue(face.C, out var vbC))
                    {
                        vbC = GetVertexBuilder(mesh, face.C);
                        vertexBuilders.Add(face.C, vbC);
                    }

                    var pbTri = pb.AddTriangle(vbA, vbC, vbB);
                    if (pbTri.A == -1 || pbTri.B == -1 || pbTri.C == -1)
                    {
                        // The primitive builder returns (-1, -1, -1) if a triangle with the given vertices already
                        // exists. This may be a premature optimization because two triangles could be the same in the
                        // base mesh, but they could animate in a different way through morph targets.
                        // Future: consider adding a really small adjustment to one of the vertex positions to avoid
                        // this optimization
                        continue;
                    }

                    // Each mesh is really a morph target after the first one
                    for (var i = 1; i < meshes.Count; ++i)
                    {
                        var mgA = GetVertexGeometry(meshes[i], face.A);
                        var mgB = GetVertexGeometry(meshes[i], face.B);
                        var mgC = GetVertexGeometry(meshes[i], face.C);

                        if (((!meshes[i].Header.Flags.HasFlag(BrgMeshFlag.Secondary) ||
                              meshes[i].Header.Flags.HasFlag(BrgMeshFlag.AnimTxCoords) ||
                              meshes[i].Header.Flags.HasFlag(BrgMeshFlag.ParticleSystem)) &&
                             meshes[i].Header.Flags.HasFlag(BrgMeshFlag.Texture1)) ||
                            (((meshes[i].Header.Flags.HasFlag(BrgMeshFlag.AlphaChannel) ||
                               meshes[i].Header.Flags.HasFlag(BrgMeshFlag.ColorChannel)) &&
                              !meshes[i].Header.Flags.HasFlag(BrgMeshFlag.Secondary)) ||
                             meshes[i].Header.Flags.HasFlag(BrgMeshFlag.AnimVertexColor)))
                        {
                            var mmA = GetVertexMaterial(meshes[i], face.A);
                            var mmB = GetVertexMaterial(meshes[i], face.B);
                            var mmC = GetVertexMaterial(meshes[i], face.C);

                            pb.SetVertexDelta(i - 1, pbTri.A, mgA.Subtract(vbA.GetGeometry()),
                                mmA.Subtract(vbA.GetMaterial()));

                            pb.SetVertexDelta(i - 1, pbTri.C, mgB.Subtract(vbB.GetGeometry()),
                                mmB.Subtract(vbB.GetMaterial()));

                            pb.SetVertexDelta(i - 1, pbTri.B, mgC.Subtract(vbC.GetGeometry()),
                                mmC.Subtract(vbC.GetMaterial()));
                        }
                        else
                        {
                            pb.SetVertexDelta(i - 1, pbTri.A, mgA.Subtract(vbA.GetGeometry()),
                                VertexMaterialDelta.Zero);

                            pb.SetVertexDelta(i - 1, pbTri.C, mgB.Subtract(vbB.GetGeometry()),
                                VertexMaterialDelta.Zero);

                            pb.SetVertexDelta(i - 1, pbTri.B, mgC.Subtract(vbC.GetGeometry()),
                                VertexMaterialDelta.Zero);
                        }
                    }
                }
            }
        }

        private static GltfVertexBuilder GetVertexBuilder(BrgMesh mesh, int index)
        {
            var vb = new GltfVertexBuilder
            {
                Geometry = GetVertexGeometry(mesh, index),
                Material = GetVertexMaterial(mesh, index)
            };

            return vb;
        }
        private static VertexPositionNormal GetVertexGeometry(BrgMesh mesh, int index)
        {
            var vert = mesh.Vertices[index];
            var norm = mesh.Normals[index];

            return new VertexPositionNormal(-vert.X, vert.Y, vert.Z, -norm.X, norm.Y, norm.Z);
        }
        private static VertexColor1Texture1 GetVertexMaterial(BrgMesh mesh, int index)
        {
            var vm = new VertexColor1Texture1();

            if ((!mesh.Header.Flags.HasFlag(BrgMeshFlag.Secondary) ||
                 mesh.Header.Flags.HasFlag(BrgMeshFlag.AnimTxCoords) ||
                 mesh.Header.Flags.HasFlag(BrgMeshFlag.ParticleSystem)) &&
                mesh.Header.Flags.HasFlag(BrgMeshFlag.Texture1))
            {
                vm.TexCoord = new Vector2(mesh.TextureCoordinates[index].X, 1 - mesh.TextureCoordinates[index].Y);
            }

            if (((mesh.Header.Flags.HasFlag(BrgMeshFlag.AlphaChannel) ||
                  mesh.Header.Flags.HasFlag(BrgMeshFlag.ColorChannel)) &&
                 !mesh.Header.Flags.HasFlag(BrgMeshFlag.Secondary)) ||
                mesh.Header.Flags.HasFlag(BrgMeshFlag.AnimVertexColor))
            {
                vm.Color = mesh.Colors[index];
            }

            return vm;
        }

        private void ConvertMaterials(BrgFile brg, TextureManager textureManager, Dictionary<int, MaterialBuilder> matIdMatBuilderMap)
        {
            var uniqueTextures = from mat in brg.Materials
                                 group mat by mat.DiffuseMapName into matGroup
                                 select (TexName: matGroup.Key, Materials: matGroup.ToList());
            foreach (var matGroup in uniqueTextures)
            {
                string imageFile = matGroup.TexName;
                var texData = CreateMemoryImage(imageFile, textureManager);

                foreach (var brgMat in matGroup.Materials)
                {
                    var mb = new MaterialBuilder(GetMaterialNameWithFlags(brgMat));
                    mb.WithMetallicRoughnessShader();
                    var cb = mb.UseChannel(KnownChannel.MetallicRoughness);
                    cb.Parameters[KnownProperty.MetallicFactor] = 0.1f;
                    cb.Parameters[KnownProperty.RoughnessFactor] = 0.5f;

                    cb = mb.UseChannel(KnownChannel.BaseColor);
                    cb.Parameters[KnownProperty.RGBA] = new Vector4(brgMat.DiffuseColor, brgMat.Opacity);
                    if (!string.IsNullOrWhiteSpace(matGroup.TexName))
                    {
                        var tb = cb.UseTexture();
                        tb.Name = texData.Image.Name;
                        tb.WrapS = brgMat.Flags.HasFlag(BrgMatFlag.WrapUTx1) ? TextureWrapMode.REPEAT : TextureWrapMode.CLAMP_TO_EDGE;
                        tb.WrapT = brgMat.Flags.HasFlag(BrgMatFlag.WrapVTx1) ? TextureWrapMode.REPEAT : TextureWrapMode.CLAMP_TO_EDGE;
                        tb.WithPrimaryImage(texData.Image);
                    }

                    cb = mb.UseChannel(KnownChannel.SpecularColor);
                    cb.Parameters[KnownProperty.RGB] = brgMat.SpecularColor;

                    cb = mb.UseChannel(KnownChannel.Emissive);
                    cb.Parameters[KnownProperty.RGB] = brgMat.EmissiveColor;
                    cb.Parameters[KnownProperty.EmissiveStrength] = 1.0f;

                    if (brgMat.Flags.HasFlag(BrgMatFlag.TwoSided))
                    {
                        mb.DoubleSided = true;
                    }

                    // TODO: Figure out alpha mode based on material and texture info
                    if (brgMat.Flags.HasFlag(BrgMatFlag.PixelXForm1) ||
                        brgMat.Flags.HasFlag(BrgMatFlag.PixelXForm1) ||
                        brgMat.Flags.HasFlag(BrgMatFlag.PixelXForm2) ||
                        brgMat.Flags.HasFlag(BrgMatFlag.PlayerXFormColor1) ||
                        brgMat.Flags.HasFlag(BrgMatFlag.PlayerXFormColor2) ||
                        brgMat.Flags.HasFlag(BrgMatFlag.PlayerXFormColor2) ||
                        brgMat.Flags.HasFlag(BrgMatFlag.PlayerXFormTx1) ||
                        brgMat.Flags.HasFlag(BrgMatFlag.PlayerXFormTx2) ||
                        !(texData.Texture?.AlphaTest ?? true))
                    {
                        mb.WithAlpha(SharpGLTF.Materials.AlphaMode.OPAQUE);
                    }
                    else
                    {
                        mb.WithAlpha(SharpGLTF.Materials.AlphaMode.MASK);
                    }

                    // Some brgs have multiple materials with the same ID, use the latest one
                    matIdMatBuilderMap[brgMat.Id] = mb;
                }
            }
        }

        private static string GetMaterialNameWithFlags(BrgMaterial mat)
        {
            var sb = new StringBuilder(Path.GetFileNameWithoutExtension(mat.DiffuseMapName));
            sb.Append('(');

            if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormColor1))
            {
                sb.Append(" colorxform1");
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormColor2))
            {
                sb.Append(" colorxform2");
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormColor3))
            {
                sb.Append(" colorxform3");
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PixelXForm1))
            {
                sb.Append(" pixelxform1");
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PixelXForm2))
            {
                sb.Append(" pixelxform2");
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PixelXForm3))
            {
                sb.Append(" pixelxform3");
            }

            if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormTx1))
            {
                sb.Append(" texturexform1");
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormTx2))
            {
                sb.Append(" texturexform2");
            }

            if (mat.Flags.HasFlag(BrgMatFlag.TwoSided))
            {
                sb.Append(" 2-sided");
            }

            sb.Append(')');
            return sb.ToString();
        }

        private static (ImageBuilder Image, Texture? Texture) CreateMemoryImage(string imageFile, TextureManager textureManager)
        {
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
                    imageBuilder = ImageBuilder.From(memImage, imageFile);
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
                    var memImage = new MemoryImage(ms.ToArray());
                    imageBuilder = ImageBuilder.From(memImage, imageFile);
                }
            }

            return (imageBuilder, tex);
        }
    }
}
