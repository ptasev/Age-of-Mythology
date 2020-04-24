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

            var sceneTemplate = SceneTemplate.Create(scene, false);
            var instance = sceneTemplate.CreateInstance();

            ConvertSkeleton(grn, instance);

            return grn;
        }

        public void ConvertSkeleton(GrnFile grn, SceneInstance scene)
        {
            // Add root
            grn.Bones.Add(new GrnBone(grn));
            grn.Bones[0].DataExtensionIndex = grn.AddDataExtension("__Root");
            grn.Bones[0].Rotation = new Quaternion(0, 0, 0, 1);
            grn.Bones[0].ParentIndex = 0;

            var nodes = scene.LogicalNodes.ToList();
            foreach (var node in scene.LogicalNodes)
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

        public void ConvertMesh(GrnFile grn, ModelRoot gltf, SceneInstance scene)
        {
            foreach (var inst in scene.DrawableInstances)
            {
                var mesh = gltf.LogicalMeshes[inst.Template.LogicalMeshIndex];

                // skip meshes if all prims don't have joints
                if (!mesh.AllPrimitivesHaveJoints) throw new InvalidOperationException($"Can't convert gtlf with mesh ({mesh.Name}) primitve without a skin.");

                if (!(inst.Transform is SkinnedTransform skinTransform)) throw new InvalidOperationException($"Can't convert gtlf with mesh ({mesh.Name}) without a skin.");

                if (mesh.MorphWeights.Count > 0) throw new InvalidOperationException($"Can't convert gltf with mesh ({mesh.Name}) vertex morphs to grn.");

                var tris = mesh.EvaluateTriangles(skinTransform);

                //var mb = new GltfMeshBuilder();
            }
        }
    }
}
