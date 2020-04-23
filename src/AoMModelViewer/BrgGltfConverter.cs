using AoMEngineLibrary.Graphics.Brg;
using AoMEngineLibrary.Graphics.Dds;
using HelixToolkit.Wpf;
using SharpGLTF.Animations;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Channels;

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

namespace AoMModelViewer
{
    public class BrgGltfConverter
    {
        private const string TrackName = "Default";
        private readonly byte[] blankImageData;

        public BrgGltfConverter()
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

        public ModelRoot Convert(BrgFile brg)
        {
            var sceneBuilder = new SceneBuilder();
            var rootNodeBuilder = new NodeBuilder("root");

            Dictionary<int, MaterialBuilder> matIdMatBuilderMap = new Dictionary<int, MaterialBuilder>();
            ConvertMaterials(brg, matIdMatBuilderMap);

            ConvertMeshes(brg, sceneBuilder, rootNodeBuilder, matIdMatBuilderMap);

            ConvertAttachpoints(brg, sceneBuilder, rootNodeBuilder);

            return sceneBuilder.ToGltf2();
        }

        private void ConvertAttachpoints(BrgFile brg, SceneBuilder sceneBuilder, NodeBuilder nodeBuilder)
        {
            var mb = CreateAttachpointMesh();

            // Create hotspot
            var hnb = nodeBuilder.CreateNode("Dummy_hotspot");
            hnb.UseTranslation().Value = brg.Meshes[0].Header.HotspotPosition;
            var ttb = hnb.UseTranslation().UseTrackBuilder(TrackName);
            ttb.SetPoint(brg.Animation.MeshKeys[0], brg.Meshes[0].Header.HotspotPosition);
            for (int i = 1; i < brg.Meshes.Count; ++i)
            {
                ttb.SetPoint(brg.Animation.MeshKeys[i], brg.Meshes[i].Header.HotspotPosition);
            }
            sceneBuilder.AddRigidMesh(mb, hnb);

            for (int i = 0; i < brg.Meshes[0].Attachpoints.Count; ++i)
            {
                var nb = ConvertAttachpoint(brg, i, nodeBuilder);
                sceneBuilder.AddRigidMesh(mb, nb);
            }
        }
        private MeshBuilder<VertexPosition, VertexColor1> CreateAttachpointMesh()
        {
            var mb = new MeshBuilder<VertexPosition, VertexColor1>("attachpointMesh");
            var pb = mb.UsePrimitive(MaterialBuilder.CreateDefault(), 3);

            // AoM dummy axes and colors
            //AddAxisToMesh(pb, new Vector3(-0.5f, 0.005f, 0.005f), new Vector4(0, 0, 1, 1));
            //AddAxisToMesh(pb, new Vector3(0.005f, 0.5f, 0.005f), new Vector4(0, 1, 0, 1));
            //AddAxisToMesh(pb, new Vector3(0.005f, 0.005f, 0.5f), new Vector4(1, 0, 0, 1));
            // Blender/3ds Max axes and colors
            AddAxisToMesh(pb, new Vector3(0.25f, 0.005f, 0.005f), new Vector4(1, 0, 0, 1));
            AddAxisToMesh(pb, new Vector3(0.005f, 0.005f, -0.25f), new Vector4(0, 1, 0, 1));
            AddAxisToMesh(pb, new Vector3(0.005f, 0.25f, 0.005f), new Vector4(0, 0, 1, 1));

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

                var faces = new List<BrgFace>()
                {
                    new BrgFace() { Indices = new List<short>() { 0, 2, 1 } },
                    new BrgFace() { Indices = new List<short>() { 0, 3, 2 } },
                    new BrgFace() { Indices = new List<short>() { 2, 3, 4 } },
                    new BrgFace() { Indices = new List<short>() { 2, 4, 5 } },
                    new BrgFace() { Indices = new List<short>() { 1, 2, 5 } },
                    new BrgFace() { Indices = new List<short>() { 1, 5, 6 } },
                    new BrgFace() { Indices = new List<short>() { 0, 7, 4 } },
                    new BrgFace() { Indices = new List<short>() { 0, 4, 3 } },
                    new BrgFace() { Indices = new List<short>() { 5, 4, 7 } },
                    new BrgFace() { Indices = new List<short>() { 5, 7, 6 } },
                    new BrgFace() { Indices = new List<short>() { 0, 6, 7 } },
                    new BrgFace() { Indices = new List<short>() { 0, 1, 6 } }
                };

                foreach (var face in faces)
                {
                    pb.AddTriangle(vbs[face.Indices[0]], vbs[face.Indices[1]], vbs[face.Indices[2]]);
                }
            }
        }
        private MeshBuilder<VertexPosition, VertexColor1> CreateAttachpointLineMesh()
        {
            var mb = new MeshBuilder<VertexPosition, VertexColor1>("attachpointMesh");
            var pb = mb.UsePrimitive(MaterialBuilder.CreateDefault(), 2);

            var vb1 = new VertexBuilder<VertexPosition, VertexColor1, VertexEmpty>();
            var vb2 = new VertexBuilder<VertexPosition, VertexColor1, VertexEmpty>();
            vb1.Material.Color = new Vector4(0, 0, 1, 1);
            vb2.Geometry.Position = new Vector3(-0.5f, 0, 0);
            vb2.Material.Color = vb1.Material.Color;
            pb.AddLine(vb1, vb2);
            vb1.Material.Color = new Vector4(0, 1, 0, 1);
            vb2.Geometry.Position = new Vector3(0, 0.5f, 0);
            vb2.Material.Color = vb1.Material.Color;
            pb.AddLine(vb1, vb2);
            vb1.Material.Color = new Vector4(1, 0, 0, 1);
            vb2.Geometry.Position = new Vector3(0, 0, 0.5f);
            vb2.Material.Color = vb1.Material.Color;
            pb.AddLine(vb1, vb2);

            return mb;
        }
        private NodeBuilder ConvertAttachpoint(BrgFile brg, int attpIndex, NodeBuilder nodeBuilder)
        {
            var attp = brg.Meshes[0].Attachpoints[attpIndex];
            var nb = nodeBuilder.CreateNode("Dummy_" + attp.Name);

            var worldMatrix = GetAttachpointWorldMatrix(attp);
            Matrix4x4.Decompose(worldMatrix, out Vector3 scale, out Quaternion rot, out Vector3 trans);
            nb.UseTranslation().Value = trans;
            nb.UseRotation().Value = rot;
            nb.UseScale().Value = scale;
            var ttb = nb.UseTranslation().UseTrackBuilder(TrackName);
            var rtb = nb.UseRotation().UseTrackBuilder(TrackName);
            var stb = nb.UseScale().UseTrackBuilder(TrackName);
            ttb.SetPoint(brg.Animation.MeshKeys[0], trans);
            rtb.SetPoint(brg.Animation.MeshKeys[0], rot);
            stb.SetPoint(brg.Animation.MeshKeys[0], scale);

            for (int i = 1; i < brg.Meshes.Count; ++i)
            {
                attp = brg.Meshes[i].Attachpoints[attpIndex];
                worldMatrix = GetAttachpointWorldMatrix(attp);
                Matrix4x4.Decompose(worldMatrix, out scale, out rot, out trans);
                ttb.SetPoint(brg.Animation.MeshKeys[i], trans);
                rtb.SetPoint(brg.Animation.MeshKeys[i], rot);
                stb.SetPoint(brg.Animation.MeshKeys[i], scale);
            }

            return nb;
        }
        private Matrix4x4 GetAttachpointWorldMatrix(BrgAttachpoint attp)
        {
            Matrix4x4 mat = new Matrix4x4();
            mat.M11 = attp.Right.X; mat.M12 = attp.Right.Y; mat.M13 = attp.Right.Z;
            mat.M21 = attp.Up.X; mat.M22 = attp.Up.Y; mat.M23 = attp.Up.Z;
            mat.M31 = attp.Forward.X; mat.M32 = attp.Forward.Y; mat.M33 = attp.Forward.Z;
            mat.M41 = -attp.Position.X; mat.M42 = attp.Position.Y; mat.M43 = attp.Position.Z; mat.M44 = 1;
            return mat;
        }

