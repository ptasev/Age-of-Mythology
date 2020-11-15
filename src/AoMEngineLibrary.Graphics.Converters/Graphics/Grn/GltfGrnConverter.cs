using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AoMEngineLibrary.Graphics.Grn
{
    public class GltfGrnConverter
    {
        private static readonly Quaternion RotX90Quat;
        private static readonly Matrix4x4 RotX90;

        static GltfGrnConverter()
        {
            RotX90 = Matrix4x4.CreateRotationX(MathF.PI * 0.5f);
            RotX90Quat = Quaternion.CreateFromRotationMatrix(RotX90);
        }

        public GltfGrnConverter()
        {

        }

        public GrnFile Convert(ModelRoot gltfFile, GltfGrnParameters parameters)
        {
            GrnFile grn = new GrnFile();
            var model = gltfFile;

            // We'll only export the default scene and first animation in the list
            var anim = model.LogicalAnimations.Count > 0 ? model.LogicalAnimations[0] : null;

            // Get a list of nodes in the default scene as a flat list
            Dictionary<int, int> nodeBoneIndexMap = new Dictionary<int, int>();
            var nodeFlatList = Node.Flatten(model.DefaultScene);
            var nodes = nodeFlatList.ToList();

            ConvertSkeleton(grn, nodes, nodeBoneIndexMap);

            if (parameters.ConvertMeshes)
            {
                ConvertMeshes(grn, model, nodes, nodeBoneIndexMap);
            }

            if (anim != null && parameters.ConvertAnimations)
                ConvertAnimation(grn, nodes, anim, nodeBoneIndexMap);

            return grn;
        }

        public void ConvertSkeleton(GrnFile grn, List<Node> nodes, Dictionary<int, int> nodeBoneIndexMap)
        {
            // Add root
            grn.Bones.Add(new GrnBone(grn));
            grn.Bones[0].DataExtensionIndex = grn.AddDataExtension("__Root");
            grn.Bones[0].Rotation = new Quaternion(0, 0, 0, 1);
            grn.Bones[0].ParentIndex = 0;

            foreach (var node in nodes)
            {
                GrnBone bone = new GrnBone(grn);
                bone.DataExtensionIndex = grn.AddDataExtension(node.Name);
                bone.ParentIndex = node.VisualParent == null ? 0 : nodeBoneIndexMap[node.VisualParent.LogicalIndex];

                if (!Matrix4x4.Decompose(node.LocalMatrix, out Vector3 scale, out Quaternion rot, out Vector3 pos))
                {
                    throw new InvalidDataException($"Node ({node.Name}) local matrix must be decomposable into translation, rotation, and scale.");
                }

                // Rotate all nodes with a parent of 0 to adjust for difference between Grn, and Gltf world space
                if (bone.ParentIndex == 0)
                {
                    bone.Position = Vector3.Transform(pos, RotX90);
                    bone.Rotation = Quaternion.Concatenate(rot, RotX90Quat);
                    bone.Scale = Matrix4x4.CreateScale(scale);
                }
                else
                {
                    bone.Position = pos;
                    bone.Rotation = rot;
                    bone.Scale = Matrix4x4.CreateScale(scale);
                }

                nodeBoneIndexMap.Add(node.LogicalIndex, grn.Bones.Count);
                grn.Bones.Add(bone);
            }
        }

        public void ConvertMeshes(GrnFile grn, ModelRoot gltf, List<Node> nodes, Dictionary<int, int> nodeBoneIndexMap)
        {
            Dictionary<int, int> matIdMapping = new Dictionary<int, int>();

            var meshNodes = nodes.Where(n => n.Mesh != null);
            foreach (var inst in meshNodes)
            {
                var mesh = inst.Mesh;

                if (inst.Skin == null) continue; // throw new InvalidOperationException($"Can't convert gtlf with mesh ({mesh.Name}) without a skin.");

                if (!mesh.AllPrimitivesHaveJoints) throw new InvalidOperationException($"Can't convert gtlf with mesh ({mesh.Name}) primitive without a skin.");

                if (mesh.MorphWeights.Count > 0) throw new InvalidOperationException($"Can't convert gltf with mesh ({mesh.Name}) vertex morphs to grn.");

                if (mesh.Primitives.Any(p => p.Material == null)) throw new NotImplementedException($"The converter does not support primitives ({mesh.Name}) with a null material.");

                // Create grn materials and a way to map their ids
                var gltfMats = mesh.Primitives.Select(p => p.Material);
                ConvertMaterials(gltfMats, grn, matIdMapping);

                // Now add a new mesh from mesh builder
                ConvertMesh(grn, inst, nodeBoneIndexMap, matIdMapping);
            }
        }
        private void ConvertMesh(GrnFile grn, Node meshNode, Dictionary<int, int> nodeBoneIndexMap, Dictionary<int, int> matIdMapping)
        {
            GrnMesh mesh = new GrnMesh(grn);
            mesh.DataExtensionIndex = grn.AddDataExtension(meshNode.Mesh.Name ?? meshNode.Name ?? $"mesh{meshNode.Mesh.LogicalIndex}");

            // Get bone bindings
            var skinTransforms = new Matrix4x4[meshNode.Skin.JointsCount];
            for (int i = 0; i < skinTransforms.Length; ++i)
            {
                // Adjust skin transforms for difference between gltf and grn coord system
                var skinJoint = meshNode.Skin.GetJoint(i);
                skinTransforms[i] = skinJoint.InverseBindMatrix * skinJoint.Joint.WorldMatrix * RotX90;

                var bb = new GrnBoneBinding();
                bb.BoneIndex = nodeBoneIndexMap[skinJoint.Joint.LogicalIndex];
                // TODO: figure these out
                bb.OBBMin = new Vector3(-0.25f);
                bb.OBBMax = new Vector3(0.25f);
                mesh.BoneBindings.Add(bb);
            }

            // Export Vertices, Normals, TexCoords, VertexWeights and Faces
            int baseVertexIndex = 0;
            Mesh gltfMesh = meshNode.Mesh;
            foreach (var p in meshNode.Mesh.Primitives)
            {
                // skip primitives that aren't tris
                if (p.DrawPrimitiveType == PrimitiveType.LINES ||
                    p.DrawPrimitiveType == PrimitiveType.LINE_LOOP ||
                    p.DrawPrimitiveType == PrimitiveType.LINE_STRIP ||
                    p.DrawPrimitiveType == PrimitiveType.POINTS)
                    continue;

                // Make sure we have all the necessary data
                var vertexAccessors = p.VertexAccessors;
                if (!vertexAccessors.ContainsKey("POSITION")) throw new InvalidDataException($"Mesh ({gltfMesh.Name}) must have positions.");
                if (!vertexAccessors.ContainsKey("NORMAL")) throw new InvalidDataException($"Mesh ({gltfMesh.Name}) must have normals.");

                if (!vertexAccessors.ContainsKey("TEXCOORD_0")) throw new InvalidDataException($"Mesh ({gltfMesh.Name}) must have tex coord set.");
                if (vertexAccessors.ContainsKey("TEXCOORD_1")) throw new InvalidOperationException($"Can't convert mesh ({gltfMesh.Name}) with more than one set of texcoords.");

                if (!vertexAccessors.ContainsKey("JOINTS_0")) throw new InvalidDataException($"Mesh ({gltfMesh.Name}) must have a set of joints.");
                if (vertexAccessors.ContainsKey("JOINTS_1")) throw new InvalidOperationException($"Can't convert mesh ({gltfMesh.Name}) with more than one set of joints.");

                if (!vertexAccessors.ContainsKey("WEIGHTS_0")) throw new InvalidDataException($"Mesh ({gltfMesh.Name}) must have a set of weights.");
                if (vertexAccessors.ContainsKey("WEIGHTS_1")) throw new InvalidOperationException($"Can't convert mesh ({gltfMesh.Name}) with more than one set of joint weights.");

                // Grab the data
                var positions = vertexAccessors["POSITION"].AsVector3Array();
                var normals = vertexAccessors["NORMAL"].AsVector3Array();
                var texCoords = vertexAccessors["TEXCOORD_0"].AsVector2Array();
                var joints = vertexAccessors["JOINTS_0"].AsVector4Array();
                var weights = vertexAccessors["WEIGHTS_0"].AsVector4Array();

                if (positions.Count < 3) throw new InvalidDataException($"Mesh ({gltfMesh.Name}) must have at least 3 positions.");

                for (int i = 0; i < positions.Count; ++i)
                {
                    var pos = positions[i];
                    var norm = normals[i];

                    GrnVertexWeight vw = new GrnVertexWeight();
                    var finPos = Vector3.Zero;
                    var finNorm = Vector3.Zero;
                    var sw = new SparseWeight8(joints[i], weights[i]);
                    var wNorm = 1.0f / sw.WeightSum;
                    foreach (var iw in sw.GetIndexedWeights())
                    {
                        if (iw.Weight <= 0) continue;

                        var normalizedWeight = (iw.Weight / wNorm);
                        finPos += Vector3.Transform(pos, skinTransforms[iw.Index]) * normalizedWeight;
                        finNorm += Vector3.TransformNormal(norm, skinTransforms[iw.Index]) * normalizedWeight;
                        vw.BoneIndices.Add(iw.Index);
                        vw.Weights.Add(normalizedWeight);
                    }

                    finNorm = Vector3.Normalize(finNorm);
                    mesh.Vertices.Add(finPos);
                    mesh.Normals.Add(finNorm);
                    mesh.TextureCoordinates.Add(new Vector3(texCoords[i], 0));
                    mesh.VertexWeights.Add(vw);
                }

                // Get the new material index in grn
                int faceMatId = p.Material.LogicalIndex;
                if (matIdMapping.ContainsKey(faceMatId))
                {
                    faceMatId = (Int16)matIdMapping[faceMatId];
                }
                else
                {
                    throw new InvalidDataException("A face has an invalid material id " + faceMatId + ".");
                }

                foreach (var tri in p.GetTriangleIndices())
                {
                    var a = tri.A + baseVertexIndex;
                    var b = tri.B + baseVertexIndex;
                    var c = tri.C + baseVertexIndex;

                    GrnFace face = new GrnFace();
                    face.MaterialIndex = faceMatId;
                    face.Indices.Add(a);
                    face.Indices.Add(b);
                    face.Indices.Add(c);
                    face.NormalIndices.Add(a);
                    face.NormalIndices.Add(b);
                    face.NormalIndices.Add(c);
                    face.TextureIndices.Add(a);
                    face.TextureIndices.Add(b);
                    face.TextureIndices.Add(c);
                    mesh.Faces.Add(face);
                }

                baseVertexIndex += positions.Count;
            }

            if (mesh.Faces.Count > 0 && mesh.Vertices.Count > 0)
                grn.Meshes.Add(mesh);
        }

        private void ConvertAnimation(GrnFile grn, List<Node> nodes, Animation gltfAnim, Dictionary<int, int> nodeBoneIndexMap)
        {
            if (gltfAnim.Duration == 0) return;
            var anim = grn.Animation;
            anim.Duration = gltfAnim.Duration;

            // Add root bone track with first and last keys
            GrnBoneTrack rootBoneTrack = new GrnBoneTrack(grn);
            rootBoneTrack.DataExtensionIndex = grn.Bones[0].DataExtensionIndex;
            rootBoneTrack.PositionKeys.Add(0); rootBoneTrack.PositionKeys.Add(anim.Duration);
            rootBoneTrack.RotationKeys.Add(0); rootBoneTrack.RotationKeys.Add(anim.Duration);
            rootBoneTrack.ScaleKeys.Add(0); rootBoneTrack.ScaleKeys.Add(anim.Duration);
            rootBoneTrack.Positions.Add(grn.Bones[0].Position); rootBoneTrack.Positions.Add(grn.Bones[0].Position);
            rootBoneTrack.Rotations.Add(grn.Bones[0].Rotation); rootBoneTrack.Rotations.Add(grn.Bones[0].Rotation);
            rootBoneTrack.Scales.Add(grn.Bones[0].Scale); rootBoneTrack.Scales.Add(grn.Bones[0].Scale);
            anim.BoneTracks.Add(rootBoneTrack);

            for (int i = 0; i < nodes.Count; ++i)
            {
                var node = nodes[i];
                var grnBone = grn.Bones[nodeBoneIndexMap[node.LogicalIndex]];
                // Adjust for difference between grn and gltf coord systems
                bool adjustCoordSystem = grnBone.ParentIndex == 0;

                var boneTrack = new GrnBoneTrack(grn);
                boneTrack.DataExtensionIndex = grnBone.DataExtensionIndex;

                // Get translation animation data
                var translationSampler = gltfAnim.FindTranslationSampler(node);
                List<(float, Vector3)>? keys = null;
                if (translationSampler != null)
                    keys = translationSampler.InterpolationMode == AnimationInterpolationMode.CUBICSPLINE ?
                        translationSampler.GetCubicKeys().Select(k => (k.Key, k.Item2.Value)).ToList() :
                        translationSampler.GetLinearKeys().ToList();
                if (keys == null || keys.Count == 0)
                {
                    boneTrack.PositionKeys.Add(0);
                    boneTrack.Positions.Add(grnBone.Position);
                    boneTrack.PositionKeys.Add(anim.Duration);
                    boneTrack.Positions.Add(grnBone.Position);
                }
                else
                {
                    foreach (var key in keys)
                    {
                        boneTrack.PositionKeys.Add(key.Item1);
                        boneTrack.Positions.Add(adjustCoordSystem ? Vector3.Transform(key.Item2, RotX90) : key.Item2);
                    }
                }

                // Get rotation animation data
                var rotationSampler = gltfAnim.FindRotationSampler(node);
                List<(float, Quaternion)>? rotKeys = null;
                if (rotationSampler != null)
                    rotKeys = rotationSampler.InterpolationMode == AnimationInterpolationMode.CUBICSPLINE ?
                        rotationSampler.GetCubicKeys().Select(k => (k.Key, k.Item2.Value)).ToList() :
                        rotationSampler.GetLinearKeys().ToList();
                if (rotKeys == null || rotKeys.Count == 0)
                {
                    boneTrack.RotationKeys.Add(0);
                    boneTrack.Rotations.Add(grnBone.Rotation);
                    boneTrack.RotationKeys.Add(anim.Duration);
                    boneTrack.Rotations.Add(grnBone.Rotation);
                }
                else
                {
                    foreach (var key in rotKeys)
                    {
                        boneTrack.RotationKeys.Add(key.Item1);
                        boneTrack.Rotations.Add(adjustCoordSystem ? Quaternion.Concatenate(key.Item2, RotX90Quat) : key.Item2);
                    }
                }

                // Get scale animation data
                var scaleSampler = gltfAnim.FindScaleSampler(node);
                List<(float, Matrix4x4)>? scaleKeys = null;
                if (scaleSampler != null)
                    scaleKeys = scaleSampler.InterpolationMode == AnimationInterpolationMode.CUBICSPLINE ?
                        scaleSampler.GetCubicKeys().Select(k => (k.Key, Matrix4x4.CreateScale(k.Item2.Value))).ToList() :
                        scaleSampler.GetLinearKeys().Select(k => (k.Key, Matrix4x4.CreateScale(k.Value))).ToList();
                if (scaleKeys == null || scaleKeys.Count == 0)
                {
                    boneTrack.ScaleKeys.Add(0);
                    boneTrack.Scales.Add(grnBone.Scale);
                    boneTrack.ScaleKeys.Add(anim.Duration);
                    boneTrack.Scales.Add(grnBone.Scale);
                }
                else
                {
                    foreach (var key in scaleKeys)
                    {
                        boneTrack.ScaleKeys.Add(key.Item1);
                        boneTrack.Scales.Add(key.Item2);
                    }
                }

                anim.BoneTracks.Add(boneTrack);
            }
        }

        private void ConvertMaterials(IEnumerable<Material> gltfMats, GrnFile grn, Dictionary<int, int> matIdMapping)
        {
            foreach (var gltfMat in gltfMats)
            {
                if (gltfMat == null) continue;

                GrnMaterial mat = new GrnMaterial(grn);
                int id = grn.Materials.Count;
                ConvertMaterial(gltfMat, mat, grn);

                int matListIndex = grn.Materials.IndexOf(mat);
                int actualMatId = gltfMat.LogicalIndex;
                if (matListIndex >= 0)
                {
                    if (!matIdMapping.ContainsKey(actualMatId))
                    {
                        matIdMapping.Add(actualMatId, matListIndex);
                    }
                    grn.DataExtensions.RemoveAt(mat.DataExtensionIndex);
                }
                else
                {
                    grn.Materials.Add(mat);
                    if (matIdMapping.ContainsKey(actualMatId))
                    {
                        matIdMapping[actualMatId] = id;
                    }
                    else
                    {
                        matIdMapping.Add(actualMatId, id);
                    }
                }
            }
        }
        private void ConvertMaterial(Material gltfMat, GrnMaterial mat, GrnFile grn)
        {
            mat.DataExtensionIndex = grn.AddDataExtension(gltfMat.Name ?? $"mat{gltfMat.LogicalIndex}");

            GrnTexture? tex = null;
            var diffuseTextureName = GetDiffuseTexture(gltfMat);
            if (!string.IsNullOrEmpty(diffuseTextureName))
            {
                tex = new GrnTexture(grn);
                tex.DataExtensionIndex = grn.AddDataExtension(diffuseTextureName);
                grn.SetDataExtensionFileName(tex.DataExtensionIndex, $"{diffuseTextureName}.tga");
            }

            if (tex != null)
            {
                int texIndex = grn.Textures.IndexOf(tex);
                if (texIndex >= 0)
                {
                    mat.DiffuseTextureIndex = texIndex;
                    grn.DataExtensions.RemoveAt(tex.DataExtensionIndex);
                }
                else
                {
                    mat.DiffuseTextureIndex = grn.Textures.Count;
                    grn.Textures.Add(tex);
                    string fileNameNoExt = Path.GetFileNameWithoutExtension(tex.FileName);
                    if (!fileNameNoExt.Equals(tex.Name))
                    {
                        throw new InvalidDataException("Texture name " + tex.Name + " must be set to " + fileNameNoExt + ".");
                    }
                }
            }
        }
        private string GetDiffuseTexture(Material srcMaterial)
        {
            var tex = srcMaterial.GetDiffuseTexture();
            if (tex == null) return string.Empty;
            if (tex.PrimaryImage == null) return string.Empty;

            string name = tex.PrimaryImage.Name ?? tex.Name ?? srcMaterial.Name ?? $"tex{tex.LogicalIndex}";
            return name;
        }
    }
}
