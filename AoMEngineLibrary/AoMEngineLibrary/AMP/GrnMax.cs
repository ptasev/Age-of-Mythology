namespace AoMEngineLibrary
{
    using AoMEngineLibrary.Graphics;
    using AoMEngineLibrary.Graphics.Grn;
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public class GrnMax : IModelMaxUi
    {
        public GrnFile File { get; set; }
        public MaxPluginForm Plugin { get; set; }
        public string FileName { get; set; }
        public int FilterIndex { get { return 2; } }

        private Dictionary<string, int> boneMap;
        private bool matGroupInit;

        private GrnExportSetting ExportSetting { get; set; }

        public GrnMax(MaxPluginForm plugin)
        {
            this.File = new GrnFile();
            this.FileName = "Untitled";
            this.Plugin = plugin;
            this.boneMap = new Dictionary<string, int>();
            this.matGroupInit = false;
            this.ExportSetting = GrnExportSetting.Model;
        }

        #region Setup
        public void Read(FileStream stream)
        {
            this.File = new GrnFile();
            this.File.Read(stream);
            this.FileName = stream.Name;
        }
        public void Write(FileStream stream)
        {
            this.File.Write(stream);
            this.FileName = stream.Name;
        }
        public void Clear()
        {
            this.File = new GrnFile();
            this.FileName = Path.GetDirectoryName(this.FileName) + "\\Untitled";
            this.boneMap = new Dictionary<string, int>();
            this.matGroupInit = false;
        }
        #endregion

        #region Import/Export
        public void Import()
        {
            this.Plugin.richTextBox1.AppendText(this.boneMap.Count.ToString() + Environment.NewLine + Environment.NewLine);
            string mainObject = "mainObject";
            string boneArray = "boneArray";

            if (!this.matGroupInit)
            {
                Maxscript.Command("matGroup = multimaterial numsubs:{0}", this.File.Materials.Count);
                this.matGroupInit = true;
            }

            this.ImportSkeleton(boneArray);

            for (int i = 0; i < this.File.Meshes.Count; i++)
            {
                this.ImportMesh(this.File.Meshes[i], mainObject, boneArray);
                Maxscript.Command("select {0}", mainObject);
                Maxscript.Command("{0}.material = matGroup", mainObject);
            }

            if (this.File.Animation.Duration > 0)
            {
                this.ImportAnimation(boneArray);
            }

            for (int i = 0; i < this.File.Materials.Count; i++)
            {
                Maxscript.Command("matGroup[{0}] = {1}", i + 1, ImportMaterial(this.File.Materials[i]));
            }

            Maxscript.Command("max zoomext sel all");
        }
        private void ImportSkeleton(string boneArray)
        {
            if (this.boneMap.Count == 0)
            {
                Maxscript.NewArray(boneArray);
            }

            for (int i = 0; i < this.File.Bones.Count; ++i)
            {
                GrnBone bone = this.File.Bones[i];
                if (bone.Name == "__Root")
                {
                    continue;
                }

                if (this.boneMap.ContainsKey(bone.Name))
                {
                    string bPos = Maxscript.NewPoint3<float>("bPos", bone.Position.X, bone.Position.Y, bone.Position.Z);
                    //Maxscript.Command("bRot = quat {0} {1} {2} {3}", bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z, bone.Rotation.W);
                    //Maxscript.Command("{0}[{1}].rotation = {2}", boneArray, this.boneMap[bone.Name] + 1, "bRot");
                    //Maxscript.Command("{0}[{1}].position = {2}", boneArray, this.boneMap[bone.Name] + 1, bPos);
                    //Maxscript.Command("{0}[{1}].scale = {2}", boneArray, this.boneMap[bone.Name] + 1,
                    //    Maxscript.Point3Literal(bone.Scale.A1, bone.Scale.B2, bone.Scale.C3));
                    Maxscript.Command("{0}[{1}].transform = {2}", boneArray, this.boneMap[bone.Name] + 1,
                        this.GetBoneLocalTransform(bone, "boneTransMat"));

                    //Maxscript.Command("{0}[{1}].transform = {2}", boneArray, boneMap[bone.Name] + 1, 
                    //    GrnMax.GetBoneLocalTransform(bone, "boneTransMat"));
                }
                else
                {
                    this.boneMap.Add(bone.Name, this.boneMap.Count);
                    this.CreateBone(bone);
                    Maxscript.Append(boneArray, "boneNode");
                }

                if (bone.ParentIndex > 0)
                {
                    Maxscript.Command("{0}[{1}].parent = {0}[{2}]", boneArray, this.boneMap[bone.Name] + 1,
                        this.boneMap[this.File.Bones[bone.ParentIndex].Name] + 1);
                    Maxscript.Command("{0}[{1}].transform *= {0}[{1}].parent.transform", boneArray, this.boneMap[bone.Name] + 1);
                }
                //this.Plugin.richTextBox1.AppendText(i + " " + bone.Name + " " + bone.ParentIndex + " " + Environment.NewLine);
                this.Plugin.richTextBox1.AppendText(i + " " + bone.Name + " " + bone.Scale + " " + Environment.NewLine);
            }

            //List<GrnBone> bones = this.File.Bones.OrderBy(b => b.ParentIndex).ToList();
            //for (int i = 0; i < bones.Count; ++i)
            //{
            //    GrnBone bone = bones[i];
            //    if (bone.Name == "__Root")
            //    {
            //        continue;
            //    }

            //    this.Plugin.richTextBox1.AppendText(i + " " + bone.Name + " " + bone.ParentIndex + " " + Environment.NewLine);
            //}
        }
        private void ImportMesh(GrnMesh mesh, string mainObject, string boneArray)
        {
            string vertArray = "";
            //string normArray = "";
            string texVerts = "";
            string faceMats = "";
            string faceArray = "";
            string tFaceArray = "";
            vertArray = Maxscript.NewArray("vertArray");
            //normArray = Maxscript.NewArray("normArray");
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
                //normArray[face.Indices[0]] = face.NormalIndices[0];
                //normArray[face.Indices[1]] = face.NormalIndices[1];
                //normArray[face.Indices[2]] = face.NormalIndices[2];
                //Maxscript.Command("{0}[{1}] = {2}", normArray, face.Indices[0] + 1,
                //    Maxscript.Point3Literal(mesh.Normals[face.NormalIndices[0]]));
                //Maxscript.Command("{0}[{1}] = {2}", normArray, face.Indices[1] + 1,
                //    Maxscript.Point3Literal(mesh.Normals[face.NormalIndices[1]]));
                //Maxscript.Command("{0}[{1}] = {2}", normArray, face.Indices[2] + 1,
                //    Maxscript.Point3Literal(mesh.Normals[face.NormalIndices[2]]));
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
                Maxscript.Command("setTVFace {0} {1} {2}[{1}]", mainObject, i + 1, tFaceArray);
            }

            Maxscript.Command("max modify mode");
            Maxscript.Command("addModifier {0} (Edit_Normals())", mainObject);
            Maxscript.Command("modPanel.setCurrentObject {0}.modifiers[#edit_normals]", mainObject);
            for (int i = 0; i < mesh.Faces.Count; ++i)
            {
                Maxscript.Command("{0}.modifiers[#edit_normals].SetNormalID {1} {2} {3}",
                    mainObject, i + 1, 1, mesh.Faces[i].NormalIndices[0] + 1);
                Maxscript.Command("{0}.modifiers[#edit_normals].SetNormalID {1} {2} {3}",
                    mainObject, i + 1, 2, mesh.Faces[i].NormalIndices[1] + 1);
                Maxscript.Command("{0}.modifiers[#edit_normals].SetNormalID {1} {2} {3}",
                    mainObject, i + 1, 3, mesh.Faces[i].NormalIndices[2] + 1);
            }
            for (int i = 0; i < mesh.Normals.Count; i++)
            {
                //Maxscript.Command("setNormal {0} {1} {2}[{1}]", mainObject, i + 1, normArray);
                //Maxscript.Command("setNormal {0} {1} {2}", mainObject, i + 1, Maxscript.Point3Literal(mesh.Normals[normArray[i]]));
                Maxscript.Command("{0}.modifiers[#edit_normals].SetNormalExplicit {1}", mainObject, i + 1);
                Maxscript.Command("{0}.modifiers[#edit_normals].SetNormal {1} {2}", mainObject, i + 1,
                    Maxscript.Point3Literal(mesh.Normals[i]));
            }
            Maxscript.Command("maxOps.CollapseNodeTo {0} 1 true", mainObject);

            // Bones
            Maxscript.Command("skinMod = Skin()");
            Maxscript.Command("addModifier {0} skinMod", mainObject);
            Maxscript.Command("modPanel.setCurrentObject skinMod");
            for (int i = 0; i < mesh.BoneBindings.Count; ++i)
            {
                GrnBoneBinding boneBinding = mesh.BoneBindings[i];
                string boundingScale = Maxscript.NewPoint3<float>("boundingScale",
                    (boneBinding.OBBMax.X - boneBinding.OBBMin.X), (boneBinding.OBBMax.Y - boneBinding.OBBMin.Y), (boneBinding.OBBMax.Z - boneBinding.OBBMin.Z));
                Maxscript.Command("{0}[{1}].boxsize = {2}", boneArray, this.boneMap[this.File.Bones[boneBinding.BoneIndex].Name] + 1, boundingScale);
                Maxscript.Command("skinOps.addBone skinMod {0}[{1}] {2}", boneArray,
                    this.boneMap[this.File.Bones[boneBinding.BoneIndex].Name] + 1, i + 1 == mesh.BoneBindings.Count ? 1 : 0);
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
        private void ImportAnimation(string boneArray)
        {
            Maxscript.Command("frameRate = {0}", 30);
            Maxscript.Interval(0, this.File.Animation.Duration);

            for (int i = 0; i < this.File.Animation.BoneTracks.Count; ++i)
            {
                if (this.File.Bones[i].Name == "__Root")
                {
                    continue;
                }
                GrnBoneTrack bone = this.File.Animation.BoneTracks[i];
                // typically bones and bonetracks match up
                // but won't if an anim file is imported on top of a regular model file
                int boneArrayIndex = this.boneMap[this.File.Bones[i].Name] + 1;
                this.Plugin.richTextBox1.AppendText(i + " " + this.File.GetDataExtensionObjectName(bone.DataExtensionIndex) + " " + Maxscript.QueryString("boneArray[{0}].name", boneArrayIndex) + Environment.NewLine);

                Maxscript.Command("{0}[{1}][3][1].controller = Bezier_Position()", boneArray, boneArrayIndex);
                for (int j = 0; j < bone.PositionKeys.Count; ++j)
                {
                    //Maxscript.Command("addNewKey {0}[{1}][3][1].controller {2}s", boneArray, boneArrayIndex, bone.PositionKeys[j]);
                    //Maxscript.Command("{0}[{1}][3][1].controller.keys[{2}].value = {3}",
                    //    boneArray, boneArrayIndex, j + 1, Maxscript.Point3Literal(bone.Positions[j]));
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

                Maxscript.Command("{0}[{1}][3][3].controller = Bezier_Scale()", boneArray, boneArrayIndex);
                for (int j = 0; j < bone.ScaleKeys.Count; ++j)
                {
                    Maxscript.Command("addNewKey {0}[{1}][3][3].controller {2}s", boneArray, boneArrayIndex, bone.ScaleKeys[j]);
                    Maxscript.Command("{0}[{1}][3][3].controller.keys[{2}].value = {3}", boneArray, boneArrayIndex, j + 1,
                        Maxscript.Point3Literal(bone.Scales[j].A1, bone.Scales[j].B2, bone.Scales[j].C3));
                }
            }
        }
        private string ImportMaterial(GrnMaterial mat)
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
                Maxscript.Command("pcTex.filename = \"{0}\"", Path.GetFileName(mat.DiffuseMapFileName));

                Maxscript.Command("pcTex2 = BitmapTexture()");
                Maxscript.Command("pcTex2.name = \"{0}\"", mat.DiffuseMap);
                Maxscript.Command("pcTex2.filename = \"{0}\"", Path.GetFileName(mat.DiffuseMapFileName));
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
                Maxscript.Command("tex.filename = \"{0}\"", Path.GetFileName(mat.DiffuseMapFileName));
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

        public void Export()
        {
            this.Clear();

            Maxscript.Command("ExportGrnData()");

            this.ExportSkeleton();

            if (this.ExportSetting.HasFlag(GrnExportSetting.Model))
            {
                int meshCount = Maxscript.QueryInteger("grnMeshes.count");
                for (int i = 0; i < meshCount; ++i)
                {
                    this.File.Meshes.Add(new GrnMesh(this.File));
                    this.ExportMesh(i);
                }
            }

            if (this.ExportSetting.HasFlag(GrnExportSetting.Animation))
            {
                this.ExportAnimation();
                if (this.File.Animation.Duration == 0f)
                {
                    this.File.Animation.Duration = 1f;
                }
                this.File.Animation.TimeStep = 1f / 60f;
            }
            else
            {
                this.File.Animation.Duration = 0f;
                this.File.Animation.TimeStep = 1f;
            }

            if (this.ExportSetting.HasFlag(GrnExportSetting.Model))
            {
                int numMaterials = Maxscript.QueryInteger("{0}.material.materialList.count", "mainObject");
                for (int i = 0; i < numMaterials; i++)
                {
                    this.File.Materials.Add(new GrnMaterial(this.File));
                    this.ExportMaterial(i, "mainObject");
                }
            }
        }
        private void ExportSkeleton()
        {
            int numBones = Maxscript.QueryInteger("grnBones.count");
            this.File.Bones.Add(new GrnBone(this.File));
            this.File.Bones[0].DataExtensionIndex = this.File.AddDataExtension("__Root");
            this.File.Bones[0].Rotation = new Quaternion(1, 0, 0, 0);
            this.File.Bones[0].ParentIndex = 0;

            for (int i = 1; i <= numBones; ++i)
            {
                if (!this.ExportSetting.HasFlag(GrnExportSetting.Model) &&
                    this.ExportSetting.HasFlag(GrnExportSetting.Animation) &&
                    Maxscript.QueryBoolean("grnBones[{0}].isanimated == false", i))
                {
                    continue;
                }

                try
                {
                    GrnBone bone = new GrnBone(this.File);
                    bone.DataExtensionIndex = this.File.AddDataExtension(Maxscript.QueryString("grnBones[{0}].name", i));
                    bone.ParentIndex = Maxscript.QueryInteger("grnBoneParents[{0}]", i);

                    Maxscript.Command("copiedBone = copy grnBones[{0}]", i);
                    Maxscript.Command("boneTransMat = grnBones[{0}].transform", i);
                    if (bone.ParentIndex > 0)
                    {
                        Maxscript.Command("boneTransMat = boneTransMat * inverse(grnBones[{0}].parent.transform)", i);
                        //Maxscript.Command("copiedBone.transform *= (inverse copiedBone.parent.transform)");
                    }
                    Maxscript.Command("bScale = boneTransMat.scale");
                    //Maxscript.Command("boneTransMat = inverse(scalematrix bScale) * boneTransMat", i);
                    Maxscript.Command("bRot = inverse(boneTransMat.rotation)");
                    Maxscript.Command("boneScTransMat = boneTransMat * inverse(boneTransMat.rotation as matrix3) --boneTransMat");
                    Maxscript.Command("boneTransMat = inverse(boneTransMat.rotation as matrix3) * boneTransMat", i);
                    Maxscript.Command("bPos = boneTransMat.position");
                    Maxscript.Command("bScale = boneTransMat.scale");

                    bone.Position = new Vector3D(
                            Maxscript.QueryFloat("bPos.x"),
                            Maxscript.QueryFloat("bPos.y"),
                            Maxscript.QueryFloat("bPos.z"));
                    bone.Rotation = new Quaternion(
                        Maxscript.QueryFloat("bRot.w"),
                        Maxscript.QueryFloat("bRot.x"),
                        Maxscript.QueryFloat("bRot.y"),
                        Maxscript.QueryFloat("bRot.z"));
                    Matrix3x3 scale = new Matrix3x3();
                    scale.A1 = Maxscript.QueryFloat("boneScTransMat.row1.x");
                    scale.A2 = Maxscript.QueryFloat("boneScTransMat.row1.y");
                    scale.A3 = Maxscript.QueryFloat("boneScTransMat.row1.z");
                    scale.B1 = Maxscript.QueryFloat("boneScTransMat.row2.x");
                    scale.B2 = Maxscript.QueryFloat("boneScTransMat.row2.y");
                    scale.B3 = Maxscript.QueryFloat("boneScTransMat.row2.z");
                    scale.C1 = Maxscript.QueryFloat("boneScTransMat.row3.x");
                    scale.C2 = Maxscript.QueryFloat("boneScTransMat.row3.y");
                    scale.C3 = Maxscript.QueryFloat("boneScTransMat.row3.z");

                    //bone.Position = new Vector3D(
                    //        Maxscript.QueryFloat("(coordsys parent copiedBone.position).x"),
                    //        Maxscript.QueryFloat("(coordsys parent copiedBone.position).y"),
                    //        Maxscript.QueryFloat("(coordsys parent copiedBone.position).z"));
                    //bone.Rotation = new Quaternion(
                    //    Maxscript.QueryFloat("(coordsys parent copiedBone.rotation).w"),
                    //    Maxscript.QueryFloat("(coordsys parent copiedBone.rotation).x"),
                    //    Maxscript.QueryFloat("(coordsys parent copiedBone.rotation).y"),
                    //    Maxscript.QueryFloat("(coordsys parent copiedBone.rotation).z"));
                    //Matrix3x3 scale = new Matrix3x3();
                    //scale.A1 = Maxscript.QueryFloat("(coordsys parent copiedBone.scale).x");
                    //scale.B2 = Maxscript.QueryFloat("(coordsys parent copiedBone.scale).y");
                    //scale.C3 = Maxscript.QueryFloat("(coordsys parent copiedBone.scale).z");

                    Maxscript.Command("delete copiedBone");
                    bone.Scale = scale;
                    this.File.Bones.Add(bone);
                    //this.Plugin.richTextBox1.AppendText(i + " " + bone.Name + Environment.NewLine);
                    //this.Plugin.richTextBox1.AppendText(bone.Rotation.ToString() + Environment.NewLine);
                    this.Plugin.richTextBox1.AppendText(i + " " + bone.Name + " " + bone.Scale + " " + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    throw new Exception("Bone Index = " + i, ex);
                }
            }
        }
        private void ExportMesh(int meshIndex)
        {
            GrnMesh mesh = this.File.Meshes[meshIndex];
            bool hadEditNormMod = false;
            string mainObject = "mainObject";
            Maxscript.Command("{0} = grnMeshes[{1}]", mainObject, meshIndex + 1);
            mesh.DataExtensionIndex = this.File.AddDataExtension(Maxscript.QueryString("{0}.name", mainObject));

            // Setup Normals
            Maxscript.Command("max modify mode");
            if (Maxscript.QueryBoolean("{0}.modifiers[#edit_normals] == undefined", mainObject))
            {
                Maxscript.Command("addModifier {0} (Edit_Normals())", mainObject);
            }
            else { hadEditNormMod = true; }
            Maxscript.Command("modPanel.setCurrentObject {0}.modifiers[#edit_normals] ui:true", mainObject);
            //Maxscript.Command("CalculateAveragedNormals {0}", mainObject);

            int numVertices = Maxscript.QueryInteger("meshop.getnumverts {0}", mainObject);
            int numFaces = Maxscript.QueryInteger("meshop.getnumfaces {0}", mainObject);
            int numNorms = Maxscript.QueryInteger("{0}.modifiers[#edit_normals].GetNumNormals()", mainObject);

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

            for (int i = 0; i < numNorms; ++i)
            {
                    Maxscript.Command("currentNormal = {0}.modifiers[#edit_normals].GetNormal {1}", 
                        mainObject, i + 1);
                    mesh.Normals.Add(new Vector3D(
                        Maxscript.QueryFloat("currentNormal.x"),
                        Maxscript.QueryFloat("currentNormal.y"),
                        Maxscript.QueryFloat("currentNormal.z")));
            }

            int numTexVertices = Maxscript.QueryInteger("meshop.getnumtverts {0}", mainObject);
            for (int i = 0; i < numTexVertices; i++)
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
                f.NormalIndices.Add(Maxscript.QueryInteger("{0}.modifiers[#edit_normals].GetNormalID {1} {2}",
                    mainObject, i + 1, 1) - 1);
                f.NormalIndices.Add(Maxscript.QueryInteger("{0}.modifiers[#edit_normals].GetNormalID {1} {2}",
                    mainObject, i + 1, 2) - 1);
                f.NormalIndices.Add(Maxscript.QueryInteger("{0}.modifiers[#edit_normals].GetNormalID {1} {2}",
                    mainObject, i + 1, 3) - 1);
                Maxscript.Command("tFace = getTVFace {0} {1}", mainObject, i + 1);
                f.TextureIndices.Add(Maxscript.QueryInteger("tFace.x") - 1);
                f.TextureIndices.Add(Maxscript.QueryInteger("tFace.y") - 1);
                f.TextureIndices.Add(Maxscript.QueryInteger("tFace.z") - 1);
                mesh.Faces.Add(f);
            }
            // Delete normals mod if it wasn't there in the first place
            if (!hadEditNormMod)
            {
                Maxscript.Command("deleteModifier {0} {0}.modifiers[#edit_normals]", mainObject);
            }

            if (Maxscript.QueryBoolean("{0}.modifiers[#skin] != undefined", mainObject))
            {
                Maxscript.Command("skinMod = {0}.modifiers[#skin]", mainObject);
                Maxscript.Command("modPanel.setCurrentObject skinMod ui:true");
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
                        mesh.VertexWeights[i].Weights.Add(Maxscript.QueryFloat("skinWeightArray[{0}][2]", j + 1));
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
        private void ExportAnimation()
        {
            int numBones = Maxscript.QueryInteger("grnBones.count");
            this.File.Animation.Duration = 0f;
            GrnBoneTrack rootBoneTrack = new GrnBoneTrack();
            HashSet<float> rootTrackKeys = new HashSet<float>();
            rootBoneTrack.DataExtensionIndex = this.File.Bones[0].DataExtensionIndex;
            this.File.Animation.BoneTracks.Add(rootBoneTrack);

            for (int i = 1; i <= numBones; ++i)
            {
                if (Maxscript.QueryBoolean("grnBones[{0}].isanimated == false", i))
                {
                    continue;
                }

                GrnBoneTrack bone = new GrnBoneTrack();
                bone.DataExtensionIndex = this.File.Bones[i].DataExtensionIndex;

                int numPosKeys = Maxscript.QueryInteger("grnBones[{0}][3][1].controller.keys.count", i);
                Maxscript.Command("sort grnBones[{0}][3][1].controller.keys", i);
                for (int j = 0; j < numPosKeys; ++j)
                {
                    try
                    {
                        bone.PositionKeys.Add(Maxscript.QueryFloat("grnBones[{0}][3][1].controller.keys[{1}].time as float / 4800", i, j + 1));
                        bone.Positions.Add(new Vector3D(
                            Maxscript.QueryFloat("grnBones[{0}][3][1][1].controller.keys[{1}].value", i, j + 1),
                            Maxscript.QueryFloat("grnBones[{0}][3][1][2].controller.keys[{1}].value", i, j + 1),
                            Maxscript.QueryFloat("grnBones[{0}][3][1][3].controller.keys[{1}].value", i, j + 1)));
                    }
                    catch (Exception ex) { throw new Exception("Pos Controller Bone " + i + " key " + (j + 1), ex); }
                }

                int numRotKeys = Maxscript.QueryInteger("grnBones[{0}][3][2].controller.keys.count", i);
                for (int j = 0; j < numRotKeys; ++j)
                {
                    bone.RotationKeys.Add(Maxscript.QueryFloat("grnBones[{0}][3][2].controller.keys[{1}].time as float / 4800", i, j + 1));
                    Maxscript.Command("rotQuat = grnBones[{0}][3][2].controller.keys[{1}].value as quat", i, j + 1);
                    bone.Rotations.Add(new Quaternion(
                        Maxscript.QueryFloat("rotQuat.w"),
                        Maxscript.QueryFloat("rotQuat.x"),
                        Maxscript.QueryFloat("rotQuat.y"),
                        Maxscript.QueryFloat("rotQuat.z")));
                }

                int numScaleKeys = Maxscript.QueryInteger("grnBones[{0}][3][3].controller.keys.count", i);
                for (int j = 0; j < numScaleKeys; ++j)
                {
                    bone.ScaleKeys.Add(Maxscript.QueryFloat("grnBones[{0}][3][3].controller.keys[{1}].time as float / 4800", i, j + 1));
                    Maxscript.Command("bTrackScale = grnBones[{0}][3][3].controller.keys[{1}].value", i, j + 1);
                    Matrix3x3 scaleMatrix = new Matrix3x3();
                    scaleMatrix.A1 = Maxscript.QueryFloat("bTrackScale.x");
                    scaleMatrix.B2 = Maxscript.QueryFloat("bTrackScale.y");
                    scaleMatrix.C3 = Maxscript.QueryFloat("bTrackScale.z");
                    bone.Scales.Add(scaleMatrix);
                }

                //this.Plugin.richTextBox1.AppendText(i + " " + Maxscript.QueryString("grnBones[{0}].name", i) + Environment.NewLine);
                //this.Plugin.richTextBox1.AppendText(bone.Rotations[0].ToString() + Environment.NewLine);
                this.File.Animation.BoneTracks.Add(bone);
                this.File.Animation.Duration = Math.Max(this.File.Animation.Duration, bone.PositionKeys.Last());
                this.File.Animation.Duration = Math.Max(this.File.Animation.Duration, bone.RotationKeys.Last());
                this.File.Animation.Duration = Math.Max(this.File.Animation.Duration, bone.ScaleKeys.Last());
            }

            // Add keys for the root node at 0, middle, end
            rootTrackKeys.Add(0);
            rootTrackKeys.Add(this.File.Animation.Duration / 2f);
            rootTrackKeys.Add(this.File.Animation.Duration);
            rootBoneTrack.PositionKeys.AddRange(rootTrackKeys);
            rootBoneTrack.RotationKeys.AddRange(rootTrackKeys);
            rootBoneTrack.ScaleKeys.AddRange(rootTrackKeys);
            for (int i = 0; i < rootTrackKeys.Count; ++i)
            {
                rootBoneTrack.Positions.Add(new Vector3D(0, 0, 0));
                rootBoneTrack.Rotations.Add(new Quaternion(1, 0, 0, 0));
                rootBoneTrack.Scales.Add(Matrix3x3.Identity);
            }
        }
        private void ExportMaterial(int matIndex, string mainObject)
        {
            GrnMaterial mat = this.File.Materials[matIndex];
            Maxscript.Command("mat = {0}.material[{1}]", mainObject, matIndex + 1);
            mat.DataExtensionIndex = this.File.AddDataExtension(Maxscript.QueryString("mat.name"));

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
                mat.TextureDataExtensionIndex = this.File.AddDataExtension(Maxscript.QueryString("mat.diffusemap.name"));
                this.File.SetDataExtensionFileName(mat.TextureDataExtensionIndex, Path.GetFileName(Maxscript.QueryString("mat.diffusemap.filename")));
            }
            else if (Maxscript.QueryBoolean("(classof mat.diffusemap) == CompositeTextureMap") && Maxscript.QueryBoolean("(classof mat.diffusemap.mapList[1]) == BitmapTexture"))
            {
                mat.TextureDataExtensionIndex = this.File.AddDataExtension(Maxscript.QueryString("mat.diffusemap.mapList[1].name"));
                this.File.SetDataExtensionFileName(mat.TextureDataExtensionIndex, Path.GetFileName(Maxscript.QueryString("mat.diffusemap.mapList[1].filename")));
            }
        }

        private string CreateBone(GrnBone bone)
        {
            string boneNode = "boneNode";

            string bPos = Maxscript.NewPoint3<float>("bPos", bone.Position.X, bone.Position.Y, bone.Position.Z);
            Maxscript.Command("bRot = quat {0} {1} {2} {3}", bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z, bone.Rotation.W);
            //Maxscript.Command("boneScaleMatrix = matrix3 [{0}, {1}, {2}] [{3}, {4}, {5}] [{6}, {7}, {8}] [0,0,0]",
            //    bone.Scale.A1, bone.Scale.A2, bone.Scale.A3,
            //    bone.Scale.B1, bone.Scale.B2, bone.Scale.B3,
            //    bone.Scale.C1, bone.Scale.C2, bone.Scale.C3);
            //Maxscript.Command("boneNode = dummy name:\"{0}\" boxsize:[0.25,0.25,0.25]", bone.Name);
            //Maxscript.Command("boneNode.scale = {0}", Maxscript.Point3Literal(bone.Scale.A1, bone.Scale.B2, bone.Scale.C3));
            //Maxscript.Command("boneNode.rotation = bRot");
            //Maxscript.Command("boneNode.position = bPos");
            //Maxscript.Command("boneNode.transform = boneScaleMatrix * boneNode.transform");
            //this.Plugin.richTextBox1.AppendText(bone.Name + " " + bone.Position + Environment.NewLine);

            Maxscript.Command("boneNode = dummy name:\"{0}\" boxsize:[0.25,0.25,0.25]", bone.Name);
            Maxscript.Command("boneNode.transform = {0}", this.GetBoneLocalTransform(bone, "boneTransMat"));
            //Maxscript.Command("boneNode.boxsize = [0.25,0.25,0.25]");

            //this.GetBoneLocalTransform(bone, "tfm");
            //Maxscript.Command("boneNode = bonesys.createbone tfm.row4 (tfm.row4 + 0.01 * (normalize tfm.row1)) (normalize tfm.row3)");
            //Maxscript.Command("boneNode.name = \"{0}\"", bone.Name);
            //Maxscript.Command("boneNode.width = 0.1");
            //Maxscript.Command("boneNode.height = 0.1");
            //Maxscript.Command("boneNode.wirecolor = yellow");
            //Maxscript.Command("boneNode.showlinks = true");
            //Maxscript.Command("boneNode.setBoneEnable false 0");
            //Maxscript.Command("boneNode.pos.controller = TCB_position ()");
            //Maxscript.Command("boneNode.rotation.controller = TCB_rotation ()");
            
            //GrnBone bone = MeshFile.Bones[boneIndex];
            //string world = GetBoneWorldTransform(MeshFile, boneIndex, "m3World");
            //string worldP = GetBoneWorldTransform(MeshFile, bone.ParentIndex, "m3WorldP");
            //Maxscript.Command("{0} = BoneSys.createBone {1}.translation {2}.translation [0, 0, 1]", boneNode, world, worldP);
            //Maxscript.Command("{0}.transform = {1}", boneNode, world);

            return boneNode;
        }
        private string GetBoneLocalTransform(GrnBone bone, string nameM3)
        {
            Maxscript.Command("{0} = matrix3 1", nameM3);
            //Maxscript.Command("{0} = (matrix3 [{1}, {2}, {3}] [{4}, {5}, {6}] [{7}, {8}, {9}] {10})", nameM3,
            //    bone.Scale.A1, bone.Scale.A2, bone.Scale.A3,
            //    bone.Scale.B1, bone.Scale.B2, bone.Scale.B3,
            //    bone.Scale.C1, bone.Scale.C2, bone.Scale.C3, Maxscript.Point3Literal(bone.Position));
            Maxscript.Command("{0} = transmatrix {1}", nameM3, Maxscript.Point3Literal(bone.Position));
            Maxscript.Command("{0} = (inverse(quat {1} {2} {3} {4}) as matrix3) * {0}", nameM3, bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z, bone.Rotation.W);
            //Maxscript.Command("{0}.scale = {1}", nameM3, Maxscript.Point3Literal(bone.Scale.A1, bone.Scale.B2, bone.Scale.C3));
            //Maxscript.Command("{0}.rotation = inverse(quat {1} {2} {3} {4})", nameM3, bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z, bone.Rotation.W);
            //Maxscript.Command("{0}.position = {1}", nameM3, Maxscript.Point3Literal(bone.Position));
            Maxscript.Command("{0} = (matrix3 [{1}, {2}, {3}] [{4}, {5}, {6}] [{7}, {8}, {9}] [0,0,0]) * {0}", nameM3,
                bone.Scale.A1, bone.Scale.A2, bone.Scale.A3,
                bone.Scale.B1, bone.Scale.B2, bone.Scale.B3,
                bone.Scale.C1, bone.Scale.C2, bone.Scale.C3);
            //Maxscript.Command("{0}.rotation = {1}", nameM3, Maxscript.QuatLiteral(bone.Rotation));
            //Maxscript.Command("{0} *= scaleMatrix {1}", nameM3, 
            //    Maxscript.Point3Literal(bone.Scale.A1, bone.Scale.B2, bone.Scale.C3));

            return nameM3;
        }
        private string GetBoneWorldTransform(GrnFile file, int boneIndex, string nameM3)
        {
            GrnBone bone = file.Bones[boneIndex];
            if (bone.ParentIndex != boneIndex)
            {
                Maxscript.Command("{0} = {1} * {2}",
                    nameM3, this.GetBoneLocalTransform(bone, nameM3 + boneIndex), 
                    GetBoneWorldTransform(file, bone.ParentIndex, nameM3 + bone.ParentIndex));
            }
            else
            {
                Maxscript.Command("{0} = {1}", nameM3, this.GetBoneLocalTransform(bone, nameM3 + boneIndex));
            }

            return nameM3;
        }
        private bool OnlyHasRootNode()
        {
            if (this.File.Bones.Count == 1 &&
                this.File.Bones[0].Name == "__Root")
            {
                return true;
            }

            return false;
        }
        #endregion

        #region UI
        public void LoadUi()
        {
            this.Plugin.Text = MaxPluginForm.PluginTitle + " - " + Path.GetFileName(this.FileName);

            this.LoadDataExtensions();

            if (this.File.Meshes.Count > 0)
            {
                this.Plugin.vertsValueToolStripStatusLabel.Text = this.File.Meshes[0].Vertices.Count.ToString();
                this.Plugin.facesValueToolStripStatusLabel.Text = this.File.Meshes[0].Faces.Count.ToString();
            }
            else
            {
                this.Plugin.vertsValueToolStripStatusLabel.Text = "0";
                this.Plugin.facesValueToolStripStatusLabel.Text = "0";
            }
            this.Plugin.meshesValueToolStripStatusLabel.Text = this.File.Meshes.Count.ToString();
            this.Plugin.animLengthValueToolStripStatusLabel.Text = this.File.Animation.Duration.ToString();

            this.Plugin.grnExportModelCheckBox.Checked = this.ExportSetting.HasFlag(GrnExportSetting.Model);
            this.Plugin.grnExportAnimCheckBox.Checked = this.ExportSetting.HasFlag(GrnExportSetting.Animation);
        }
        private void LoadDataExtensions()
        {
            this.Plugin.grnObjectsListBox.Items.Clear();
            for (int i = 0; i < this.File.DataExtensions.Count; ++i)
            {
                this.Plugin.grnObjectsListBox.Items.Add(this.File.GetDataExtensionObjectName(i));
            }
        }

        public void SaveUi()
        {
            // Export Settings
            this.ExportSetting = (GrnExportSetting)0;
            if (this.Plugin.grnExportModelCheckBox.Checked)
            {
                this.ExportSetting |= GrnExportSetting.Model;
            }
            if (this.Plugin.grnExportAnimCheckBox.Checked)
            {
                this.ExportSetting |= GrnExportSetting.Animation;
            }
        }
        #endregion

        [Flags]
        private enum GrnExportSetting
        {
            Model = 0x1,
            Animation = 0x2
        }
    }
}
