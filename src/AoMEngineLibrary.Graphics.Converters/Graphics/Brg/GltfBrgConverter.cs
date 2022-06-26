using SharpGLTF.Materials;
using SharpGLTF.Runtime;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using AlphaMode = SharpGLTF.Schema2.AlphaMode;
using GltfMaterial = SharpGLTF.Schema2.Material;
using GltfAnimation = SharpGLTF.Schema2.Animation;
using GltfMeshBuilder = SharpGLTF.Geometry.MeshBuilder<
    SharpGLTF.Schema2.Material,
    SharpGLTF.Geometry.VertexTypes.VertexPositionNormal,
    SharpGLTF.Geometry.VertexTypes.VertexColor1Texture1,
    SharpGLTF.Geometry.VertexTypes.VertexEmpty>;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class GltfBrgConverter
    {
        private record GltfMeshData(GltfMeshBuilder MeshBuilder, bool HasTexCoords);

        public GltfBrgConverter()
        {

        }

        public BrgFile Convert(ModelRoot gltfFile, GltfBrgParameters parameters)
        {
            BrgFile brg = new BrgFile();
            var model = gltfFile;

            // We'll only export the default scene and the chosen animation
            var scene = model.DefaultScene;
            var animIndex = parameters.AnimationIndex;
            if (animIndex < 0 || animIndex >= model.LogicalAnimations.Count) animIndex = 0;
            var anim = model.LogicalAnimations.Count > 0 ? model.LogicalAnimations[animIndex] : null;

            // Figure out animation duration and keys
            float fps = parameters.SampleRateFps;
            if (anim == null)
            {
                brg.Header.NumMeshes = 1;
            }
            else
            {
                brg.Animation.Duration = anim.Duration;
                brg.Header.NumMeshes = (int)(anim.Duration * fps + 0.5f);
            }

            float spf = brg.Animation.Duration / (brg.Header.NumMeshes - 1);
            for (int i = 0; i < brg.Header.NumMeshes; ++i)
            {
                brg.Animation.MeshKeys.Add(i * spf);
            }

            // Convert meshes, materials and attachpoints
            ConvertMeshes(scene, anim, brg);

            // Perform final processing
            BrgMesh mesh = brg.Meshes[0];
            HashSet<int> diffFaceMats = mesh.Faces.Select(f => (int)f.MaterialIndex).ToHashSet();
            if (diffFaceMats.Count > 0)
            {
                mesh.ExtendedHeader.NumMaterials = (byte)(diffFaceMats.Count - 1);
                mesh.ExtendedHeader.NumUniqueMaterials = diffFaceMats.Count;
            }

            brg.Materials = brg.Materials.Where(m => diffFaceMats.Contains(m.Id)).ToList();
            brg.Header.NumMaterials = brg.Materials.Count;

            return brg;
        }

        private void ConvertMeshes(Scene gltfScene, GltfAnimation? gltfAnimation, BrgFile brg)
        {
            Dictionary<int, int> matIdMapping = new Dictionary<int, int>();

            var sceneTemplate = SceneTemplate.Create(gltfScene, new RuntimeOptions() { IsolateMemory = false });
            var instance = sceneTemplate.CreateInstance();

            Predicate<string> noDummySelector = nodeName => !nodeName.StartsWith("dummy_", StringComparison.InvariantCultureIgnoreCase);

            // Mesh Frames
            for (int i = 0; i < brg.Header.NumMeshes; i++)
            {
                // Compute the data of the scene at time
                if (gltfAnimation == null)
                {
                    instance.Armature.SetPoseTransforms();
                }
                else
                {
                    instance.Armature.SetAnimationFrame(gltfAnimation.LogicalIndex, brg.Animation.MeshKeys[i]);
                }

                // Add a new mesh (frame)
                var mesh = new BrgMesh();
                mesh.Header.Format = BrgMeshFormat.HasFaceNormals | BrgMeshFormat.AnimationLength;
                mesh.ExtendedHeader.AnimationLength = brg.Animation.Duration;
                if (i > 0)
                {
                    mesh.Header.Flags |= BrgMeshFlag.Secondary;
                }
                brg.Meshes.Add(mesh);

                // Convert all the attachpoints in the scene
                ConvertAttachpoints(gltfScene, instance, mesh);

                // Evaluate the entire gltf scene
                ConvertSceneToMeshFrame(gltfScene, instance, noDummySelector, brg, mesh, matIdMapping);

                // Validate the mesh settings
                brg.UpdateMeshSettings(i, brg.Meshes[0].Header.Flags, brg.Meshes[0].Header.Format, brg.Meshes[0].Header.AnimationType, brg.Meshes[0].Header.InterpolationType);
            }

            BrgMeshOptimizer.Optimize(brg.Meshes);
        }

        private static void ConvertSceneToMeshFrame(Scene scene, SceneInstance instance, Predicate<string> nodeNameSelector,
            BrgFile brg, BrgMesh mesh, Dictionary<int, int> matIdMapping)
        {
            // Add every drawable instance in the scene to the mesh frame
            var drawableInstances = instance
                .Where(d => nodeNameSelector(d.Template.NodeName))
                .ToArray();

            // Get all meshes in scene and compute some data
            var meshes = drawableInstances
                .Select(d => scene.LogicalParent.LogicalMeshes[d.Template.LogicalMeshIndex])
                .ToArray();
            var sceneHasTexCoords = meshes
                .Where(m => nodeNameSelector(m.Name))
                .SelectMany(m => m.Primitives)
                .Any(p => p.VertexAccessors.Keys.Any(k => k.StartsWith("TEXCOORD_", StringComparison.Ordinal)));
            var sceneHasColors = meshes
                .Where(m => nodeNameSelector(m.Name))
                .SelectMany(m => m.Primitives)
                .Any(p => p.VertexAccessors.Keys.Any(k => k.StartsWith("COLOR_", StringComparison.Ordinal)));
            var sceneHasAnimTexCoords = meshes
                .Where(m => nodeNameSelector(m.Name))
                .SelectMany(m => m.Primitives)
                .SelectMany(p => Enumerable.Range(0, p.MorphTargetsCount).Select(p.GetMorphTargetAccessors))
                .Any(a => a.Keys.Any(k => k.StartsWith("TEXCOORD_", StringComparison.Ordinal)));
            var sceneHasAnimColors = meshes
                .Where(m => nodeNameSelector(m.Name))
                .SelectMany(m => m.Primitives)
                .SelectMany(p => Enumerable.Range(0, p.MorphTargetsCount).Select(p.GetMorphTargetAccessors))
                .Any(a => a.Keys.Any(k => k.StartsWith("COLOR_", StringComparison.Ordinal)));

            foreach (var drawableInstance in drawableInstances)
            {
                var gltfMesh = scene.LogicalParent.LogicalMeshes[drawableInstance.Template.LogicalMeshIndex];
                var decoder = gltfMesh.Decode();

                // Create brg materials and a way to map their ids
                var gltfMats = decoder.Primitives.Select(p => p.Material);
                ConvertMaterials(gltfMats, brg, matIdMapping);

                ConvertMesh(decoder, drawableInstance.Transform, sceneHasTexCoords, sceneHasColors,
                    sceneHasAnimTexCoords, sceneHasAnimColors, mesh, matIdMapping);
            }

            // Update header data
            mesh.Header.Version = 22;
            mesh.Header.ExtendedHeaderSize = 40;
            mesh.Header.NumVertices = (ushort)mesh.Vertices.Count;
            mesh.Header.NumFaces = (ushort)mesh.Faces.Count;

            // Calculate extents, etc.
            Vector3 centerPos = new Vector3();
            Vector3 minExtent = new Vector3(float.MaxValue);
            Vector3 maxExtent = new Vector3(float.MinValue);
            foreach (var pos in mesh.Vertices)
            {
                centerPos += pos;
                minExtent.X = Math.Min(pos.X, minExtent.X);
                minExtent.Y = Math.Min(pos.Y, minExtent.Y);
                minExtent.Z = Math.Min(pos.Z, minExtent.Z);
                maxExtent.X = Math.Max(pos.X, maxExtent.X);
                maxExtent.Y = Math.Max(pos.Y, maxExtent.Y);
                maxExtent.Z = Math.Max(pos.Z, maxExtent.Z);
            }

            // Update header values
            centerPos /= mesh.Vertices.Count;
            Vector3 bbox = (maxExtent - minExtent) / 2;
            mesh.Header.CenterPosition = centerPos;
            mesh.Header.MinimumExtent = -bbox;
            mesh.Header.MaximumExtent = bbox;
            mesh.Header.CenterRadius = Math.Max(Math.Max(bbox.X, bbox.Y), bbox.Z);
        }

        private static void ConvertMesh(IMeshDecoder<GltfMaterial> decoder, IGeometryTransform transform,
            bool sceneHasTexCoords, bool sceneHasColors, bool sceneHasAnimTexCoords, bool sceneHasAnimColors,
            BrgMesh mesh, Dictionary<int, int> matIdMapping)
        {
            foreach (var p in decoder.Primitives)
            {
                // Skip primitives without material
                // TODO: figure out if there's an alternative to skipping
                if (p.Material is null) continue;

                // Get positions and normals
                var baseVertexIndex = mesh.Vertices.Count;
                for (int i = 0; i < p.VertexCount; ++i)
                {
                    var pos = p.GetPosition(i, transform);
                    var norm = p.GetNormal(i, transform);
                    mesh.Vertices.Add(new Vector3(-pos.X, pos.Y, pos.Z));
                    mesh.Normals.Add(new Vector3(-norm.X, norm.Y, norm.Z));
                }

                // Get TexCoords
                if (sceneHasTexCoords)
                {
                    // the scene may have tex coords, but maybe not each primitive
                    // the API will simply get a tex coord with (0, 0) for ones that don't
                    mesh.Header.Flags |= BrgMeshFlag.Texture1;
                    if (sceneHasAnimTexCoords) mesh.Header.Flags |= BrgMeshFlag.AnimTxCoords;

                    if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.Secondary) ||
                        mesh.Header.Flags.HasFlag(BrgMeshFlag.AnimTxCoords))
                    {
                        var texCoordSet = GetDiffuseBaseColorTexCoordSet(p.Material);
                        for (var i = 0; i < p.VertexCount; ++i)
                        {
                            var tc = p.GetTextureCoord(i, texCoordSet, transform);
                            mesh.TextureCoordinates.Add(new Vector2(tc.X, 1 - tc.Y));
                        }
                    }
                }

                // Get Colors
                if (sceneHasColors)
                {
                    // the scene may have colors, but maybe not each primitive
                    // the API will simply get a color with (1, 1, 1, 1) for ones that don't
                    mesh.Header.Flags |= BrgMeshFlag.ColorChannel | BrgMeshFlag.AlphaChannel;
                    if (sceneHasAnimColors) mesh.Header.Flags |= BrgMeshFlag.AnimVertexColor;

                    if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.Secondary) ||
                        mesh.Header.Flags.HasFlag(BrgMeshFlag.AnimVertexColor))
                    {
                        for (var i = 0; i < p.VertexCount; ++i)
                        {
                            var color = p.GetColor(i, 0, transform);
                            mesh.Colors.Add(color);
                        }
                    }
                }

                // Get faces
                if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.Secondary))
                {
                    mesh.Header.Flags |= BrgMeshFlag.Material;
                    mesh.VertexMaterials.AddRange(Enumerable.Repeat((short)0, mesh.Vertices.Count));

                    short faceMatIndex;
                    var faceMatId = p.Material.LogicalIndex;
                    if (matIdMapping.ContainsKey(faceMatId))
                    {
                        faceMatIndex = (short)matIdMapping[faceMatId];
                    }
                    else
                    {
                        throw new InvalidDataException($"A face has an invalid material id {faceMatId}.");
                    }

                    foreach (var (A, B, C) in p.TriangleIndices)
                    {
                        var a = System.Convert.ToUInt16(A + baseVertexIndex);
                        var b = System.Convert.ToUInt16(B + baseVertexIndex);
                        var c = System.Convert.ToUInt16(C + baseVertexIndex);

                        var f = new BrgFace(a, c, b) {MaterialIndex = faceMatIndex};
                        mesh.Faces.Add(f);

                        mesh.VertexMaterials[f.A] = f.MaterialIndex;
                        mesh.VertexMaterials[f.B] = f.MaterialIndex;
                        mesh.VertexMaterials[f.C] = f.MaterialIndex;
                    }
                }
            }
        }

        private void ConvertAttachpoints(Scene scene, SceneInstance sceneInstance, BrgMesh mesh)
        {
            // Convert Attachpoints
            var dummies = sceneInstance.Armature.LogicalNodes.Where(n => n.Name.StartsWith("dummy_", StringComparison.InvariantCultureIgnoreCase));

            if (dummies.Any())
            {
                mesh.Header.Flags |= BrgMeshFlag.DummyObjects;
                foreach (var dummy in dummies)
                {
                    var aName = dummy.Name;
                    if (!BrgDummyTypeInfo.TryGetByName(aName.Substring(6), out var typeInfo))
                    {
                        // Check if it's hotspot
                        if (aName.Equals("Dummy_hotspot", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var trans = dummy.ModelMatrix.Translation;
                            mesh.Header.HotspotPosition = new Vector3(-trans.X, trans.Y, trans.Z);
                        }
                        continue;
                    }

                    var transform = dummy.ModelMatrix;
                    var att = new BrgDummy
                    {
                        Type = typeInfo.Type,
                        Up = new Vector3(transform.M21, transform.M22, transform.M23),
                        Forward = new Vector3(transform.M31, transform.M32, transform.M33),
                        Right = new Vector3(transform.M11, transform.M12, transform.M13),
                        Position = new Vector3(-transform.M41, transform.M42, transform.M43)
                    };

                    // TODO: Calculate dummy bounding box

                    mesh.Dummies.Add(att);
                }
            }
        }

        private static void ConvertMaterials(IEnumerable<GltfMaterial> gltfMats, BrgFile brg, Dictionary<int, int> matIdMapping)
        {
            foreach (var gltfMat in gltfMats)
            {
                if (gltfMat == null) continue;

                var mat = new BrgMaterial();
                mat.Id = brg.Materials.Count + 1;
                ConvertMaterial(gltfMat, mat);

                int matListIndex = brg.Materials.IndexOf(mat);
                int actualMatId = gltfMat.LogicalIndex;
                if (matListIndex >= 0)
                {
                    if (!matIdMapping.ContainsKey(actualMatId))
                    {
                        matIdMapping.Add(actualMatId, brg.Materials[matListIndex].Id);
                    }
                }
                else
                {
                    brg.Materials.Add(mat);
                    if (matIdMapping.ContainsKey(actualMatId))
                    {
                        matIdMapping[actualMatId] = mat.Id;
                    }
                    else
                    {
                        matIdMapping.Add(actualMatId, mat.Id);
                    }
                }
            }
        }

        private static void ConvertMaterial(GltfMaterial glMat, BrgMaterial mat)
        {
            mat.DiffuseColor = GetDiffuseColor(glMat);
            mat.AmbientColor = mat.DiffuseColor;
            mat.SpecularColor = GetSpecularColor(glMat);
            mat.EmissiveColor = GetEmissiveColor(glMat);

            mat.SpecularExponent = GetSpecularPower(glMat);
            if (mat.SpecularExponent > 0)
            {
                mat.Flags |= BrgMatFlag.SpecularExponent;
            }

            mat.Opacity = GetAlphaLevel(glMat);
            if (mat.Opacity < 1f)
            {
                mat.Flags |= BrgMatFlag.Alpha;
            }

            if (glMat.DoubleSided)
            {
                mat.Flags |= BrgMatFlag.TwoSided;
            }

            mat.DiffuseMapName = GetDiffuseTexture(glMat, out BrgMatFlag wrapFlags);
            int parenthIndex = mat.DiffuseMapName.IndexOf('(');
            if (parenthIndex >= 0)
            {
                mat.DiffuseMapName = mat.DiffuseMapName.Remove(parenthIndex);
            }
            if (mat.DiffuseMapName.Length > 0)
            {
                mat.Flags |= wrapFlags;
            }

            GetMaterialFlagsFromName(glMat, mat);
        }
        private static Vector3 GetDiffuseColor(GltfMaterial srcMaterial)
        {
            var diffuse = srcMaterial.GetDiffuseColor(Vector4.One);
            return new Vector3(diffuse.X, diffuse.Y, diffuse.Z);
        }
        private static Vector3 GetSpecularColor(GltfMaterial srcMaterial)
        {
            var mr = srcMaterial.FindChannel("SpecularColor");
            return !mr.HasValue ? Vector3.Zero : new Vector3(mr.Value.Color.X, mr.Value.Color.Y, mr.Value.Color.Z);
        }
        private static Vector3 GetEmissiveColor(GltfMaterial srcMaterial)
        {
            var emissive = srcMaterial.FindChannel("Emissive");
            if (!emissive.HasValue) return Vector3.Zero;
            return new Vector3(emissive.Value.Color.X, emissive.Value.Color.Y, emissive.Value.Color.Z);
        }
        private static float GetSpecularPower(GltfMaterial srcMaterial)
        {
            var mr = srcMaterial.FindChannel("MetallicRoughness");
            if (!mr.HasValue) return 0;

            var metallic = mr.Value.GetFactor(KnownProperty.MetallicFactor.ToString());
            var roughness = mr.Value.GetFactor(KnownProperty.RoughnessFactor.ToString());
            var mult = metallic - roughness;

            return mult <= 0 ? 0 : 25 * mult;
        }
        private static float GetAlphaLevel(GltfMaterial srcMaterial)
        {
            if (srcMaterial.Alpha == AlphaMode.OPAQUE) return 1;

            var baseColor = srcMaterial.FindChannel("BaseColor");
            if (baseColor == null) return 1;
            return baseColor.Value.Color.W;
        }
        private static string GetDiffuseTexture(GltfMaterial srcMaterial, out BrgMatFlag wrapFlags)
        {
            wrapFlags = BrgMatFlag.WrapUTx1 | BrgMatFlag.WrapVTx1;

            var tex = srcMaterial.GetDiffuseTexture();
            if (tex?.PrimaryImage == null) return string.Empty;

            if (tex.Sampler != null)
            {
                wrapFlags = 0;
                wrapFlags |= tex.Sampler.WrapS == TextureWrapMode.CLAMP_TO_EDGE ? 0 : BrgMatFlag.WrapUTx1;
                wrapFlags |= tex.Sampler.WrapT == TextureWrapMode.CLAMP_TO_EDGE ? 0 : BrgMatFlag.WrapVTx1;
            }

            string name = tex.PrimaryImage.Name ?? tex.Name ?? srcMaterial.Name ?? $"tex{tex.LogicalIndex}";
            return name;
        }
        private static void GetMaterialFlagsFromName(GltfMaterial glMat, BrgMaterial mat)
        {
            string? flags = glMat.Name?.ToLower();
            if (flags == null) return;

            StringComparison cmp = StringComparison.InvariantCultureIgnoreCase;
            if (flags.Contains("colorxform1", cmp))
            {
                mat.Flags |= BrgMatFlag.PlayerXFormColor1;
            }
            if (flags.Contains("colorxform2", cmp))
            {
                mat.Flags |= BrgMatFlag.PlayerXFormColor2;
            }
            if (flags.Contains("colorxform3", cmp))
            {
                mat.Flags |= BrgMatFlag.PlayerXFormColor3;
            }

            if (flags.Contains("pixelxform1", cmp))
            {
                mat.Flags |= BrgMatFlag.PixelXForm1;
            }
            if (flags.Contains("pixelxform2", cmp))
            {
                mat.Flags |= BrgMatFlag.PixelXForm2;
            }
            if (flags.Contains("pixelxform3", cmp))
            {
                mat.Flags |= BrgMatFlag.PixelXForm3;
            }

            if (flags.Contains("texturexform1", cmp))
            {
                mat.Flags |= BrgMatFlag.PlayerXFormTx1;
            }
            if (flags.Contains("texturexform2", cmp))
            {
                mat.Flags |= BrgMatFlag.PlayerXFormTx2;
            }

            if (flags.Contains("2-sided", cmp) ||
                flags.Contains("2 sided", cmp) ||
                flags.Contains("2sided", cmp))
            {
                mat.Flags |= BrgMatFlag.TwoSided;
            }
        }
        private static int GetDiffuseBaseColorTexCoordSet(GltfMaterial srcMaterial)
        {
            var channel = srcMaterial.FindChannel("Diffuse");
            if (channel.HasValue) return channel.Value.TextureCoordinate;

            channel = srcMaterial.FindChannel("BaseColor");
            if (channel.HasValue) return channel.Value.TextureCoordinate;

            return 0;
        }
    }
}
