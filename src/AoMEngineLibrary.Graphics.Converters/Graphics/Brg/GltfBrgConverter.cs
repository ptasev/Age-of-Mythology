using SharpGLTF.Runtime;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

using GltfMesh = SharpGLTF.Schema2.Mesh;
using GltfMaterial = SharpGLTF.Schema2.Material;
using GltfAnimation = SharpGLTF.Schema2.Animation;
using GltfMeshBuilder = SharpGLTF.Geometry.MeshBuilder<
    AoMEngineLibrary.Graphics.Brg.NullableGltfMaterial,
    SharpGLTF.Geometry.VertexTypes.VertexPositionNormal,
    SharpGLTF.Geometry.VertexTypes.VertexColor1Texture1,
    SharpGLTF.Geometry.VertexTypes.VertexEmpty>;
using GltfPrimitiveBuilder = SharpGLTF.Geometry.PrimitiveBuilder<
    AoMEngineLibrary.Graphics.Brg.NullableGltfMaterial,
    SharpGLTF.Geometry.VertexTypes.VertexPositionNormal,
    SharpGLTF.Geometry.VertexTypes.VertexColor1Texture1,
    SharpGLTF.Geometry.VertexTypes.VertexEmpty>;
using System.IO;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Geometry;

namespace AoMEngineLibrary.Graphics.Brg
{
    internal struct NullableGltfMaterial
    {
        public GltfMaterial m;
    }

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

            var sceneTemplate = SceneTemplate.Create(gltfScene, false);
            var instance = sceneTemplate.CreateInstance();

            Predicate<string> noDummySelector = nodeName => !nodeName.StartsWith("dummy_", StringComparison.InvariantCultureIgnoreCase);

            // Mesh Animations
            var gltfMeshes = new List<GltfMeshData>();
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

                // Evaluate the entire gltf scene
                var gltfMeshData = GetMeshBuilder(gltfScene, instance, noDummySelector);
                gltfMeshes.Add(gltfMeshData);

                // Add a new mesh
                var mesh = new BrgMesh(brg);
                mesh.Header.Format = BrgMeshFormat.HASFACENORMALS | BrgMeshFormat.ANIMATED;
                mesh.ExtendedHeader.AnimationLength = brg.Animation.Duration;
                if (i > 0)
                {
                    mesh.Header.Flags |= BrgMeshFlag.SECONDARYMESH;
                }
                brg.Meshes.Add(mesh);

