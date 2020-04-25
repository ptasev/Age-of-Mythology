using AoMEngineLibrary.Graphics.Grn;
using SharpGLTF.Runtime;
using SharpGLTF.Schema2;
using SharpGLTF.Transforms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AoMModelViewer
{
    public class GltfGrnConverter
    {
        public GltfGrnConverter()
        {

        }

        public GrnFile Convert(ModelRoot gltfFile)
        {
            GrnFile grn = new GrnFile();
            var model = gltfFile;

            // We'll only export the default scene and first animation in the list
            var scene = model.DefaultScene;
            var anim = model.LogicalAnimations.Count > 0 ? model.LogicalAnimations[0] : null;
            var nodes = Node.Flatten(scene).ToList();

            var sceneTemplate = SceneTemplate.Create(scene, false);
            var instance = sceneTemplate.CreateInstance();

            ConvertSkeleton(grn, nodes);

            return grn;
        }

        public void ConvertSkeleton(GrnFile grn, List<Node> nodes)
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
                bone.ParentIndex = node.VisualParent == null ? 0 : nodes.IndexOf(node.VisualParent) + 1;

                // TODO: rotate all nodes with a parent of 0 to adjust for difference between Grn, and Gltf world space
                if (!Matrix4x4.Decompose(node.LocalMatrix, out Vector3 scale, out Quaternion rot, out Vector3 pos))
                {
                    throw new InvalidDataException($"Node ({node.Name}) local matrix must be decomposable into translation, rotation, and scale.");
                }

                bone.Position = pos;
                bone.Rotation = rot;
                bone.Scale = Matrix4x4.CreateScale(scale);

                grn.Bones.Add(bone);
            }
        }

        public void ConvertMeshes(GrnFile grn, ModelRoot gltf, SceneInstance scene, List<Node> nodes)
        {
            Dictionary<int, int> matIdMapping = new Dictionary<int, int>();

            var meshNodes = nodes.Where(n => n.Mesh != null);
            foreach (var inst in meshNodes)
            {
                var mesh = inst.Mesh;

                if (inst.Skin == null) continue; // throw new InvalidOperationException($"Can't convert gtlf with mesh ({mesh.Name}) without a skin.");

                if (!mesh.AllPrimitivesHaveJoints) throw new InvalidOperationException($"Can't convert gtlf with mesh ({mesh.Name}) primitve without a skin.");

                if (mesh.MorphWeights.Count > 0) throw new InvalidOperationException($"Can't convert gltf with mesh ({mesh.Name}) vertex morphs to grn.");

                if (mesh.Primitives.Any(p => p.Material == null)) throw new NotImplementedException($"The converter does not support primitives ({mesh.Name}) with a null material.");

                // TODO: Convert materials and add to id mapping
                // Create grn materials and a way to map their ids
                var gltfMats = mesh.Primitives.Select(p => p.Material);

                // Now add a new mesh from mesh builder
                ConvertMesh(grn, inst, nodes, matIdMapping);
            }
        }

        private void ConvertMesh(GrnFile grn, Node meshNode, List<Node> nodes, Dictionary<int, int> matIdMapping)
        {
            GrnMesh mesh = new GrnMesh(grn);
            mesh.DataExtensionIndex = grn.AddDataExtension(meshNode.Mesh.Name ?? meshNode.Name ?? $"mesh{meshNode.Mesh.LogicalIndex}");

            // Get bone bindings
            var skinTransforms = new Matrix4x4[meshNode.Skin.JointsCount];
            for (int i = 0; i < skinTransforms.Length; ++i)
            {
                // TODO: Add adjustment to skin transforms for difference between gltf and grn coord system
                var skinJoint = meshNode.Skin.GetJoint(i);
                skinTransforms[i] = skinJoint.InverseBindMatrix * skinJoint.Joint.WorldMatrix;

                var bb = new GrnBoneBinding();
                bb.BoneIndex = nodes.IndexOf(skinJoint.Joint) + 1;
                // TODO: figure these out
                bb.OBBMin = new Vector3(-0.25f);
                bb.OBBMax = new Vector3(0.25f);
                mesh.BoneBindings.Add(bb);
            }

            // Export Vertices, Normals, TexCoords
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
                if (vertexAccessors.ContainsKey("POSITION")) throw new InvalidDataException($"Mesh ({gltfMesh.Name}) must have positions.");
                if (!vertexAccessors.ContainsKey("NORMAL")) throw new InvalidDataException($"Mesh ({gltfMesh.Name}) must have normals.");

                if (!vertexAccessors.ContainsKey("TEXCOORD_0")) throw new InvalidDataException($"Mesh ({gltfMesh.Name}) must have tex coord set.");
                if (vertexAccessors.ContainsKey("TEXCOORD_1")) throw new InvalidOperationException($"Can't convert mesh ({gltfMesh.Name}) with more than one set of texcoords.");

                if (vertexAccessors.ContainsKey("JOINTS_0")) throw new InvalidDataException($"Mesh ({gltfMesh.Name}) must have a set of joints.");
                if (vertexAccessors.ContainsKey("JOINTS_1")) throw new InvalidOperationException($"Can't convert mesh ({gltfMesh.Name}) with more than one set of joints.");

                if (vertexAccessors.ContainsKey("WEIGHTS_0")) throw new InvalidDataException($"Mesh ({gltfMesh.Name}) must have a set of weights.");
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
                    mesh.Vertices.Add(new Vector3(-pos.X, pos.Y, pos.Z));
                    mesh.Normals.Add(new Vector3(-norm.X, norm.Y, norm.Z));
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
                    GrnFace face = new GrnFace();
                    face.MaterialIndex = faceMatId;
                    face.Indices.Add(tri.A);
                    face.Indices.Add(tri.B);
                    face.Indices.Add(tri.C);
                    face.NormalIndices.Add(tri.A);
                    face.NormalIndices.Add(tri.B);
                    face.NormalIndices.Add(tri.C);
                    face.TextureIndices.Add(tri.A);
                    face.TextureIndices.Add(tri.B);
                    face.TextureIndices.Add(tri.C);
                    mesh.Faces.Add(face);
                }
            }

            if (mesh.Faces.Count > 0 && mesh.Vertices.Count > 0)
                grn.Meshes.Add(mesh);
        }
    }
}