        private void ConvertMeshes(BrgFile brg, SceneBuilder sceneBuilder, NodeBuilder nodeBuilder, Dictionary<int, MaterialBuilder> matIdMatBuilderMap)
        {
            var primitives = from face in brg.Meshes[0].Faces
                             group face by face.MaterialIndex into faceGroup
                             select (Material: matIdMatBuilderMap[faceGroup.Key], Faces: faceGroup.ToList());

            IMeshBuilder<MaterialBuilder> mb = CreateMeshBuilder(brg.Meshes[0].Header.Flags);
            mb.Name = "brgMesh";
            ConvertMesh(brg.Meshes[0], primitives, mb);

            // Each mesh is really a morph target after the first one
            for (int i = 1; i < brg.Meshes.Count; ++i)
            {
                var mt = mb.UseMorphTarget(i - 1);
                var mesh = brg.Meshes[i];
                for (int v = 0; v < mesh.Vertices.Count; ++v)
                {
                    var baseVal = GetVertexGeometry(brg.Meshes[0], v);
                    var morphVal = GetVertexGeometry(mesh, v);
                    mt.SetVertexDelta(baseVal.Position, morphVal.Subtract(baseVal));
                }
            }

            // Create the node with morphing
            var nb = nodeBuilder.CreateNode("brgModel");
            var instBuilder = sceneBuilder.AddRigidMesh(mb, nb);
            instBuilder.Content.UseMorphing().Value = SparseWeight8.Create(0);

            // Animate morph weights
            var tb  = instBuilder.Content.UseMorphing().UseTrackBuilder(TrackName);
            tb.SetPoint(brg.Animation.MeshKeys[0], SparseWeight8.Create(0));
            for (int i = 1; i < brg.Animation.MeshKeys.Count; ++i)
            {
                tb.SetPoint(brg.Animation.MeshKeys[i], new SparseWeight8(new Vector4(i - 1, 0, 0, 0), new Vector4(1, 0, 0, 0)));
            }
        }
        private IMeshBuilder<MaterialBuilder> CreateMeshBuilder(BrgMeshFlag flags)
        {
            bool hasColor = flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) || flags.HasFlag(BrgMeshFlag.COLORCHANNEL);
            bool hasTex = flags.HasFlag(BrgMeshFlag.TEXCOORDSA);

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
        private void ConvertMesh(BrgMesh mesh, IEnumerable<(MaterialBuilder Material, List<BrgFace> Faces)> primitives, IMeshBuilder<MaterialBuilder> mb)
        {
            foreach (var prim in primitives)
            {
                var pb = mb.UsePrimitive(prim.Material);
                
                foreach (var face in prim.Faces)
                {
                    pb.AddTriangle(GetVertexBuilder(mesh, face.Indices[0]), GetVertexBuilder(mesh, face.Indices[2]), GetVertexBuilder(mesh, face.Indices[1]));
                }
            }
        }
        private GltfVertexBuilder GetVertexBuilder(BrgMesh mesh, int index)
        {
            var vb = new GltfVertexBuilder();

            var vert = mesh.Vertices[index];
            var norm = mesh.Normals[index];

            vb.Geometry = new VertexPositionNormal(-vert.X, vert.Y, vert.Z, -norm.X, norm.Y, norm.Z);

            if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
            {
                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                {
                    vb.Material.TexCoord = new Vector2(mesh.TextureCoordinates[index].X, 1 - mesh.TextureCoordinates[index].Y);
                }

                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL) ||
                    mesh.Header.Flags.HasFlag(BrgMeshFlag.COLORCHANNEL))
                {
                    vb.Material.Color = mesh.Colors[index];
                }
            }

            return vb;
        }
        private VertexPositionNormal GetVertexGeometry(BrgMesh mesh, int index)
        {
            var vert = mesh.Vertices[index];
            var norm = mesh.Normals[index];

            return new VertexPositionNormal(-vert.X, vert.Y, vert.Z, -norm.X, norm.Y, norm.Z);
        }

        private void ConvertMaterials(BrgFile brg, Dictionary<int, MaterialBuilder> matIdMatBuilderMap)
        {
            var uniqueTextures = from mat in brg.Materials
                                 group mat by mat.DiffuseMapName into matGroup
                                 select (TexName: matGroup.Key, Materials: matGroup.ToList());
            foreach (var matGroup in uniqueTextures)
            {
                string imageFile = matGroup.TexName + ".png";
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

                foreach (var brgMat in matGroup.Materials)
                {
                    var mb = new MaterialBuilder(GetMaterialNameWithFlags(brgMat));
                    mb.WithMetallicRoughnessShader();
                    var cb = mb.UseChannel(KnownChannel.MetallicRoughness);
                    cb.Parameter = new Vector4(0.2f, 0.5f, 0, 0);
                    cb = mb.UseChannel(KnownChannel.BaseColor);
                    cb.Parameter = new Vector4(brgMat.DiffuseColor, brgMat.Opacity);

                    if (!string.IsNullOrWhiteSpace(matGroup.TexName))
                    {
                        var tb = cb.UseTexture();
                        tb.WrapS = brgMat.Flags.HasFlag(BrgMatFlag.WrapUTx1) ? TextureWrapMode.CLAMP_TO_EDGE : TextureWrapMode.REPEAT;
                        tb.WrapT = brgMat.Flags.HasFlag(BrgMatFlag.WrapVTx1) ? TextureWrapMode.CLAMP_TO_EDGE : TextureWrapMode.REPEAT;
                        tb.WithPrimaryImage(memImage);
                    }

                    cb = mb.UseChannel(KnownChannel.Emissive);
                    cb.Parameter = new Vector4(brgMat.EmissiveColor, 0);

                    if (brgMat.Flags.HasFlag(BrgMatFlag.TwoSided))
                    {
                        mb.DoubleSided = true;
                    }

                    // TODO: add all the transforms as mask
                    //if (brgMat.Flags.HasFlag(BrgMatFlag.PixelXForm1))
                    //{
                    //    mb.WithAlpha(SharpGLTF.Materials.AlphaMode.MASK);
                    //}
                    //else
                    //{
                    //    mb.WithAlpha(SharpGLTF.Materials.AlphaMode.OPAQUE);
                    //}

                    matIdMatBuilderMap.Add(brgMat.Id, mb);
                }
            }
        }
        private string GetMaterialNameWithFlags(BrgMaterial mat)
        {
            string name = Path.GetFileNameWithoutExtension(mat.DiffuseMapName) ?? string.Empty;

            if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormColor1))
            {
                name += " colorxform1";
            }

            if (mat.Flags.HasFlag(BrgMatFlag.PixelXForm1))
            {
                name += " pixelxform1";
            }

            if (mat.Flags.HasFlag(BrgMatFlag.TwoSided))
            {
                name += " 2-sided";
            }

            return name;
        }
    }
}
