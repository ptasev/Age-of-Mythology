namespace AoMMaxPlugin
{
    using AoMEngineLibrary.Graphics;
    using AoMEngineLibrary.Graphics.Grn;
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public class GrnMax : IModelMaxUI
    {
        public GrnFile File { get; set; }
        public MaxPluginForm Plugin { get; set; }
        public string FileName { get; set; }
        public int FilterIndex { get { return 2; } }

        private Dictionary<string, int> boneMap;

        private GrnExportSetting ExportSetting { get; set; }

        public GrnMax(MaxPluginForm plugin)
        {
            this.File = new GrnFile();
            this.FileName = "Untitled";
            this.Plugin = plugin;
            this.boneMap = new Dictionary<string, int>();
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
        }
        #endregion

        #region Import/Export
        public void Import()
        {
            Maxscript.Command("importStartTime = timeStamp()");
            string mainObject = "mainObject";
            string boneArray = "boneArray";

            if (this.File.Meshes.Count > 0 || Maxscript.QueryBoolean("boneArray == undefined or not(isvalidnode boneArray[1])"))
            {
                this.boneMap = new Dictionary<string, int>();
            }

            if (this.File.Materials.Count > 0)
            {
                Maxscript.Command("matGroup = multimaterial numsubs:{0}", this.File.Materials.Count);
            }

            //this.Plugin.ProgDialog.SetProgressText("Importing skeleton...");
            this.ImportSkeleton(boneArray);
            //this.Plugin.ProgDialog.SetProgressValue(35);

            //this.Plugin.ProgDialog.SetProgressText("Importing meshes...");
            for (int i = 0; i < this.File.Meshes.Count; i++)
            {
                this.ImportMesh(this.File.Meshes[i], mainObject, boneArray);
                Maxscript.Command("{0}.material = matGroup", mainObject);
            }
            //this.Plugin.ProgDialog.SetProgressValue(70);

            //this.Plugin.ProgDialog.SetProgressText("Importing animation...");
            if (this.File.Animation.Duration > 0)
            {
                this.ImportAnimation(boneArray);
            }
            //this.Plugin.ProgDialog.SetProgressValue(85);

            //this.Plugin.ProgDialog.SetProgressText("Importing materials...");
            for (int i = 0; i < this.File.Materials.Count; i++)
            {
                Maxscript.Command("matGroup[{0}] = {1}", i + 1, ImportMaterial(this.File.Materials[i]));
            }
            //this.Plugin.ProgDialog.SetProgressValue(100);

            Maxscript.Command("max select none");
            Maxscript.Command("max zoomext sel all");
            Maxscript.Command("max zoomext sel all");
            Maxscript.Command("importEndTime = timeStamp()");
            Maxscript.Format("Import took % seconds\n", "((importEndTime - importStartTime) / 1000.0)");
            //if (this.Plugin.ProgDialog.InvokeRequired)
            //{
            //    this.Plugin.ProgDialog.BeginInvoke(new Action(() => this.Plugin.ProgDialog.Close()));
            //}
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
                    Maxscript.Command("{0}[{1}].transform = {2}", boneArray, this.boneMap[bone.Name] + 1,
                        this.GetBoneLocalTransform(bone, "boneTransMat"));
                }
                else
                {
                    this.boneMap.Add(bone.Name, this.boneMap.Count);
                    Maxscript.Append(boneArray, this.CreateBone(bone));
                }

                if (bone.ParentIndex > 0)
                {
                    Maxscript.Command("{0}[{1}].parent = {0}[{2}]", boneArray, this.boneMap[bone.Name] + 1,
                        this.boneMap[this.File.Bones[bone.ParentIndex].Name] + 1);
                    Maxscript.Command("{0}[{1}].transform *= {0}[{1}].parent.transform", boneArray, this.boneMap[bone.Name] + 1);
                }
            }
        }
        private void ImportMesh(GrnMesh mesh, string mainObject, string boneArray)
        {
            string vertArray = "";
            string texVerts = "";
            string faceMats = "";
            string faceArray = "";
            string tFaceArray = "";
            vertArray = Maxscript.NewArray("vertArray");
            texVerts = Maxscript.NewArray("texVerts");
            faceMats = Maxscript.NewArray("faceMats");
            faceArray = Maxscript.NewArray("faceArray");
            tFaceArray = Maxscript.NewArray("tFaceArray");

            for (int i = 0; i < mesh.Vertices.Count; ++i)
            {
                Maxscript.Append(vertArray, Maxscript.Point3Literal(mesh.Vertices[i]));
            }

            for (int i = 0; i < mesh.TextureCoordinates.Count; ++i)
            {
                Maxscript.Append(texVerts, Maxscript.Point3Literal(mesh.TextureCoordinates[i].X, 1f-mesh.TextureCoordinates[i].Y, mesh.TextureCoordinates[i].Z));
            }

            foreach (var face in mesh.Faces)
            {
                Maxscript.Append(faceMats, face.MaterialIndex + 1);
                Maxscript.Append(faceArray, Maxscript.Point3Literal(face.Indices[0] + 1, face.Indices[1] + 1, face.Indices[2] + 1));
                Maxscript.Append(tFaceArray, Maxscript.Point3Literal(face.TextureIndices[0] + 1, face.TextureIndices[1] + 1, face.TextureIndices[2] + 1));
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
            Maxscript.Command("select {0}", mainObject);
            Maxscript.Command("addModifier {0} (Edit_Normals()) ui:off", mainObject);
            Maxscript.Command("modPanel.setCurrentObject {0}.modifiers[#edit_normals]", mainObject);

            Maxscript.Command("{0}.modifiers[#edit_normals].Break selection:#{{1..{1}}}", mainObject, mesh.Normals.Count);
            Maxscript.Command("meshSetNormalIdFunc = {0}.modifiers[#edit_normals].SetNormalID", mainObject);
            for (int i = 0; i < mesh.Faces.Count; ++i)
            {
                Maxscript.Command("meshSetNormalIdFunc {0} {1} {2}",
                    i + 1, 1, mesh.Faces[i].NormalIndices[0] + 1);
                Maxscript.Command("meshSetNormalIdFunc {0} {1} {2}",
                    i + 1, 2, mesh.Faces[i].NormalIndices[1] + 1);
                Maxscript.Command("meshSetNormalIdFunc {0} {1} {2}",
                    i + 1, 3, mesh.Faces[i].NormalIndices[2] + 1);
            }
            Maxscript.Command("{0}.modifiers[#edit_normals].MakeExplicit selection:#{{1..{1}}}", mainObject, mesh.Normals.Count);
            Maxscript.Command("meshSetNormalFunc = {0}.modifiers[#edit_normals].SetNormal", mainObject);
            for (int i = 0; i < mesh.Normals.Count; i++)
            {
                Maxscript.Command("meshSetNormalFunc {0} {1}", i + 1, Maxscript.Point3Literal(mesh.Normals[i]));
            }
            Maxscript.Command("collapseStack {0}", mainObject);

            // Bones
            Maxscript.Command("skinMod = Skin()");
            Maxscript.Command("addModifier {0} skinMod", mainObject);
            Maxscript.Command("modPanel.setCurrentObject skinMod");
            Maxscript.Command("skinAddBoneFunc = skinOps.addBone");
            for (int i = 0; i < mesh.BoneBindings.Count; ++i)
            {
                GrnBoneBinding boneBinding = mesh.BoneBindings[i];
                string boundingScale = Maxscript.NewPoint3<float>("boundingScale",
                    (boneBinding.OBBMax.X - boneBinding.OBBMin.X), (boneBinding.OBBMax.Y - boneBinding.OBBMin.Y), (boneBinding.OBBMax.Z - boneBinding.OBBMin.Z));
                Maxscript.Command("{0}[{1}].boxsize = {2}", boneArray, this.boneMap[this.File.Bones[boneBinding.BoneIndex].Name] + 1, boundingScale);
                Maxscript.Command("skinAddBoneFunc skinMod {0}[{1}] {2}", boneArray,
                    this.boneMap[this.File.Bones[boneBinding.BoneIndex].Name] + 1, i + 1 == mesh.BoneBindings.Count ? 1 : 0);
            }
            Maxscript.Command("completeRedraw()"); // would get "Exceeded the vertex countSkin:skin" error without this
            Maxscript.Command("skinReplaceVertWeightsFunc = skinOps.ReplaceVertexWeights");
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
                Maxscript.Command("skinReplaceVertWeightsFunc skinMod {0} {1} {2}",
                    i + 1, 1, 0.0);
                Maxscript.Command("skinReplaceVertWeightsFunc skinMod {0} {1} {2}",
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
                // typically bones and bonetracks match up in a file
                // but won't if an anim file is imported on top of a regular model file
                int boneArrayIndex = this.boneMap[this.File.Bones[i].Name] + 1;

                Vector3 pos = new Vector3();
                Quaternion rot = new Quaternion();
                Matrix4x4 scale = Matrix4x4.Identity;
                HashSet<float> uKeys = new HashSet<float>();
                uKeys.UnionWith(bone.RotationKeys);
                uKeys.UnionWith(bone.ScaleKeys);
                uKeys.UnionWith(bone.PositionKeys);
                List<float> keys = uKeys.ToList();
                keys.Sort();

                for (int j = 0; j < keys.Count; ++j)
                {
                    int index = bone.PositionKeys.IndexOf(keys[j]);
                    if (index >= 0)
                    {
                        pos = bone.Positions[index];
                    }
                    index = bone.RotationKeys.IndexOf(keys[j]);
                    if (index >= 0)
                    {
                        rot = bone.Rotations[index];
                    }
                    index = bone.ScaleKeys.IndexOf(keys[j]);
                    if (index >= 0)
                    {
                        scale = bone.Scales[index];
                    }

                    Maxscript.AnimateAtTime(keys[j], "{0}[{1}][3].controller.value = {2}",
                        boneArray, boneArrayIndex, this.GetBoneLocalTransform("bAnimMatrix", pos, rot, scale));
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
            Maxscript.Command("tex.name = \"{0}\"", mat.DiffuseTexture.Name);
            Maxscript.Command("tex.filename = \"{0}\"", Path.GetFileName(mat.DiffuseTexture.FileName));
            Maxscript.Command("mat.diffusemap = tex");

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
            Maxscript.Command("exportStartTime = timeStamp()");
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

            Maxscript.Command("exportEndTime = timeStamp()");
            Maxscript.Format("Export took % seconds\n", "((exportEndTime - exportStartTime) / 1000.0)");
        }
        private void ExportSkeleton()
        {
            int numBones = Maxscript.QueryInteger("grnBones.count");
            this.File.Bones.Add(new GrnBone(this.File));
            this.File.Bones[0].DataExtensionIndex = this.File.AddDataExtension("__Root");
            this.File.Bones[0].Rotation = new Quaternion(0, 0, 0, 1);
            this.File.Bones[0].ParentIndex = 0;

            for (int i = 1; i <= numBones; ++i)
            {
                try
                {
                    GrnBone bone = new GrnBone(this.File);
                    bone.DataExtensionIndex = this.File.AddDataExtension(Maxscript.QueryString("grnBones[{0}].name", i));
                    bone.ParentIndex = Maxscript.QueryInteger("grnBoneParents[{0}]", i);

                    Maxscript.Command("boneTransMat = grnBones[{0}].transform", i);
                    if (bone.ParentIndex > 0)
                    {
                        Maxscript.Command("boneTransMat = boneTransMat * inverse(grnBones[{0}].parent.transform)", i);
                    }

                    Vector3 pos;
                    Quaternion rot;
                    Matrix4x4 scale;
                    this.GetTransformPRS("boneTransMat", out pos, out rot, out scale);
                    bone.Position = pos;
                    bone.Rotation = rot;
                    bone.Scale = scale;

                    this.File.Bones.Add(bone);
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
            string mainObject = "mainObject";
            Maxscript.Command("{0} = grnMeshes[{1}]", mainObject, meshIndex + 1);
            string mainMesh = Maxscript.SnapshotAsMesh("mainMesh", mainObject);
            mesh.DataExtensionIndex = this.File.AddDataExtension(Maxscript.QueryString("{0}.name", mainObject));

            Dictionary<int, int> matIdMapping = this.ExportMeshMaterial(mainObject);

            // Setup Normals
            Maxscript.Command("max modify mode");
            string tempObject = "tempObject";
            Maxscript.Command("{0} = Editable_Mesh()", tempObject);
            Maxscript.Command("{0}.mesh = {1}", tempObject, mainMesh);
            Maxscript.Command("addModifier {0} (Edit_Normals()) ui:off", tempObject);
            Maxscript.Command("modPanel.setCurrentObject {0}.modifiers[#edit_normals] ui:true", tempObject);

            int numVertices = Maxscript.QueryInteger("meshop.getnumverts {0}", mainMesh);
            int numFaces = Maxscript.QueryInteger("meshop.getnumfaces {0}", mainMesh);

            for (int i = 0; i < numVertices; i++)
            {
                try
                {
                    Maxscript.Command("vertex = meshGetVertFunc {0} {1}", mainMesh, i + 1);
                    mesh.Vertices.Add(new Vector3(
                        Maxscript.QueryFloat("vertex.x"),
                        Maxscript.QueryFloat("vertex.y"),
                        Maxscript.QueryFloat("vertex.z")));
                }
                catch (Exception ex)
                {
                    throw new Exception("Error importing vertex at index " + (i + 1) + ".", ex);
                }
            }

            int numNorms = Maxscript.QueryInteger("{0}.modifiers[#edit_normals].GetNumNormals()", tempObject);
            Maxscript.Command("getVertNormalFunc = {0}.modifiers[#edit_normals].GetNormal", tempObject);
            for (int i = 0; i < numNorms; ++i)
            {
                try
                {
                    Maxscript.Command("currentNormal = getVertNormalFunc {0}", i + 1);
                    mesh.Normals.Add(new Vector3(
                        Maxscript.QueryFloat("currentNormal.x"),
                        Maxscript.QueryFloat("currentNormal.y"),
                        Maxscript.QueryFloat("currentNormal.z")));
                }
                catch (Exception ex)
                {
                    throw new Exception("Error importing normal at index " + (i + 1) + ".", ex);
                }
            }

            int numTexVertices = Maxscript.QueryInteger("meshop.getnumtverts {0}", mainMesh);
            for (int i = 0; i < numTexVertices; i++)
            {
                try
                { 
                Maxscript.Command("tVert = meshGetMapVertFunc {0} 1 {1}", mainMesh, i + 1);
                mesh.TextureCoordinates.Add(new Vector3(
                    Maxscript.QueryFloat("tVert.x"),
                    1f-Maxscript.QueryFloat("tVert.y"),
                    Maxscript.QueryFloat("tVert.z")));
                }
                catch (Exception ex)
                {
                    throw new Exception("Error importing texture vertex at index " + (i + 1) + ".", ex);
                }
            }

            Maxscript.Command("meshGetNormalIdFunc = {0}.modifiers[#edit_normals].GetNormalID", tempObject);
            for (int i = 0; i < numFaces; ++i)
            {
                GrnFace f = new GrnFace();
                Int32 matIndex = Maxscript.QueryInteger("getFaceMatID {0} {1}", mainMesh, i + 1);
                if (matIdMapping.ContainsKey(matIndex))
                {
                    f.MaterialIndex = (Int16)(matIdMapping[matIndex]);
                }
                else
                {
                    throw new Exception("In mesh " + mesh.Name + " face index " + (i + 1) + " has an invalid material id " + matIndex + ".");
                }

                Maxscript.Command("face = getFace {0} {1}", mainMesh, i + 1);
                f.Indices.Add((Int16)(Maxscript.QueryInteger("face.x") - 1));
                f.Indices.Add((Int16)(Maxscript.QueryInteger("face.y") - 1));
                f.Indices.Add((Int16)(Maxscript.QueryInteger("face.z") - 1));

                f.NormalIndices.Add(Maxscript.QueryInteger("meshGetNormalIdFunc {0} {1}", i + 1, 1) - 1);
                f.NormalIndices.Add(Maxscript.QueryInteger("meshGetNormalIdFunc {0} {1}", i + 1, 2) - 1);
                f.NormalIndices.Add(Maxscript.QueryInteger("meshGetNormalIdFunc {0} {1}", i + 1, 3) - 1);

                Maxscript.Command("tFace = getTVFace {0} {1}", mainMesh, i + 1);
                f.TextureIndices.Add(Maxscript.QueryInteger("tFace.x") - 1);
                f.TextureIndices.Add(Maxscript.QueryInteger("tFace.y") - 1);
                f.TextureIndices.Add(Maxscript.QueryInteger("tFace.z") - 1);
                mesh.Faces.Add(f);
            }
            // Delete temporary object
            Maxscript.Command("delete {0}", tempObject);

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
                    mesh.BoneBindings[i].OBBMax = new Vector3(
                        Maxscript.QueryFloat("bbMax.x"),
                        Maxscript.QueryFloat("bbMax.y"),
                        Maxscript.QueryFloat("bbMax.z"));
                    mesh.BoneBindings[i].OBBMin = new Vector3(
                        Maxscript.QueryFloat("bbMin.x"),
                        Maxscript.QueryFloat("bbMin.y"),
                        Maxscript.QueryFloat("bbMin.z"));
                }

                if (numBVerts == 0 || numSkinBBs == 0)
                {
                    throw new Exception("Failed to export skin vertices in mesh " + mesh.Name + ".");
                }
            }
            else
            {
                throw new Exception("Mesh " + mesh.Name + " has no skin.");
            }
        }
        private Dictionary<int, int> ExportMeshMaterial(string mainObject)
        {
            Dictionary<int, int> matIdMapping = new Dictionary<int, int>();
            if (Maxscript.QueryBoolean("classof {0}.material == Multimaterial", mainObject))
            {
                int numMaterials = Maxscript.QueryInteger("{0}.material.materialList.count", "mainObject");
                for (int i = 0; i < numMaterials; ++i)
                {
                    GrnMaterial mat = new GrnMaterial(this.File);
                    int id = this.File.Materials.Count;
                    Maxscript.Command("mat = {0}.material.materialList[{1}]", mainObject, i + 1);
                    this.ExportMaterial(mat);

                    int matListIndex = this.File.Materials.IndexOf(mat);
                    int actualMatId = Maxscript.QueryInteger("{0}.material.materialIdList[{1}]", mainObject, i + 1);
                    if (matListIndex >= 0)
                    {
                        if (!matIdMapping.ContainsKey(actualMatId))
                        {
                            matIdMapping.Add(actualMatId, matListIndex);
                        }
                        this.File.DataExtensions.RemoveAt(mat.DataExtensionIndex);
                    }
                    else
                    {
                        this.File.Materials.Add(mat);
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
            else if (Maxscript.QueryBoolean("classof {0}.material == Standardmaterial", mainObject))
            {
                GrnMaterial mat = new GrnMaterial(this.File);
                int id = this.File.Materials.Count;
                Maxscript.Command("mat = {0}.material", mainObject);
                this.ExportMaterial(mat);

                int matListIndex = this.File.Materials.IndexOf(mat);
                if (matListIndex >= 0)
                {
                    matIdMapping.Add(1, matListIndex);
                    this.File.DataExtensions.RemoveAt(mat.DataExtensionIndex);
                }
                else
                {
                    this.File.Materials.Add(mat);
                    matIdMapping.Add(1, id);
                }
            }

            return matIdMapping;
        }
        private void ExportMaterial(GrnMaterial mat)
        {
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

            GrnTexture tex = null;
            if (Maxscript.QueryBoolean("(classof mat.diffusemap) == BitmapTexture"))
            {
                tex = new GrnTexture(this.File);
                tex.DataExtensionIndex = this.File.AddDataExtension(Maxscript.QueryString("mat.diffusemap.name"));
                this.File.SetDataExtensionFileName(tex.DataExtensionIndex, Path.GetFileNameWithoutExtension(Maxscript.QueryString("mat.diffusemap.filename")) + ".tga");
            }
            else if (Maxscript.QueryBoolean("(classof mat.diffusemap) == CompositeTextureMap") && Maxscript.QueryBoolean("(classof mat.diffusemap.mapList[1]) == BitmapTexture"))
            {
                tex = new GrnTexture(this.File);
                tex.DataExtensionIndex = this.File.AddDataExtension(Maxscript.QueryString("mat.diffusemap.mapList[1].name"));
                this.File.SetDataExtensionFileName(tex.DataExtensionIndex, Path.GetFileNameWithoutExtension(Maxscript.QueryString("mat.diffusemap.mapList[1].filename")) + ".tga");
            }

            if (tex != null)
            {
                int texIndex = this.File.Textures.IndexOf(tex);
                if (texIndex >= 0)
                {
                    mat.DiffuseTextureIndex = texIndex;
                    this.File.DataExtensions.RemoveAt(tex.DataExtensionIndex);
                }
                else
                {
                    mat.DiffuseTextureIndex = this.File.Textures.Count;
                    this.File.Textures.Add(tex);
                    string fileNameNoExt = Path.GetFileNameWithoutExtension(tex.FileName);
                    if (!fileNameNoExt.Equals(tex.Name))
                    {
                        throw new Exception("Texture name " + tex.Name + " must be set to " + fileNameNoExt + ".");
                    }
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
            Maxscript.Command("grnBoneAnimKeys = #()");

            for (int i = 1; i <= numBones; ++i)
            {
                GrnBoneTrack bone = new GrnBoneTrack();
                bone.DataExtensionIndex = this.File.Bones[i].DataExtensionIndex;

                Maxscript.Command("GetBoneAnimKeys grnBones[{0}]", i);
                Maxscript.Command("append grnBoneAnimKeys keys");
                if (this.File.Bones[i].ParentIndex > 0 &&
                    (Maxscript.QueryBoolean("classof grnBones[{0}] == Biped_Object", i) ||
                    Maxscript.QueryBoolean("classof grnBones[{0}][3].controller == BipSlave_Control", i) ||
                    Maxscript.QueryBoolean("classof grnBones[{0}][3].controller == Vertical_Horizontal_Turn", i)))
                {
                    Maxscript.Command("join keys grnBoneAnimKeys[grnBoneParents[{0}]]", i);
                    Maxscript.Command("keys = makeUniqueArray keys");
                    Maxscript.Command("sort keys");
                    Maxscript.Command("grnBoneAnimKeys[{0}] = keys", i);
                }

                int numKeys = Maxscript.QueryInteger("keys.count");
                float startTime = Maxscript.QueryFloat("animationRange.start.ticks / 4800.0");
                float time = Maxscript.QueryFloat("keys[1]");
                Vector3 pos = new Vector3();
                Quaternion rot = new Quaternion();
                Matrix4x4 scale = Matrix4x4.Identity;

                if (numKeys > 0)
                {
                    if (Maxscript.QueryBoolean("classof grnBones[{0}] == Dummy", i))
                    {
                        Maxscript.SetVarAtTime(time + startTime, "boneTransMat", "grnBones[{0}][3].controller.value", i);
                    }
                    else if (this.File.Bones[i].ParentIndex > 0)
                    {
                        Maxscript.SetVarAtTime(time + startTime, "boneTransMat", "grnBones[{0}].transform", i);
                        Maxscript.SetVarAtTime(time + startTime, "bonePTransMat", "grnBones[{0}].parent.transform", i);
                        Maxscript.Command("boneTransMat = boneTransMat * inverse(bonePTransMat)");
                    }
                    else
                    {
                        Maxscript.SetVarAtTime(time + startTime, "boneTransMat", "grnBones[{0}].transform", i);
                    }
                    this.GetTransformPRS("boneTransMat", out pos, out rot, out scale);

                    bone.PositionKeys.Add(time);
                    bone.Positions.Add(pos);
                    bone.RotationKeys.Add(time);
                    bone.Rotations.Add(rot);
                    bone.ScaleKeys.Add(time);
                    bone.Scales.Add(scale);
                }

                Vector3 posCurrent = new Vector3();
                Quaternion rotCurrent = new Quaternion();
                Matrix4x4 scaleCurrent = Matrix4x4.Identity;
                for (int j = 1; j < numKeys; ++j)
                {
                    time = Maxscript.QueryFloat("keys[{0}]", j + 1);
                    if (Maxscript.QueryBoolean("classof grnBones[{0}] == Dummy", i))
                    {
                        Maxscript.SetVarAtTime(time + startTime, "boneTransMat", "grnBones[{0}][3].controller.value", i);
                    }
                    else if (this.File.Bones[i].ParentIndex > 0)
                    {
                        Maxscript.SetVarAtTime(time + startTime, "boneTransMat", "grnBones[{0}].transform", i);
                        Maxscript.SetVarAtTime(time + startTime, "bonePTransMat", "grnBones[{0}].parent.transform", i);
                        Maxscript.Command("boneTransMat = boneTransMat * inverse(bonePTransMat)");
                    }
                    else
                    {
                        Maxscript.SetVarAtTime(time + startTime, "boneTransMat", "grnBones[{0}].transform", i);
                    }
                    this.GetTransformPRS("boneTransMat", out posCurrent, out rotCurrent, out scaleCurrent);

                    if (pos != posCurrent || j + 1 == numKeys)
                    {
                        bone.PositionKeys.Add(time);
                        bone.Positions.Add(posCurrent);
                        pos = posCurrent;
                    }

                    if (rot != rotCurrent || j + 1 == numKeys)
                    {
                        bone.RotationKeys.Add(time);
                        bone.Rotations.Add(rotCurrent);
                        rot = rotCurrent;
                    }

                    if (!this.AreEqual(scale, scaleCurrent) || j + 1 == numKeys) //if (scale != scaleCurrent)
                    {
                        bone.ScaleKeys.Add(time);
                        bone.Scales.Add(scaleCurrent);
                        scale = scaleCurrent;
                    }
                }

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
                rootBoneTrack.Positions.Add(new Vector3(0, 0, 0));
                rootBoneTrack.Rotations.Add(new Quaternion(0, 0, 0, 1));
                rootBoneTrack.Scales.Add(Matrix4x4.Identity);
            }
        }

        private string CreateBone(GrnBone bone)
        {
            string boneNode = "boneNode";

            Maxscript.Command("boneNode = dummy name:\"{0}\" boxsize:[0.25,0.25,0.25]", bone.Name);
            Maxscript.Command("boneNode.transform = {0}", this.GetBoneLocalTransform(bone, "boneTransMat"));

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

            return boneNode;
        }
        private string GetBoneLocalTransform(GrnBone bone, string nameM3)
        {
            Maxscript.Command("{0} = matrix3 1", nameM3);
            Maxscript.Command("{0} = transmatrix {1}", nameM3, Maxscript.Point3Literal(bone.Position));
            Maxscript.Command("{0} = (inverse(quat {1} {2} {3} {4}) as matrix3) * {0}", nameM3, bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z, bone.Rotation.W);
            Maxscript.Command("{0} = (matrix3 [{1}, {2}, {3}] [{4}, {5}, {6}] [{7}, {8}, {9}] [0,0,0]) * {0}", nameM3,
                bone.Scale.M11, bone.Scale.M12, bone.Scale.M13,
                bone.Scale.M21, bone.Scale.M22, bone.Scale.M23,
                bone.Scale.M31, bone.Scale.M32, bone.Scale.M33);

            return nameM3;
        }
        private string GetBoneLocalTransform(string nameM3, Vector3 pos, Quaternion rot, Matrix4x4 scale)
        {
            Maxscript.Command("{0} = matrix3 1", nameM3);
            Maxscript.Command("{0} = transmatrix {1}", nameM3, Maxscript.Point3Literal(pos));
            Maxscript.Command("{0} = (inverse(quat {1} {2} {3} {4}) as matrix3) * {0}", nameM3, rot.X, rot.Y, rot.Z, rot.W);
            Maxscript.Command("{0} = (matrix3 [{1}, {2}, {3}] [{4}, {5}, {6}] [{7}, {8}, {9}] [0,0,0]) * {0}", nameM3,
                scale.M11, scale.M12, scale.M13,
                scale.M21, scale.M22, scale.M23,
                scale.M31, scale.M32, scale.M33);

            return nameM3;
        }
        private void GetTransformPRS(string nameM3, out Vector3 pos, out Quaternion rot, out Matrix4x4 scale)
        {
            Maxscript.Command("bRot = inverse({0}.rotation)", nameM3);
            Maxscript.Command("boneScTransMat = {0} * inverse({0}.rotation as matrix3)", nameM3);
            Maxscript.Command("{0} = inverse({0}.rotation as matrix3) * {0}", nameM3);
            Maxscript.Command("bPos = {0}.position", nameM3);

            pos = new Vector3(
                    Maxscript.QueryFloat("bPos.x"),
                    Maxscript.QueryFloat("bPos.y"),
                    Maxscript.QueryFloat("bPos.z"));
            rot = new Quaternion(
                Maxscript.QueryFloat("bRot.x"),
                Maxscript.QueryFloat("bRot.y"),
                Maxscript.QueryFloat("bRot.z"),
                Maxscript.QueryFloat("bRot.w"));
            scale = new Matrix4x4();
            scale.M11 = Maxscript.QueryFloat("boneScTransMat.row1.x");
            scale.M12 = Maxscript.QueryFloat("boneScTransMat.row1.y");
            scale.M13 = Maxscript.QueryFloat("boneScTransMat.row1.z");
            scale.M21 = Maxscript.QueryFloat("boneScTransMat.row2.x");
            scale.M22 = Maxscript.QueryFloat("boneScTransMat.row2.y");
            scale.M23 = Maxscript.QueryFloat("boneScTransMat.row2.z");
            scale.M31 = Maxscript.QueryFloat("boneScTransMat.row3.x");
            scale.M32 = Maxscript.QueryFloat("boneScTransMat.row3.y");
            scale.M33 = Maxscript.QueryFloat("boneScTransMat.row3.z");
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
        private bool AreEqual(Matrix4x4 m1, Matrix4x4 m2)
        {
            float epsilon = 10e-6f;

            return (Math.Abs(m1.M11 - m2.M11) < epsilon) &&
                (Math.Abs(m1.M12 - m2.M12) < epsilon) &&
                (Math.Abs(m1.M13 - m2.M13) < epsilon) &&
                (Math.Abs(m1.M21 - m2.M21) < epsilon) &&
                (Math.Abs(m1.M22 - m2.M22) < epsilon) &&
                (Math.Abs(m1.M23 - m2.M23) < epsilon) &&
                (Math.Abs(m1.M31 - m2.M31) < epsilon) &&
                (Math.Abs(m1.M32 - m2.M32) < epsilon) &&
                (Math.Abs(m1.M33 - m2.M33) < epsilon);
        }
        #endregion

        #region UI
        public void LoadUI()
        {
            this.Plugin.Text = MaxPluginForm.PluginTitle + " - " + Path.GetFileName(this.FileName);

            this.Plugin.grnObjectsTreeListView.ClearObjects();
            if (this.File.Bones.Count > 0)
            {
                this.Plugin.grnObjectsTreeListView.AddObject(this.File.Bones[0]);
            }
            this.Plugin.grnObjectsTreeListView.AddObjects(this.File.Meshes);
            this.Plugin.grnObjectsTreeListView.AddObjects(this.File.Materials);

            int totalVerts = 0;
            int totalFaces = 0;
            for (int i = 0; i < this.File.Meshes.Count; ++i)
            {
                totalVerts += this.File.Meshes[i].Vertices.Count;
                totalFaces += this.File.Meshes[i].Faces.Count;
            }
            this.Plugin.vertsValueToolStripStatusLabel.Text = totalVerts.ToString();
            this.Plugin.facesValueToolStripStatusLabel.Text = totalFaces.ToString();
            this.Plugin.meshesValueToolStripStatusLabel.Text = this.File.Meshes.Count.ToString();
            this.Plugin.matsValueToolStripStatusLabel.Text = this.File.Materials.Count.ToString();
            this.Plugin.animLengthValueToolStripStatusLabel.Text = this.File.Animation.Duration.ToString();

            this.Plugin.grnExportModelCheckBox.Checked = this.ExportSetting.HasFlag(GrnExportSetting.Model);
            this.Plugin.grnExportAnimCheckBox.Checked = this.ExportSetting.HasFlag(GrnExportSetting.Animation);
        }

        public void SaveUI()
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
