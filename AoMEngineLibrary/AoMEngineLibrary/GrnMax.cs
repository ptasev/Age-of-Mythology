using AoMEngineLibrary.Graphics;
using AoMEngineLibrary.Graphics.Grn;
using AoMEngineLibrary.Graphics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AoMEngineLibrary
{
    public static class GrnMax
    {
        public static GrnFile File;
        private static Dictionary<string, int> boneMap = new Dictionary<string, int>();
        private static bool matGroupInit = false;

        public static bool ExportAnimations = false;

        public static void Clear()
        {
            File = new GrnFile();
            boneMap = new Dictionary<string, int>();
            matGroupInit = false;
        }

        public static void Import()
        {
            string mainObject = "mainObject";
            string boneArray = "boneArray";

            if (!GrnMax.matGroupInit)
            {
                Maxscript.Command("matGroup = multimaterial numsubs:{0}", File.Materials.Count);
                GrnMax.matGroupInit = true;
            }

            GrnMax.ImportSkeleton(boneArray);

            for (int i = 0; i < File.Meshes.Count; i++)
            {
                GrnMax.ImportMesh(File.Meshes[i], mainObject, boneArray);
                Maxscript.Command("select {0}", mainObject);
                Maxscript.Command("{0}.material = matGroup", mainObject);
            }

            if (File.Animation.Duration > 0)
            {
                GrnMax.ImportAnimation(boneArray);
            }

            for (int i = 0; i < File.Materials.Count; i++)
            {
                Maxscript.Command("matGroup[{0}] = {1}", i + 1, ImportMaterial(File.Materials[i]));
            }

            Maxscript.Command("max zoomext sel all");
        }
        private static void ImportSkeleton(string boneArray)
        {
            if (boneMap.Count == 0)
            {
                Maxscript.NewArray(boneArray);
            }

            for (int i = 0; i < File.Bones.Count; ++i)
            {
                GrnBone bone = File.Bones[i];
                if (bone.Name == "__Root")
                {
                    continue;
                }

                if (boneMap.ContainsKey(bone.Name))
                {
                    string bPos = Maxscript.NewPoint3<float>("bPos", bone.Position.X, bone.Position.Y, bone.Position.Z);
                    Maxscript.Command("bRot = quat {0} {1} {2} {3}", bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z, bone.Rotation.W);
                    Maxscript.Command("{0}[{1}].rotation = {2}", boneArray, boneMap[bone.Name] + 1, "bRot");
                    Maxscript.Command("{0}[{1}].position = {2}", boneArray, boneMap[bone.Name] + 1, bPos);
                    Maxscript.Command("{0}[{1}].scale = {2}", boneArray, boneMap[bone.Name] + 1,
                        Maxscript.Point3Literal(bone.Scale.A1, bone.Scale.B2, bone.Scale.C3));

                    //Maxscript.Command("{0}[{1}].transform = {2}", boneArray, boneMap[bone.Name] + 1, 
                    //    GrnMax.GetBoneLocalTransform(bone, "boneTransMat"));
                }
                else
                {
                    boneMap.Add(bone.Name, boneMap.Count);
                    GrnMax.CreateBone(bone);
                    Maxscript.Append(boneArray, "boneNode");
                }

                if (bone.ParentIndex > 0)
                {
                    Maxscript.Command("{0}[{1}].parent = {0}[{2}]", boneArray, boneMap[bone.Name] + 1,
                        boneMap[File.Bones[bone.ParentIndex].Name] + 1);
                    Maxscript.Command("{0}[{1}].transform *= {0}[{1}].parent.transform", boneArray, boneMap[bone.Name] + 1);
                }
            }
        }
        private static void ImportMesh(GrnMesh mesh, string mainObject, string boneArray)
        {
            string vertArray = "";
            string normArray = "";
            string texVerts = "";
            string faceMats = "";
            string faceArray = "";
            string tFaceArray = "";
            vertArray = Maxscript.NewArray("vertArray");
            normArray = Maxscript.NewArray("normArray");
            texVerts = Maxscript.NewArray("texVerts");
            faceMats = Maxscript.NewArray("faceMats");
            faceArray = Maxscript.NewArray("faceArray");
            tFaceArray = Maxscript.NewArray("tFaceArray");

            for (int i = 0; i < mesh.Vertices.Count; ++i)
            {
                Maxscript.Append(vertArray, Maxscript.NewPoint3<float>("v",
                    mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z));
            }

            for (int i = 0; i < mesh.TextureCoordinates.Count; ++i)
            {
                Maxscript.Append(texVerts, Maxscript.NewPoint3<float>("tV",
                    mesh.TextureCoordinates[i].X, mesh.TextureCoordinates[i].Y, mesh.TextureCoordinates[i].Z));
            }

            foreach (var face in mesh.Faces)
            {
                Maxscript.Append(faceMats, face.MaterialIndex.ToString());
                Maxscript.Append(faceArray, Maxscript.NewPoint3<Int32>("fV", 
                    face.Indices[0] + 1, face.Indices[1] + 1, face.Indices[2] + 1));
                Maxscript.Append(tFaceArray, Maxscript.NewPoint3<Int32>("tFV", 
                    face.TextureIndices[0] + 1, face.TextureIndices[1] + 1, face.TextureIndices[2] + 1));
            }

            Maxscript.Command("meshBone = getNodeByName \"{0}\"", mesh.Name);
            Maxscript.Command("{5} = mesh name:\"{0}\" vertices:{1} faces:{2} materialIDs:{3} tverts:{4}", mesh.Name, vertArray, faceArray, faceMats, texVerts, mainObject);
            if (Maxscript.QueryBoolean("meshBone != undefined"))
            {
                Maxscript.Command("{0}.parent = meshBone.parent", mainObject);
                Maxscript.Command("delete meshBone");
            }

            Maxscript.CommentTitle("TVert Hack"); // Needed <= 3ds Max 2014; idk about 2015+
            Maxscript.Command("buildTVFaces {0}", mainObject);
            for (int i = 0; i < mesh.Faces.Count; ++i)
            {
                //for i = 1 to mmesh.numfacesdo(setTVFace mmesh i (getFace mmesh i))
                Maxscript.Command("setTVFace {0} {1} {2}[{1}]", mainObject, i + 1, tFaceArray);
            }

            //for (int i = 0; i < mesh.Vertices.Count; i++) // changed from Normals to Vertices
            //{
            //    Maxscript.Command("setNormal {0} {1} {2}", mainObject, i + 1,
            //        Maxscript.Point3Literal(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z));
            //}

            // Bones
            Maxscript.Command("max modify mode");
            Maxscript.Command("skinMod = Skin()");
            Maxscript.Command("addModifier {0} skinMod", mainObject);
            Maxscript.Command("modPanel.setCurrentObject skinMod");
            for (int i = 0; i < mesh.BoneBindings.Count; ++i)
            {
                GrnBoneBinding boneBinding = mesh.BoneBindings[i];
                string boundingScale = Maxscript.NewPoint3<float>("boundingScale",
                    (boneBinding.OBBMax.X - boneBinding.OBBMin.X), (boneBinding.OBBMax.Y - boneBinding.OBBMin.Y), (boneBinding.OBBMax.Z - boneBinding.OBBMin.Z));
                Maxscript.Command("{0}[{1}].boxsize = {2}", boneArray, boneMap[File.Bones[boneBinding.BoneIndex].Name] + 1, boundingScale);
                Maxscript.Command("skinOps.addBone skinMod {0}[{1}] {2}", boneArray,
                    boneMap[File.Bones[boneBinding.BoneIndex].Name] + 1, i + 1 == mesh.BoneBindings.Count ? 1 : 0);
            }
            Maxscript.Command("completeRedraw()"); // would get "Exceeded the vertex countSkin:skin" error without this
            for (int i = 0; i < mesh.VertexWeights.Count; ++i)
            {
                // Index correspond to order that the bones were added to Skin mod
                string boneIndexArray = Maxscript.NewArray("boneIndexArray");
                string weightsArray = Maxscript.NewArray("weightsArray");
                for (int j = 0; j < mesh.VertexWeights[i].Weights.Count; ++j)
                {
                    Maxscript.Append(boneIndexArray, mesh.VertexWeights[i].BoneIndices[j] + 1);
                    Maxscript.Append(weightsArray, mesh.VertexWeights[i].Weights[j]);
                }
                Maxscript.Command("skinOps.ReplaceVertexWeights skinMod {0} {1} {2}",
                    i + 1, 1, 0.0);
                Maxscript.Command("skinOps.ReplaceVertexWeights skinMod {0} {1} {2}",
                    i + 1, boneIndexArray, weightsArray);
            }
        }
        private static void ImportAnimation(string boneArray)
        {
            Maxscript.Command("frameRate = {0}", 60);
            Maxscript.Interval(0, File.Animation.Duration);

            for (int i = 0; i < File.Animation.BoneTracks.Count; ++i)
            {
                if (File.Bones[i].Name == "__Root")
                {
                    continue;
                }
                GrnBoneTrack bone = File.Animation.BoneTracks[i];
                int boneArrayIndex = boneMap[File.Bones[i].Name] + 1;

                for (int j = 0; j < bone.PositionKeys.Count; ++j)
                {
                    Maxscript.Command("addNewKey {0}[{1}][3][1][1].controller {2}s", boneArray, boneArrayIndex, bone.PositionKeys[j]);
                    Maxscript.Command("addNewKey {0}[{1}][3][1][2].controller {2}s", boneArray, boneArrayIndex, bone.PositionKeys[j]);
                    Maxscript.Command("addNewKey {0}[{1}][3][1][3].controller {2}s", boneArray, boneArrayIndex, bone.PositionKeys[j]);

                    Maxscript.Command("{0}[{1}][3][1][1].controller.keys[{2}].value = {3}",
                        boneArray, boneArrayIndex, j + 1, bone.Positions[j].X);
                    Maxscript.Command("{0}[{1}][3][1][2].controller.keys[{2}].value = {3}",
                        boneArray, boneArrayIndex, j + 1, bone.Positions[j].Y);
                    Maxscript.Command("{0}[{1}][3][1][3].controller.keys[{2}].value = {3}",
                        boneArray, boneArrayIndex, j + 1, bone.Positions[j].Z);
                }

                Maxscript.Command("{0}[{1}][3][2].controller = Bezier_Rotation()", boneArray, boneArrayIndex);
                for (int j = 0; j < bone.RotationKeys.Count; ++j)
                {
                    Maxscript.Command("addNewKey {0}[{1}][3][2].controller {2}s", boneArray, boneArrayIndex, bone.RotationKeys[j]);
                    Maxscript.Command("{0}[{1}][3][2].controller.keys[{2}].value = {3}", 
                        boneArray, boneArrayIndex, j + 1, Maxscript.QuatLiteral(bone.Rotations[j]));
                }

                for (int j = 0; j < bone.ScaleKeys.Count; ++j)
                {
                    Maxscript.Command("addNewKey {0}[{1}][3][3].controller {2}s", boneArray, boneArrayIndex, bone.ScaleKeys[j]);
                    Maxscript.Command("{0}[{1}][3][3].controller.keys[{2}].value = {3}", boneArray, boneArrayIndex, j + 1,
                        Maxscript.Point3Literal(bone.Scales[j].A1, bone.Scales[j].B2, bone.Scales[j].C3));
                }
            }
        }
        private static string ImportMaterial(GrnMaterial mat)
        {
            Maxscript.Command("mat = StandardMaterial()");
            Maxscript.Command("mat.name = \"{0}\"", mat.Name);
            Maxscript.Command("mat.adLock = false");
            Maxscript.Command("mat.useSelfIllumColor = true");
            Maxscript.Command("mat.diffuse = color {0} {1} {2}", mat.DiffuseColor.R * 255f, mat.DiffuseColor.G * 255f, mat.DiffuseColor.B * 255f);
            Maxscript.Command("mat.ambient = color {0} {1} {2}", mat.AmbientColor.R * 255f, mat.AmbientColor.G * 255f, mat.AmbientColor.B * 255f);
            Maxscript.Command("mat.specular = color {0} {1} {2}", mat.SpecularColor.R * 255f, mat.SpecularColor.G * 255f, mat.SpecularColor.B * 255f);
            Maxscript.Command("mat.selfIllumColor = color {0} {1} {2}", mat.EmissiveColor.R * 255f, mat.EmissiveColor.G * 255f, mat.EmissiveColor.B * 255f);
            Maxscript.Command("mat.opacity = {0}", mat.Opacity * 100f);
            Maxscript.Command("mat.specularLevel = {0}", mat.SpecularExponent);

            Maxscript.Command("tex = BitmapTexture()");
            Maxscript.Command("tex.name = \"{0}\"", mat.DiffuseMap);
            if (mat.Name == "pixelxform1")
            {
                Maxscript.Command("pcCompTex = CompositeTextureMap()");

                Maxscript.Command("pcTex = BitmapTexture()");
                Maxscript.Command("pcTex.name = \"{0}\"", mat.DiffuseMap);
                Maxscript.Command("pcTex.filename = \"{0}\"", mat.DiffuseMapFileName);

                Maxscript.Command("pcTex2 = BitmapTexture()");
                Maxscript.Command("pcTex2.name = \"{0}\"", mat.DiffuseMap);
                Maxscript.Command("pcTex2.filename = \"{0}\"", mat.DiffuseMapFileName);
                Maxscript.Command("pcTex2.monoOutput = 1");

                Maxscript.Command("pcCheck = Checker()");
                Maxscript.Command("pcCheck.Color1 = color 0 0 255");
                Maxscript.Command("pcCheck.Color2 = color 0 0 255");

                Maxscript.Command("pcCompTex.mapList[1] = pcTex");
                Maxscript.Command("pcCompTex.mapList[2] = pcCheck");
                Maxscript.Command("pcCompTex.mask[2] = pcTex2");

                Maxscript.Command("mat.diffusemap = pcCompTex");
            }
            else
            {
                Maxscript.Command("tex.filename = \"{0}\"", mat.DiffuseMapFileName);
                Maxscript.Command("mat.diffusemap = tex");
            }

            return "mat";
        }
        //public static void ExportAnimToMax()
        //{
        //    Maxscript.Command("frameRate = {0}", Math.Round(1 / AnimFile.Animation.TimeStep));
        //    Maxscript.Interval(0, AnimFile.Animation.Duration);

        //    string boneArray = "boneArray";

        //    // Match up the animation file bones to the model file bones
        //    List<int> boneMap = new List<int>();
        //    int currentNumberOfBones = MeshFile.Bones.Count;
        //    for (int i = 0; i < AnimFile.Bones.Count; ++i)
        //    {
        //        boneMap.Add(-1);
        //        for (int j = 0; j < MeshFile.Bones.Count; ++j)
        //        {
        //            if (AnimFile.Bones[i].Name == MeshFile.Bones[j].Name)
        //            {
        //                boneMap[i] = j;
        //                break;
        //            }
        //        }

        //        if (boneMap[i] == -1)
        //        {
        //            // Create the new bone
        //            GrnBone bone = AnimFile.Bones[i];
        //            boneMap[i] = currentNumberOfBones++;
        //            string bPos = Maxscript.NewPoint3<float>("bPos", bone.Position.X, bone.Position.Y, bone.Position.Z);
        //            Maxscript.Command("bRot = quat {0} {1} {2} {3}", bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z, bone.Rotation.W);
        //            Maxscript.Command("boneNode = dummy name:\"{0}\" rotation:{1} position:{2} boxsize:{3}",
        //                bone.Name, "bRot", "bPos", "[0.25,0.25,0.25]");
        //            Maxscript.Append(boneArray, "boneNode");
        //        }

        //        //Maxscript.Command("print \"{0} {1}\"", AnimFile.Bones[i].Name, boneMap[i]);
        //    }

        //    // Set the animation file base skeleton pose
        //    for (int i = 0; i < AnimFile.Bones.Count; ++i)
        //    {
        //        GrnBone bone = AnimFile.Bones[i];
        //        string bPos = Maxscript.NewPoint3<float>("bPos", bone.Position.X, bone.Position.Y, bone.Position.Z);
        //        Maxscript.Command("bRot = quat {0} {1} {2} {3}", bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z, bone.Rotation.W);
        //        Maxscript.Command("{0}[{1}].rotation = {2}", boneArray, boneMap[i] + 1, "bRot");
        //        Maxscript.Command("{0}[{1}].position = {2}", boneArray, boneMap[i] + 1, bPos);

        //        if (AnimFile.Bones[i].ParentIndex != i)
        //        {
        //            Maxscript.Command("{0}[{1}].parent = {0}[{2}]", boneArray, boneMap[i] + 1, boneMap[AnimFile.Bones[i].ParentIndex] + 1);
        //            Maxscript.Command("{0}[{1}].transform *= {0}[{1}].parent.transform", boneArray, boneMap[i] + 1);
        //        }
        //    }

        //    // Animate all bones
        //    for (int i = 0; i < AnimFile.Animation.BoneTracks.Count; ++i)
        //    {
        //        GrnBoneTrack bone = AnimFile.Animation.BoneTracks[i];
        //        for (int j = 0; j < bone.RotationKeys.Count; ++j)
        //        {
        //            Maxscript.Command("bRot = quat {0} {1} {2} {3}", bone.Rotations[j].X, bone.Rotations[j].Y, bone.Rotations[j].Z, bone.Rotations[j].W);
        //            Maxscript.AnimateAtTime(bone.RotationKeys[j], "rotate {0}[{1}] {0}[{1}].transform.rotation", boneArray, boneMap[i] + 1);
        //            Maxscript.AnimateAtTime(bone.RotationKeys[j], "rotate {0}[{1}] {2}", boneArray, boneMap[i] + 1, "bRot");
        //            //Maxscript.AnimateAtTime(bone.RotationKeys[j], "{0}[{1}].rotation = {2}", boneArray, boneMap[i] + 1, "bRot");
        //            if (AnimFile.Bones[i].ParentIndex != i)
        //            {
        //                Maxscript.AnimateAtTime(bone.RotationKeys[j], "rotate {0}[{1}] {0}[{1}].parent.rotation", boneArray, boneMap[i] + 1);
        //                //Maxscript.AnimateAtTime(bone.RotationKeys[j], "rotate {0}[{1}] {0}[{1}].parent.rotation", boneArray, boneMap[i] + 1);
        //                //Maxscript.AtTime(bone.RotationKeys[j],
        //                //    "bRot = -({0}[{1}].parent.rotation * {0}[{1}].rotation)", boneArray, boneMap[i] + 1);
        //                //Maxscript.AnimateAtTime(bone.RotationKeys[j], "{0}[{1}].rotation = {2}", boneArray, boneMap[i] + 1, "bRot");
        //                //Maxscript.AtTime(bone.RotationKeys[j],
        //                //    "bRot = ({0}[{1}].transform * {0}[{1}].parent.transform).rotation", boneArray, boneMap[i] + 1);
        //                //Maxscript.AnimateAtTime(bone.RotationKeys[j], "{0}[{1}].rotation = {2}", boneArray, boneMap[i] + 1, "bRot");
        //                //Maxscript.AnimateAtTime(bone.RotationKeys[j], "{0}[{1}].transform *= {0}[{1}].parent.transform", boneArray, boneMap[i] + 1);
        //            }
        //        }
        //        for (int j = 0; j < bone.PositionKeys.Count; ++j)
        //        {
        //            string bPos = Maxscript.NewPoint3<float>("bPos", bone.Positions[j].X, bone.Positions[j].Y, bone.Positions[j].Z);
        //            Maxscript.AnimateAtTime(bone.PositionKeys[j], "{0}[{1}].position = {2}", boneArray, boneMap[i] + 1, bPos);
        //            if (AnimFile.Bones[i].ParentIndex != i)
        //            {
        //                Maxscript.AtTime(bone.PositionKeys[j],
        //                    "bPos = ({0}[{1}].transform * {0}[{1}].parent.transform).translation", boneArray, boneMap[i] + 1);
        //                Maxscript.AnimateAtTime(bone.PositionKeys[j], "{0}[{1}].position = {2}", boneArray, boneMap[i] + 1, "bPos");
        //                //Maxscript.AnimateAtTime(bone.PositionKeys[j], "{0}[{1}].transform *= {0}[{1}].parent.transform", boneArray, boneMap[i] + 1);
        //            }
        //        }
        //    }
        //}

        public static void Export()
        {
            GrnMax.Clear();

            Maxscript.Command("ExportGrnData()");

            GrnMax.ExportSkeleton();

            int meshCount = Maxscript.QueryInteger("grnMeshes.count");
            for (int i = 0; i < meshCount; ++i)
            {
                File.Meshes.Add(new GrnMesh(File));
                GrnMax.ExportMesh(i);
            }

            if (GrnMax.ExportAnimations)
            {
                GrnMax.ExportAnimation();
                if (File.Animation.Duration == 0f)
                {
                    File.Animation.Duration = 1f;
                }
                File.Animation.TimeStep = 1f / 60f;
            }

            int numMaterials = Maxscript.QueryInteger("{0}.material.materialList.count", "mainObject");
            for (int i = 0; i < numMaterials; i++)
            {
                File.Materials.Add(new GrnMaterial(File));
                GrnMax.ExportMaterial(i, "mainObject");
            }
        }
        private static void ExportSkeleton()
        {
            int numBones = Maxscript.QueryInteger("grnBones.count");
            File.Bones.Add(new GrnBone(File));
            File.Bones[0].DataExtensionIndex = File.AddDataExtension("__Root");

            for (int i = 1; i <= numBones; ++i)
            {
                if (GrnMax.ExportAnimations && Maxscript.QueryBoolean("grnBones[{0}].isanimated == false", i))
                {
                    continue;
                }

                try
                {
                    GrnBone bone = new GrnBone(File);
                    bone.DataExtensionIndex = File.AddDataExtension(Maxscript.QueryString("grnBones[{0}].name", i));
                    bone.ParentIndex = Maxscript.QueryInteger("grnBoneParents[{0}]", i);

                    Maxscript.Command("copiedBone = copy grnBones[{0}]", i);
                    if (bone.ParentIndex > 0)
                    {
                        //Maxscript.Command("copiedBone.transform *= (inverse copiedBone.parent.transform)");
                    }

                    bone.Position = new Vector3D(
                            Maxscript.QueryFloat("(coordsys parent copiedBone.position).x"),
                            Maxscript.QueryFloat("(coordsys parent copiedBone.position).y"),
                            Maxscript.QueryFloat("(coordsys parent copiedBone.position).z"));
                    bone.Rotation = new Quaternion(
                        Maxscript.QueryFloat("(coordsys local copiedBone.rotation).x"),
                        Maxscript.QueryFloat("(coordsys local copiedBone.rotation).y"),
                        Maxscript.QueryFloat("(coordsys local copiedBone.rotation).z"),
                        Maxscript.QueryFloat("(coordsys local copiedBone.rotation).w"));
                    Matrix3x3 scale = new Matrix3x3();
                    scale.A1 = Maxscript.QueryFloat("(coordsys parent copiedBone).scale.x");
                    scale.B2 = Maxscript.QueryFloat("(coordsys parent copiedBone).scale.y");
                    scale.C3 = Maxscript.QueryFloat("(coordsys parent copiedBone).scale.z");

                    Maxscript.Command("delete copiedBone");

                    //if (bone.ParentIndex > 0)
                    //{
                    //    Maxscript.Command("boneTM = grnBones[{0}].transform * (inverse grnBones[{0}].parent.transform)", i);
                    //}
                    //else
                    //{
                    //    Maxscript.Command("boneTM = grnBones[{0}].transform", i);
                    //}


                    //bone.Position = new Vector3D(
                    //        Maxscript.QueryFloat("boneTM.translation.x"),
                    //        Maxscript.QueryFloat("boneTM.translation.y"),
                    //        Maxscript.QueryFloat("boneTM.translation.z"));
                    //bone.Rotation = new Quaternion(
                    //    Maxscript.QueryFloat("boneTM.rotation.x"),
                    //    Maxscript.QueryFloat("boneTM.rotation.y"),
                    //    Maxscript.QueryFloat("boneTM.rotation.z"),
                    //    -Maxscript.QueryFloat("boneTM.rotation.w"));
                    //Matrix3x3 scale = new Matrix3x3();
                    //scale.A1 = Maxscript.QueryFloat("boneTM.scale.x");
                    //scale.B2 = Maxscript.QueryFloat("boneTM.scale.y");
                    //scale.C3 = Maxscript.QueryFloat("boneTM.scale.z");
                    bone.Scale = scale;
                    File.Bones.Add(bone);
                }
                catch (Exception ex)
                {
                    throw new Exception("Bone Index = " + i, ex);
                }
            }
        }
        private static void ExportMesh(int meshIndex)
        {
            GrnMesh mesh = File.Meshes[meshIndex];
            string mainObject = "mainObject";
            Maxscript.Command("{0} = grnMeshes[{1}]", mainObject, meshIndex + 1);
            mesh.DataExtensionIndex = File.AddDataExtension(Maxscript.QueryString("{0}.name", mainObject));

            // Setup Normals
            //Maxscript.Command("max modify mode");
            //if (Maxscript.QueryBoolean("{0}.modifiers[#edit_normals] == undefined", mainObject))
            //{
            //    Maxscript.Command("addModifier {0} (Edit_Normals())", mainObject);
            //}
            //Maxscript.Command("modPanel.setCurrentObject {0}.modifiers[#edit_normals] ui:false", mainObject);
            //Maxscript.Command("CalculateAveragedNormals()");

            int numVertices = Maxscript.QueryInteger("meshop.getnumverts {0}", mainObject);
            int numFaces = Maxscript.QueryInteger("meshop.getnumfaces {0}", mainObject);

            for (int i = 0; i < numVertices; i++)
            {
                try
                {
                    Maxscript.Command("vertex = meshop.getVert {0} {1}", mainObject, i + 1);
                    mesh.Vertices.Add(new Vector3D(
                        Maxscript.QueryFloat("vertex.x"),
                        Maxscript.QueryFloat("vertex.y"),
                        Maxscript.QueryFloat("vertex.z")));

                    //mesh.Normals.Add(new Vector3D(
                    //    Maxscript.QueryFloat("{0}[{1}].x", "averagedNormals", i + 1),
                    //    Maxscript.QueryFloat("{0}[{1}].y", "averagedNormals", i + 1),
                    //    Maxscript.QueryFloat("{0}[{1}].z", "averagedNormals", i + 1)));
                }
                catch (Exception ex)
                {
                    throw new Exception("Import Verts/Normals " + i.ToString(), ex);
                }
            }

            int numTexVertices = Maxscript.QueryInteger("meshop.getnumtverts {0}", mainObject);
            for (int i = 0; i < numVertices; i++)
            {
                Maxscript.Command("tVert = meshop.getmapvert {0} 1 {1}", mainObject, i + 1);
                mesh.TextureCoordinates.Add(new Vector3D(
                    Maxscript.QueryFloat("tVert.x"),
                    Maxscript.QueryFloat("tVert.y"),
                    Maxscript.QueryFloat("tVert.z")));
            }

            for (int i = 0; i < numFaces; ++i)
            {
                Face f = new Face();
                f.MaterialIndex = (Int16)Maxscript.QueryInteger("getFaceMatID {0} {1}", mainObject, i + 1);
                Maxscript.Command("face = getFace {0} {1}", mainObject, i + 1);
                f.Indices.Add((Int16)(Maxscript.QueryInteger("face.x") - 1));
                f.Indices.Add((Int16)(Maxscript.QueryInteger("face.y") - 1));
                f.Indices.Add((Int16)(Maxscript.QueryInteger("face.z") - 1));
                f.NormalIndices.Add(f.Indices[0]);
                f.NormalIndices.Add(f.Indices[1]);
                f.NormalIndices.Add(f.Indices[2]);
                Maxscript.Command("tFace = getTVFace {0} {1}", mainObject, i + 1);
                f.TextureIndices.Add(Maxscript.QueryInteger("tFace.x") - 1);
                f.TextureIndices.Add(Maxscript.QueryInteger("tFace.y") - 1);
                f.TextureIndices.Add(Maxscript.QueryInteger("tFace.z") - 1);
                mesh.Faces.Add(f);
            }

            if (Maxscript.QueryBoolean("{0}.modifiers[#skin] != undefined", mainObject))
            {
                Maxscript.Command("skinMod = {0}.modifiers[#skin]", mainObject);
                Maxscript.Command("modPanel.setCurrentObject skinMod ui:false");
                Maxscript.Command("ExportSkinData()");
                int numBVerts = Maxscript.QueryInteger("grnSkinWeights.count");
                for (int i = 0; i < numBVerts; ++i)
                {
                    mesh.VertexWeights.Add(new VertexWeight());
                    Maxscript.Command("skinWeightArray = grnSkinWeights[{0}]", i + 1);
                    int numVWs = Maxscript.QueryInteger("skinWeightArray.count");
                    for (int j = 0; j < numVWs; ++j)
                    {
                        mesh.VertexWeights[i].BoneIndices.Add(Maxscript.QueryInteger("skinWeightArray[{0}][1]", j + 1));
                        mesh.VertexWeights[i].Weights.Add(Maxscript.QueryInteger("skinWeightArray[{0}][2]", j + 1));
                    }
                }

                int numSkinBBs = Maxscript.QueryInteger("grnSkinBBMaxs.count");
                for (int i = 0; i < numSkinBBs; ++i)
                {
                    Maxscript.Command("bbMax = grnSkinBBMaxs[{0}]", i + 1);
                    Maxscript.Command("bbMin = grnSkinBBMins[{0}]", i + 1);
                    mesh.BoneBindings.Add(new GrnBoneBinding());
                    mesh.BoneBindings[i].BoneIndex = Maxscript.QueryInteger("grnSkinBBIndices[{0}]", i + 1);
                    mesh.BoneBindings[i].OBBMax = new Vector3D(
                        Maxscript.QueryFloat("bbMax.x"),
                        Maxscript.QueryFloat("bbMax.y"),
                        Maxscript.QueryFloat("bbMax.z"));
                    mesh.BoneBindings[i].OBBMin = new Vector3D(
                        Maxscript.QueryFloat("bbMin.x"),
                        Maxscript.QueryFloat("bbMin.y"),
                        Maxscript.QueryFloat("bbMin.z"));
                }
            }
        }
        private static void ExportAnimation()
        {
            int numBones = Maxscript.QueryInteger("grnBones.count");
            File.Animation.Duration = 0f;

            for (int i = 1; i <= numBones; ++i)
            {
                if (Maxscript.QueryBoolean("grnBones[{0}].isanimated == false", i))
                {
                    continue;
                }

                GrnBoneTrack bone = new GrnBoneTrack();
                bone.DataExtensionIndex = File.Bones[i].DataExtensionIndex;

                int numPosKeys = Maxscript.QueryInteger("grnBones[{0}][3][1].controller.keys.count", i);
                Maxscript.Command("sort grnBones[{0}][3][1].controller.keys");
                for (int j = 0; j < numPosKeys; ++j)
                {
                    bone.PositionKeys.Add(Maxscript.QueryFloat("grnBones[{0}][3][1].controller.keys[{1]].time as float / 4800", i, j + 1));
                    bone.Positions.Add(new Vector3D(
                        Maxscript.QueryFloat("grnBones[{0}][3][1][1].controller.keys[{1}].value", i, j + 1),
                        Maxscript.QueryFloat("grnBones[{0}][3][1][2].controller.keys[{1}].value", i, j + 1),
                        Maxscript.QueryFloat("grnBones[{0}][3][1][3].controller.keys[{1}].value", i, j + 1)));
                }

                int numRotKeys = Maxscript.QueryInteger("grnBones[{0}][3][2].controller.keys.count", i);
                for (int j = 0; j < numRotKeys; ++j)
                {
                    bone.RotationKeys.Add(Maxscript.QueryFloat("grnBones[{0}][3][2].controller.keys[{1]].time as float / 4800", i, j + 1));
                    Maxscript.Command("rotQuat = grnBones[{0}][3][2].controller.keys[{1}].value as quat", i, j + 1);
                    bone.Rotations.Add(new Quaternion(
                        Maxscript.QueryFloat("rotQuat.x"),
                        Maxscript.QueryFloat("rotQuat.y"),
                        Maxscript.QueryFloat("rotQuat.z"),
                        Maxscript.QueryFloat("rotQuat.w")));
                }

                int numScaleKeys = Maxscript.QueryInteger("grnBones[{0}][3][3].controller.keys.count", i);
                for (int j = 0; j < numScaleKeys; ++j)
                {
                    bone.ScaleKeys.Add(Maxscript.QueryFloat("grnBones[{0}][3][3].controller.keys[{1]].time as float / 4800", i, j + 1));
                    Maxscript.Command("bTrackScale = grnBones[{0}][3][3].controller.keys[{1}].value", i, j + 1);
                    Matrix3x3 scaleMatrix = new Matrix3x3();
                    scaleMatrix.A1 = Maxscript.QueryFloat("bTrackScale.x");
                    scaleMatrix.B2 = Maxscript.QueryFloat("bTrackScale.y");
                    scaleMatrix.C3 = Maxscript.QueryFloat("bTrackScale.z");
                    bone.Scales.Add(scaleMatrix);
                }

                File.Animation.BoneTracks.Add(bone);
                File.Animation.Duration = Math.Max(File.Animation.Duration, bone.PositionKeys.Last());
                File.Animation.Duration = Math.Max(File.Animation.Duration, bone.RotationKeys.Last());
                File.Animation.Duration = Math.Max(File.Animation.Duration, bone.ScaleKeys.Last());
            }
        }
        private static void ExportMaterial(int matIndex, string mainObject)
        {
            GrnMaterial mat = File.Materials[matIndex];
            Maxscript.Command("mat = {0}.material[{1}]", mainObject, matIndex + 1);
            mat.DataExtensionIndex = File.AddDataExtension(Maxscript.QueryString("mat.name"));

            mat.DiffuseColor = new Color3D(Maxscript.QueryFloat("mat.diffuse.r") / 255f,
                Maxscript.QueryFloat("mat.diffuse.g") / 255f,
                Maxscript.QueryFloat("mat.diffuse.b") / 255f);
            mat.AmbientColor = new Color3D(Maxscript.QueryFloat("mat.ambient.r") / 255f,
                Maxscript.QueryFloat("mat.ambient.g") / 255f,
                Maxscript.QueryFloat("mat.ambient.b") / 255f);
            mat.SpecularColor = new Color3D(Maxscript.QueryFloat("mat.specular.r") / 255f,
                Maxscript.QueryFloat("mat.specular.g") / 255f,
                Maxscript.QueryFloat("mat.specular.b") / 255f);
            mat.EmissiveColor = new Color3D(Maxscript.QueryFloat("mat.selfIllumColor.r") / 255f,
                Maxscript.QueryFloat("mat.selfIllumColor.g") / 255f,
                Maxscript.QueryFloat("mat.selfIllumColor.b") / 255f);
            mat.Opacity = Maxscript.QueryFloat("mat.opacity") / 100f;
            mat.SpecularExponent = Maxscript.QueryFloat("mat.specularLevel");
            int opacityType = Maxscript.QueryInteger("mat.opacityType");

            if (Maxscript.QueryBoolean("(classof mat.diffusemap) == BitmapTexture"))
            {
                mat.TextureDataExtensionIndex = File.AddDataExtension(Maxscript.QueryString("mat.diffusemap.name"));
                File.SetDataExtensionFileName(mat.TextureDataExtensionIndex, Maxscript.QueryString("mat.diffusemap.filename"));
            }
            else if (Maxscript.QueryBoolean("(classof mat.diffusemap) == CompositeTextureMap") && Maxscript.QueryBoolean("(classof mat.diffusemap.mapList[1]) == BitmapTexture"))
            {
                mat.TextureDataExtensionIndex = File.AddDataExtension(Maxscript.QueryString("mat.diffusemap.mapList[1].name"));
                File.SetDataExtensionFileName(mat.TextureDataExtensionIndex, Maxscript.QueryString("mat.diffusemap.mapList[1].filename"));
            }
        }

        private static string CreateBone(GrnBone bone)
        {
            string boneNode = "boneNode";

            string bPos = Maxscript.NewPoint3<float>("bPos", bone.Position.X, bone.Position.Y, bone.Position.Z);
            Maxscript.Command("bRot = quat {0} {1} {2} {3}", bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z, bone.Rotation.W);
            Maxscript.Command("boneNode = dummy name:\"{0}\" rotation:{1} position:{2} scale:{3} boxsize:{4}",
                bone.Name, "bRot", "bPos",
                Maxscript.Point3Literal(bone.Scale.A1, bone.Scale.B2, bone.Scale.C3), "[0.25,0.25,0.25]");

            //Maxscript.Command("boneNode = dummy name:\"{0}\"", bone.Name);
            //Maxscript.Command("boneNode.transform = {0}", GrnMax.GetBoneLocalTransform(bone, "boneTransMat"));
            //Maxscript.Command("boneNode.boxsize = [0.25,0.25,0.25]");

            //GrnBone bone = MeshFile.Bones[boneIndex];
            //string world = GetBoneWorldTransform(MeshFile, boneIndex, "m3World");
            //string worldP = GetBoneWorldTransform(MeshFile, bone.ParentIndex, "m3WorldP");
            //Maxscript.Command("{0} = BoneSys.createBone {1}.translation {2}.translation [0, 0, 1]", boneNode, world, worldP);
            //Maxscript.Command("{0}.transform = {1}", boneNode, world);

            return boneNode;
        }
        private static string GetBoneLocalTransform(GrnBone bone, string nameM3)
        {
            Maxscript.Command("{0} = matrix3 0", nameM3);
            Maxscript.Command("{0}.rotation = quat {1} {2} {3} -{4}", nameM3, bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z, bone.Rotation.W);
            //Maxscript.Command("{0}.rotation = {1}", nameM3, Maxscript.QuatLiteral(bone.Rotation));
            Maxscript.Command("{0}.translation = {1}", nameM3, Maxscript.Point3Literal(bone.Position));
            Maxscript.Command("{0} *= scaleMatrix {1}", nameM3, 
                Maxscript.Point3Literal(bone.Scale.A1, bone.Scale.B2, bone.Scale.C3));

            return nameM3;
        }
        private static string GetBoneWorldTransform(GrnFile file, int boneIndex, string nameM3)
        {
            GrnBone bone = file.Bones[boneIndex];
            if (bone.ParentIndex != boneIndex)
            {
                Maxscript.Command("{0} = {1} * {2}",
                    nameM3, GetBoneLocalTransform(bone, nameM3 + boneIndex), 
                    GetBoneWorldTransform(file, bone.ParentIndex, nameM3 + bone.ParentIndex));
            }
            else
            {
                Maxscript.Command("{0} = {1}", nameM3, GetBoneLocalTransform(bone, nameM3 + boneIndex));
            }

            return nameM3;
        }
    }
}