                // Convert all the attachpoints in the scene
                ConvertAttachpoints(gltfScene, instance, mesh);
            }

            // Calculate face indices for each primitve
            var primitveFaces = CalculateFaces(gltfMeshes);

            // Convert
            for (int i = 0; i < brg.Meshes.Count; ++i)
            {
                // We only care about primitves with a material
                var gltfMeshData = gltfMeshes[i];
                var prims = gltfMeshData.MeshBuilder.Primitives.Where(p => p.Material.m != null);

                // Create brg materials and a way to map their ids
                var gltfMats = prims.Select(p => p.Material.m);
                ConvertMaterials(gltfMats, brg, matIdMapping);

                // ConvertMesh will set proper flags to the brg mesh which we then want to validate
                var mesh = brg.Meshes[i];
                ConvertMesh(prims.ToList(), gltfMeshData.HasTexCoords, mesh, matIdMapping, primitveFaces);
                brg.UpdateMeshSettings(i, brg.Meshes[0].Header.Flags, brg.Meshes[0].Header.Format, brg.Meshes[0].Header.AnimationType, brg.Meshes[0].Header.InterpolationType);
            }
        }

        private GltfMeshData GetMeshBuilder(Scene scene, SceneInstance instance, Predicate<string> nodeNameSelector)
        {
            var meshes = scene.LogicalParent.LogicalMeshes;

            var tris = instance
                .DrawableInstances
                .Where(d => nodeNameSelector(d.Template.NodeName))
                .SelectMany(item => meshes[item.Template.LogicalMeshIndex].EvaluateTriangles(item.Transform));

            var mb = new GltfMeshBuilder();
            bool hasTexCoords = false;
            foreach (var tri in tris)
            {
                if (!hasTexCoords)
                {
                    hasTexCoords = tri.A.GetMaterial().MaxTextCoords > 0;
                }

                var a = SetMaterialTexCoordToSlot0(tri.A, tri.Material);
                var b = SetMaterialTexCoordToSlot0(tri.B, tri.Material);
                var c = SetMaterialTexCoordToSlot0(tri.C, tri.Material);
                var mat = new NullableGltfMaterial() { m = tri.Material };
                mb.UsePrimitive(mat).AddTriangle(a, b, c);
            }

            return new GltfMeshData(mb, hasTexCoords);

            static IVertexBuilder SetMaterialTexCoordToSlot0(IVertexBuilder vb, GltfMaterial mat)
            {
                // This func will get the diffuse/base color tex coordinate index
                // and set that to slot 0 in the vertex material
                if (mat is null) return vb;

                var vbMat = vb.GetMaterial();
                if (vbMat.MaxTextCoords <= 1) return vb;

                var texIndex = GetDiffuseBaseColorTexCoord(mat);
                if (texIndex == 0) return vb;

                // It seems the tex index is higher than the max coordinate stored in the tex coord
                // ignore for now without warning or error (in the future at least log as warning)
                if (texIndex >= vbMat.MaxTextCoords) return vb;

                // Place the tex coord at mat tex index into slot 0.
                vbMat.SetTexCoord(0, vbMat.GetTexCoord(texIndex));
                vb.SetMaterial(vbMat);

                return vb;

                static int GetDiffuseBaseColorTexCoord(GltfMaterial srcMaterial)
                {
                    var channel = srcMaterial.FindChannel("Diffuse");
                    if (channel.HasValue) return channel.Value.TextureCoordinate;

                    channel = srcMaterial.FindChannel("BaseColor");
                    if (channel.HasValue) return channel.Value.TextureCoordinate;

                    return 0;
                }
            }
        }

        private List<List<(int A, int B, int C)>> CalculateFaces(List<GltfMeshData> gltfMeshes)
        {
            var primFaces = new List<List<(int A, int B, int C)>>();

            var primitiveCount = gltfMeshes.Count > 0 ? gltfMeshes[0].MeshBuilder.Primitives.Count : 0;
            for (int p = 0; p < primitiveCount; ++p)
            {
                // Get a list of faces for each frame from primitive p
                var faceLists = new List<List<(int A, int B, int C)>>();
                foreach (var gltfMesh in gltfMeshes)
                {
                    var pf = gltfMesh.MeshBuilder.Primitives.ElementAt(p);
                    if (pf.Material.m == null) break;
                    faceLists.Add(pf.Triangles.ToList());
                }

                if (faceLists.Count <= 0) continue;
                primFaces.Add(CalculatePrimitiveFaces(faceLists));
            }

            return primFaces;
        }
        private List<(int A, int B, int C)> CalculatePrimitiveFaces(List<List<(int A, int B, int C)>> faceLists)
        {
            // test data
            //List<(int A, int B, int C)> one = new List<(int A, int B, int C)>();
            //one.Add((0, 1, 2)); one.Add((2, 1, 3));
            //List<(int A, int B, int C)> two = new List<(int A, int B, int C)>();
            //two.Add((0, 1, 2)); two.Add((2, 1, 3)); //two.Add((4, 3, 5));

            //List<List<(int A, int B, int C)>> faceLists = new List<List<(int A, int B, int C)>>();
            //faceLists.Add(one); faceLists.Add(two);

            // Create a new face list that can be used across all frames for primitive p
            int uniqueVerts = 0;
            int faceCount = faceLists[0].Count;
            var frameVertexHistory = Enumerable.Range(0, faceLists.Count).Select(x => new Dictionary<int, (int, int)>()).ToList();
            var newIndices = new List<(int A, int B, int C)>();
            for (int i = 0; i < faceCount; ++i)
            {
                (int A, int B, int C) newFace = (0, 0, 0);

                bool aUnique = false;
                var aMatch = (-1, -1); // face, corner
                bool bUnique = false;
                var bMatch = (-1, -1); // face, corner
                bool cUnique = false;
                var cMatch = (-1, -1); // face, corner
                for (int j = 0; j < faceLists.Count; ++j)
                {
                    // get face i from frame j
                    var face = faceLists[j][i];
                    var vertHistory = frameVertexHistory[j];

                    // Ignore faces that are invalid
                    if (face.A == -1) continue;

                    // Peform actions for corner A/0
                    if (vertHistory.ContainsKey(face.A))
                    {
                        // we've found a vertex appear in a previous face for this frame
                        if (!aUnique)
                        {
                            // only do these checks if we haven't already determined to be unique
                            if (aMatch.Item1 == -1)
                            {
                                // this is the first frame, store for other frames to compare
                                aMatch = vertHistory[face.A];
                            }
                            else if (aMatch.CompareTo(vertHistory[face.A]) != 0)
                            {
                                // we didn't match the face and corner with another frame
                                aUnique = true;
                            }
                        }
                    }
                    else
                    {
                        // this vert never appeared in a previous face for this frame
                        vertHistory.Add(face.A, (i, 0));
                        aUnique = true;
                    }

                    // Peform actions for corner B/1
                    if (vertHistory.ContainsKey(face.B))
                    {
                        // we've found a vertex appear in a previous face for this frame
                        if (!bUnique)
                        {
                            // only do these checks if we haven't already determined to be unique
                            if (bMatch.Item1 == -1)
                            {
                                // this is the first frame, store for other frames to compare
                                bMatch = vertHistory[face.B];
                            }
                            else if (bMatch.CompareTo(vertHistory[face.B]) != 0)
                            {
                                // we didn't match the face and corner with another frame
                                bUnique = true;
                            }
                        }
                    }
                    else
                    {
                        // this vert never appeared in a previous face for this frame
                        vertHistory.Add(face.B, (i, 1));
                        bUnique = true;
                    }

                    // Peform actions for corner C/2
                    if (vertHistory.ContainsKey(face.C))
                    {
                        // we've found a vertex appear in a previous face for this frame
                        if (!cUnique)
                        {
                            // only do these checks if we haven't already determined to be unique
                            if (cMatch.Item1 == -1)
                            {
                                // this is the first frame, store for other frames to compare
                                cMatch = vertHistory[face.C];
                            }
                            else if (cMatch.CompareTo(vertHistory[face.C]) != 0)
                            {
                                // we didn't match the face and corner with another frame
                                cUnique = true;
                            }
                        }
                    }
                    else
                    {
                        // this vert never appeared in a previous face for this frame
                        vertHistory.Add(face.C, (i, 2));
                        cUnique = true;
                    }
                }

                // If none of the frames had a valid face ignore it completely
                if (aUnique || aMatch.Item1 != -1)
                {
                    if (aUnique)
                    {
                        // Create a new vert
                        newFace.A = uniqueVerts;
                        ++uniqueVerts;
                    }
                    else
                    {
                        // Use vert from previous face
                        var oldFace = newIndices[aMatch.Item1];
                        newFace.A = aMatch.Item2 switch
                        {
                            0 => oldFace.A,
                            1 => oldFace.B,
                            2 => oldFace.C,
                            _ => throw new Exception("Failed to optimize face list.")
                        };
                    }

                    if (bUnique)
                    {
                        // Create a new vert
                        newFace.B = uniqueVerts;
                        ++uniqueVerts;
                    }
                    else
                    {
                        // Use vert from previous face
                        var oldFace = newIndices[bMatch.Item1];
                        newFace.B = bMatch.Item2 switch
                        {
                            0 => oldFace.A,
                            1 => oldFace.B,
                            2 => oldFace.C,
                            _ => throw new Exception("Failed to optimize face list.")
                        };
                    }

                    if (cUnique)
                    {
                        // Create a new vert
                        newFace.C = uniqueVerts;
                        ++uniqueVerts;
                    }
                    else
                    {
                        // Use vert from previous face
                        var oldFace = newIndices[cMatch.Item1];
                        newFace.C = cMatch.Item2 switch
                        {
                            0 => oldFace.A,
                            1 => oldFace.B,
                            2 => oldFace.C,
                            _ => throw new Exception("Failed to optimize face list.")
                        };
                    }

                    newIndices.Add(newFace);
                }
            }

            return newIndices;
        }

        private void ConvertMesh(List<GltfPrimitiveBuilder> primitives, bool hasTexCoords, BrgMesh mesh, Dictionary<int, int> matIdMapping, List<List<(int A, int B, int C)>> primFaces)
        {
            mesh.Header.Version = 22;
            mesh.Header.ExtendedHeaderSize = 40;

            // Create new vert list based on prim faces
            var primVertCounts = primFaces.Select(p => p.Max(f => Math.Max(f.A, Math.Max(f.B, f.C))) + 1).ToList();
            var totalVerts = primVertCounts.Sum();
            var vertices = new VertexBuilder<VertexPositionNormal, VertexColor1Texture1, VertexEmpty>[totalVerts];
            HashSet<int> traversedIndices = new HashSet<int>();
            int vertOffset = 0;
            for (int i = 0; i < primitives.Count; ++i)
            {
                var p = primitives[i];
                var faces = primFaces[i];

                for (int j = 0; j < faces.Count; ++j)
                {
                    var of = p.Triangles[j];
                    var nf = faces[j];

                    // if any is -1 all are -1
                    if (of.A == -1) continue;
                    var na = nf.A + vertOffset;
                    if (!traversedIndices.Contains(na))
                    {
                        traversedIndices.Add(na);
                        var vert = p.Vertices[of.A];
                        vertices[na] = vert;
                    }

                    var nb = nf.B + vertOffset;
                    if (!traversedIndices.Contains(nb))
                    {
                        traversedIndices.Add(nb);
                        var vert = p.Vertices[of.B];
                        vertices[nb] = vert;
                    }

                    var nc = nf.C + vertOffset;
                    if (!traversedIndices.Contains(nc))
                    {
                        traversedIndices.Add(nc);
                        var vert = p.Vertices[of.C];
                        vertices[nc] = vert;
                    }
                }

                vertOffset += primVertCounts[i];
            }

            // Export Vertices and Normals
            Vector3 centerPos = new Vector3();
            Vector3 minExtent = new Vector3(float.MaxValue);
            Vector3 maxExtent = new Vector3(float.MinValue);
            foreach (var v in vertices)
            {
                var pos = v.Position;
                var norm = v.Geometry.Normal;
                mesh.Vertices.Add(new Vector3(-pos.X, pos.Y, pos.Z));
                mesh.Normals.Add(new Vector3(-norm.X, norm.Y, norm.Z));

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
            mesh.Header.CenterPosition = new Vector3(-centerPos.X, centerPos.Y, centerPos.Z);
            mesh.Header.MinimumExtent = -bbox;
            mesh.Header.MaximumExtent = bbox;
            mesh.Header.CenterRadius = Math.Max(Math.Max(bbox.X, bbox.Y), bbox.Z);

            // Check if MeshBuilder has texcoords
            //if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || mesh.Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
            //{
            //    hasTexCoords = gltfMesh.Primitives.Any(p => p.Vertices.Any(v => v.Material.TexCoord != Vector2.Zero));
            //}

            // Export TexCoords
            if (hasTexCoords)
            {
                mesh.Header.Flags |= BrgMeshFlag.TEXCOORDSA;
                foreach (var v in vertices)
                {
                    var tc = v.Material.TexCoord;
                    mesh.TextureCoordinates.Add(new Vector2(tc.X, 1 - tc.Y));
                }
            }

            // Check if MeshBuilder has colors
            // TODO: Add support for vertex colors
            //bool hasColors = gltfMesh.Primitives.Any(p => p.Vertices.Any(v => v.Material.Color != Vector4.Zero));

            // Export faces
            if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
            {
                mesh.Header.Flags |= BrgMeshFlag.MATERIAL;
                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                {
                    mesh.VertexMaterials.AddRange(new Int16[mesh.Vertices.Count]);
                }

                int baseVertexIndex = 0;
                for (int i = 0; i < primitives.Count; ++i)
                {
                    var p = primitives[i];
                    var faces = primFaces[i];

                    short faceMatIndex = 0;
                    if (mesh.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        int faceMatId = p.Material.m.LogicalIndex;
                        if (matIdMapping.ContainsKey(faceMatId))
                        {
                            faceMatIndex = (Int16)matIdMapping[faceMatId];
                        }
                        else
                        {
                            throw new InvalidDataException("A face has an invalid material id " + faceMatId + ".");
                        }
                    }

                    foreach (var t in faces)
                    {
                        var a = t.A + baseVertexIndex;
                        var b = t.B + baseVertexIndex;
                        var c = t.C + baseVertexIndex;

                        BrgFace f = new BrgFace();
                        mesh.Faces.Add(f);
                        f.Indices.Add((Int16)a);
                        f.Indices.Add((Int16)c);
                        f.Indices.Add((Int16)b);
                        f.MaterialIndex = faceMatIndex;

                        if (mesh.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                        {
                            mesh.VertexMaterials[f.Indices[0]] = f.MaterialIndex;
                            mesh.VertexMaterials[f.Indices[1]] = f.MaterialIndex;
                            mesh.VertexMaterials[f.Indices[2]] = f.MaterialIndex;
                        }
                    }

                    baseVertexIndex += p.Vertices.Count;
                }
            }

            mesh.Header.NumFaces = (short)mesh.Faces.Count;
        }

        private void ConvertAttachpoints(Scene scene, SceneInstance sceneInstance, BrgMesh mesh)
        {
            // Convert Attachpoints
            var dummies = sceneInstance.Armature.LogicalNodes.Where(n => n.Name.StartsWith("dummy_", StringComparison.InvariantCultureIgnoreCase));

            if (dummies.Any())
            {
                mesh.Header.Flags |= BrgMeshFlag.ATTACHPOINTS;
                foreach (var dummy in dummies)
                {
                    string aName = dummy.Name;
                    if (!BrgAttachpoint.TryGetIdByName(aName.Substring(6), out int nameId))
                    {
                        // Check if it's hotspot
                        if (aName.Equals("Dummy_hotspot", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var trans = dummy.ModelMatrix.Translation;
                            mesh.Header.HotspotPosition = new Vector3(-trans.X, trans.Y, trans.Z);
                        }
                        continue;
                    }

                    BrgAttachpoint att = new BrgAttachpoint();
                    att.NameId = nameId;
                    var transform = dummy.ModelMatrix;

                    att.Up.X = transform.M21;
                    att.Up.Y = transform.M22;
                    att.Up.Z = transform.M23;

                    att.Forward.X = transform.M31;
                    att.Forward.Y = transform.M32;
                    att.Forward.Z = transform.M33;

                    att.Right.X = transform.M11;
                    att.Right.Y = transform.M12;
                    att.Right.Z = transform.M13;

                    att.Position.X = -transform.M41;
                    att.Position.Y = transform.M42;
                    att.Position.Z = transform.M43;

                    // TODO: Calculate dummy bounding box
                    var bBox = new Vector3(0.5f, 0.5f, 0.5f);
                    att.BoundingBoxMin.X = -bBox.X;
                    att.BoundingBoxMin.Z = -bBox.Y;
                    att.BoundingBoxMin.Y = -bBox.Z;
                    att.BoundingBoxMax.X = bBox.X;
                    att.BoundingBoxMax.Z = bBox.Y;
                    att.BoundingBoxMax.Y = bBox.Z;

                    mesh.Attachpoints.Add(att);
                }
            }
        }

        private void ConvertMaterials(IEnumerable<GltfMaterial> gltfMats, BrgFile brg, Dictionary<int, int> matIdMapping)
        {
            foreach (var gltfMat in gltfMats)
            {
                if (gltfMat == null) continue;

                BrgMaterial mat = new BrgMaterial(brg);
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

        private void ConvertMaterial(GltfMaterial glMat, BrgMaterial mat)
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

            //int opacityType = Maxscript.QueryInteger("mat.opacityType");
            //if (opacityType == 1)
            //{
            //    mat.Flags |= BrgMatFlag.SubtractiveBlend;
            //}
            //else if (opacityType == 2)
            //{
            //    mat.Flags |= BrgMatFlag.AdditiveBlend;
            //}

            if (glMat.DoubleSided)
            {
                mat.Flags |= BrgMatFlag.TwoSided;
            }

            //if (Maxscript.QueryBoolean("mat.faceMap"))
            //{
            //    mat.Flags |= BrgMatFlag.FaceMap;
            //}

            //if (Maxscript.QueryBoolean("(classof mat.reflectionMap) == BitmapTexture"))
            //{
            //    mat.Flags |= BrgMatFlag.WrapUTx1 | BrgMatFlag.WrapVTx1 | BrgMatFlag.REFLECTIONTEXTURE;
            //    BrgMatSFX sfxMap = new BrgMatSFX();
            //    sfxMap.Id = 30;
            //    sfxMap.Name = Maxscript.QueryString("getFilenameFile(mat.reflectionMap.filename)") + ".cub";
            //    mat.sfx.Add(sfxMap);
            //}
            //if (Maxscript.QueryBoolean("(classof mat.bumpMap) == BitmapTexture"))
            //{
            //    mat.BumpMap = Maxscript.QueryString("getFilenameFile(mat.bumpMap.filename)");
            //    if (mat.BumpMap.Length > 0)
            //    {
            //        mat.Flags |= BrgMatFlag.WrapUTx3 | BrgMatFlag.WrapVTx3 | BrgMatFlag.BumpMap;
            //    }
            //}
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

            this.GetMaterialFlagsFromName(glMat, mat);
        }
        private static Vector3 GetDiffuseColor(GltfMaterial srcMaterial)
        {
            var diffuse = srcMaterial.GetDiffuseColor(Vector4.One);
            return new Vector3(diffuse.X, diffuse.Y, diffuse.Z);
        }
        private static Vector3 GetSpecularColor(GltfMaterial srcMaterial)
        {
            var mr = srcMaterial.FindChannel("MetallicRoughness");
            if (!mr.HasValue) return Vector3.Zero;

            var diffuse = GetDiffuseColor(srcMaterial);
            var metallic = mr.Value.Parameter.X;
            var roughness = mr.Value.Parameter.Y;

            var k = Vector3.Zero;
            k += Vector3.Lerp(diffuse, Vector3.Zero, roughness);
            k += Vector3.Lerp(diffuse, Vector3.One, metallic);
            k *= 0.5f;
            return k;
        }
        private static Vector3 GetEmissiveColor(GltfMaterial srcMaterial)
        {
            var emissive = srcMaterial.FindChannel("Emissive");
            if (!emissive.HasValue) return Vector3.Zero;
            return new Vector3(emissive.Value.Parameter.X, emissive.Value.Parameter.Y, emissive.Value.Parameter.Z);
        }
        private static float GetSpecularPower(GltfMaterial srcMaterial)
        {
            var mr = srcMaterial.FindChannel("MetallicRoughness");
            if (!mr.HasValue) return 0;

            var metallic = mr.Value.Parameter.X;
            var roughness = mr.Value.Parameter.Y;
            var mult = metallic - roughness;

            return mult <= 0 ? 0 : 25 * mult;
        }
        private static float GetAlphaLevel(GltfMaterial srcMaterial)
        {
            if (srcMaterial.Alpha == AlphaMode.OPAQUE) return 1;

            var baseColor = srcMaterial.FindChannel("BaseColor");

            if (baseColor == null) return 1;

            return baseColor.Value.Parameter.W;
        }
        private string GetDiffuseTexture(GltfMaterial srcMaterial, out BrgMatFlag wrapFlags)
        {
            wrapFlags = BrgMatFlag.WrapUTx1 | BrgMatFlag.WrapVTx1;

            var tex = srcMaterial.GetDiffuseTexture();
            if (tex == null) return string.Empty;
            if (tex.PrimaryImage == null) return string.Empty;

            if (tex.Sampler != null)
            {
                wrapFlags = 0;
                wrapFlags |= tex.Sampler.WrapS == TextureWrapMode.CLAMP_TO_EDGE ? 0 : BrgMatFlag.WrapUTx1;
                wrapFlags |= tex.Sampler.WrapT == TextureWrapMode.CLAMP_TO_EDGE ? 0 : BrgMatFlag.WrapVTx1;
            }

            string name = tex.PrimaryImage.Name ?? tex.Name ?? srcMaterial.Name ?? $"tex{tex.LogicalIndex}";
            return name;
        }
        private void GetMaterialFlagsFromName(GltfMaterial glMat, BrgMaterial mat)
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
    }
}
