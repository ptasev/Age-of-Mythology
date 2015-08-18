namespace AoMEngineLibrary.AMP
{
    using AoMEngineLibrary.Extensions;
    using AoMEngineLibrary.Graphics;
    using AoMEngineLibrary.Graphics.Brg;
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class BrgMax : IModelMaxUi
    {
        public BrgFile File { get; set; }
        public MaxPluginForm Plugin { get; set; }
        public string FileName { get; set; }
        public int FilterIndex { get { return 1; } }

        public BrgMeshInterpolationType InterpolationType { get; set; }
        public BrgMeshFlag Flags { get; set; }
        public BrgMeshFormat Format { get; set; }
        public BrgMeshAnimType AnimationType { get; set; }

        private BrgMaterial lastMatSelected;
        private bool uniformAttachpointScale;
        private bool modelAtCenter;

        public BrgMax(MaxPluginForm plugin)
        {
            this.Clear();
            this.File = new BrgFile();
            this.FileName = "Untitled";
            this.Plugin = plugin;
        }

        #region Setup
        public void Read(FileStream stream)
        {
            this.File = new BrgFile(stream);
            this.FileName = stream.Name;
        }
        public void Write(FileStream stream)
        {
            this.File.Write(stream);
            this.FileName = stream.Name;
        }
        public void Clear()
        {
            this.File = new BrgFile();
            this.FileName = Path.GetDirectoryName(this.FileName) + "\\Untitled";
            this.InterpolationType = 0;
            this.Flags = BrgMeshFlag.TEXCOORDSA | BrgMeshFlag.MATERIAL | BrgMeshFlag.ATTACHPOINTS;
            this.Format = BrgMeshFormat.HASFACENORMALS | BrgMeshFormat.ANIMATED;
            this.AnimationType = BrgMeshAnimType.KeyFrame;
        }
        #endregion

        #region Import/Export
        public void Import()
        {
            BrgFile brg = this.File as BrgFile;
            Maxscript.Command("frameRate = 30 --{0}", Math.Round(1 / this.File.Animation.TimeStep));
            Maxscript.Interval(0, this.File.Animation.Duration);

            if (this.File.Meshes.Count > 0)
            {
                string mainObject = "mainObj";
                //System.Windows.Forms.MessageBox.Show(file.Meshes[0].MeshAnimations.Count + " " + file.Animation.MeshChannel.MeshTimes.Count);
                for (int i = 0; i <= this.File.Meshes[0].MeshAnimations.Count; i++)
                {
                    Maxscript.CommentTitle("ANIMATE FRAME " + i);
                    if (i > 0)
                    {
                        ImportBrgMesh(mainObject, ((BrgMesh)brg.Meshes[0].MeshAnimations[i - 1]), brg.Animation.MeshKeys[i]);
                    }
                    else
                    {
                        ImportBrgMesh(mainObject, brg.Meshes[0], brg.Animation.MeshKeys[0]);
                    }
                }

                Maxscript.Command("update {0}", mainObject);
                if (this.modelAtCenter)
                {
                    Maxscript.Command("moveAmount = [0,0,0] - $Dummy_hotspot.position");
                    Maxscript.Command("max select all");
                    Maxscript.Command("move $ moveAmount");
                }
                Maxscript.Command("max select none");
                Maxscript.Command("max zoomext sel all");
                Maxscript.Command("max zoomext sel all");

                if (this.File.Materials.Count > 0)
                {
                    Maxscript.CommentTitle("LOAD MATERIALS");
                    Maxscript.Command("matGroup = multimaterial numsubs:{0}", this.File.Materials.Count);
                    for (int i = 0; i < this.File.Materials.Count; i++)
                    {
                        Maxscript.Command("matGroup[{0}] = {1}", i + 1, ImportBrgMaterial(brg.Materials[i]));
                        Maxscript.Command("matGroup.materialIDList[{0}] = {1}", i + 1, this.File.Materials[i].Id);
                    }
                    Maxscript.Command("{0}.material = matGroup", mainObject);
                }
            }
        }
        private void ImportBrgMesh(string mainObject, BrgMesh mesh, float time)
        {
            string vertArray = "";
            string normArray = "";
            string texVerts = "";
            string faceMats = "";
            string faceArray = "";
            if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
            {
                vertArray = Maxscript.NewArray("vertArray");
                normArray = Maxscript.NewArray("normArray");
                texVerts = Maxscript.NewArray("texVerts");

                faceMats = Maxscript.NewArray("faceMats");
                faceArray = Maxscript.NewArray("faceArray");
            }

            Maxscript.CommentTitle("Load Vertices/Normals/UVWs");
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                {
                    Maxscript.AnimateAtTime(time, "meshOp.setVert {0} {1} {2}", mainObject, i + 1,
                        Maxscript.Point3Literal<float>(-mesh.Vertices[i].X, -mesh.Vertices[i].Z, mesh.Vertices[i].Y));
                    //Maxscript.AnimateAtTime(time, "setNormal {0} {1} {2}", mainObject, i + 1,
                    //    Maxscript.NewPoint3Literal<float>(-this.Normals[i].X, -this.Normals[i].Z, this.Normals[i].Y));
                    //Maxscript.AnimateAtTime(time, "{0}.Edit_Normals.SetNormal {1} {2}", mainObject, i + 1,
                    //    Maxscript.NewPoint3Literal<float>(-this.Normals[i].X, -this.Normals[i].Z, this.Normals[i].Y));
                    //Maxscript.AnimateAtTime(time, "{0}.Edit_Normals.SetNormalExplicit {1}", mainObject, i + 1);

                    if (mesh.Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS) &&
                        mesh.Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                    {
                        Maxscript.Animate("{0}.Unwrap_UVW.SetVertexPosition {1}s {2} {3}", mainObject, time, i + 1,
                            Maxscript.Point3Literal<float>(mesh.TextureCoordinates[i].X, mesh.TextureCoordinates[i].Y, 0));
                    }

                    if (mesh.Header.Flags.HasFlag(BrgMeshFlag.ANIMVERTCOLORALPHA))
                    {
                        if (mesh.Header.Flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL))
                        {
                            Maxscript.AnimateAtTime(time, "meshop.setVertAlpha {0} -2 {1} {2}",
                                mainObject, i + 1, mesh.Colors[i].A);
                        }
                        else if (mesh.Header.Flags.HasFlag(BrgMeshFlag.COLORCHANNEL))
                        {
                            Maxscript.AnimateAtTime(time, "meshop.setVertColor {0} 0 {1} (color {2} {3} {4})", mainObject, i + 1,
                                mesh.Colors[i].R, mesh.Colors[i].G, mesh.Colors[i].B);
                        }
                    }
                }
                else
                {
                    Maxscript.Append(vertArray, Maxscript.NewPoint3<float>("v",
                        -mesh.Vertices[i].X, -mesh.Vertices[i].Z, mesh.Vertices[i].Y));

                    //Maxscript.Append(normArray, Maxscript.NewPoint3<float>("n",
                    //    -mesh.Normals[i].X, -mesh.Normals[i].Z, mesh.Normals[i].Y));

                    if (mesh.Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                    {
                        Maxscript.Append(texVerts, Maxscript.NewPoint3<float>("tV",
                            mesh.TextureCoordinates[i].X, mesh.TextureCoordinates[i].Y, 0));
                    }

                    if (mesh.Header.Flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL))
                    {
                        //Maxscript.Command("meshop.supportVAlphas {0}", mainObject);
                        Maxscript.Command("meshop.setVertAlpha {0} -2 {1} {2}",
                            mainObject, i + 1, mesh.Colors[i].A);
                    }
                    else if (mesh.Header.Flags.HasFlag(BrgMeshFlag.COLORCHANNEL))
                    {
                        Maxscript.Command("meshop.setVertColor {0} 0 {1} (color {2} {3} {4})", mainObject, i + 1,
                            mesh.Colors[i].R, mesh.Colors[i].G, mesh.Colors[i].B);
                    }
                }
            }

            if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
            {
                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                {
                    Maxscript.CommentTitle("Load Face Materials");
                    foreach (var fMat in mesh.Faces)
                    {
                        Maxscript.Append(faceMats, fMat.MaterialIndex.ToString());
                    }
                }

                Maxscript.CommentTitle("Load Faces");
                foreach (var face in mesh.Faces)
                {
                    Maxscript.Append(faceArray, Maxscript.NewPoint3<Int32>("fV", face.Indices[0] + 1, face.Indices[2] + 1, face.Indices[1] + 1));
                }

                Maxscript.AnimateAtTime(time, Maxscript.NewMeshLiteral(mainObject, vertArray, normArray, faceArray, faceMats, texVerts));
                Maxscript.Command("{0} = getNodeByName \"{0}\"", mainObject);

                Maxscript.Command("dummy name:\"Dummy_hotspot\" pos:{0} boxsize:[10,10,0]", Maxscript.Point3Literal(-mesh.Header.HotspotPosition.X, -mesh.Header.HotspotPosition.Z, mesh.Header.HotspotPosition.Y));

                Maxscript.CommentTitle("TVert Hack"); // Needed <= 3ds Max 2014; idk about 2015+
                Maxscript.Command("buildTVFaces {0}", mainObject);
                for (int i = 1; i <= mesh.Faces.Count; i++)
                {
                    Maxscript.Command("setTVFace {0} {1} (getFace {0} {1})", mainObject, i);
                }

                Maxscript.CommentTitle("Load Normals for first Frame");
                Maxscript.Command("max modify mode");
                Maxscript.Command("select {0}", mainObject);
                Maxscript.Command("addModifier {0} (Edit_Normals())", mainObject);
                Maxscript.Command("modPanel.setCurrentObject {0}.modifiers[#edit_normals]", mainObject);
                for (int i = 0; i < mesh.Faces.Count; ++i)
                {
                    Maxscript.Command("{0}.modifiers[#edit_normals].SetNormalID {1} {2} {3}",
                        mainObject, i + 1, 1, mesh.Faces[i].Indices[0] + 1);
                    Maxscript.Command("{0}.modifiers[#edit_normals].SetNormalID {1} {2} {3}",
                        mainObject, i + 1, 2, mesh.Faces[i].Indices[2] + 1);
                    Maxscript.Command("{0}.modifiers[#edit_normals].SetNormalID {1} {2} {3}",
                        mainObject, i + 1, 3, mesh.Faces[i].Indices[1] + 1);
                }
                for (int i = 0; i < mesh.Normals.Count; i++)
                {
                    Maxscript.Command("{0}.modifiers[#edit_normals].SetNormalExplicit {1}", mainObject, i + 1);
                    Maxscript.Command("{0}.modifiers[#edit_normals].SetNormal {1} {2}", mainObject, i + 1,
                        Maxscript.Point3Literal(-mesh.Normals[i].X, -mesh.Normals[i].Z, mesh.Normals[i].Y));
                }
                //Maxscript.Command("deleteModifier {0} {0}.modifiers[#edit_normals]", mainObject);
                Maxscript.Command("collapseStack {0}", mainObject);
                //Maxscript.Command("maxOps.CollapseNodeTo {0} 1 true", mainObject);

                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
                {
                    Maxscript.Command("select {0}", mainObject);
                    Maxscript.Command("addModifier {0} (Unwrap_UVW())", mainObject);

                    Maxscript.Command("select {0}.verts", mainObject);
                    Maxscript.Animate("{0}.Unwrap_UVW.moveSelected [0,0,0]", mainObject);
                }
            }

            Maxscript.CommentTitle("Load Attachpoints");
            string attachDummyArray = "attachDummyArray";
            if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
            {
                Maxscript.NewArray(attachDummyArray);
            }
            for (int i = 0; i < mesh.Attachpoints.Count; ++i)
            {
                BrgAttachpoint att = mesh.Attachpoints[i];
                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                {
                    //Maxscript.Command("attachpoint = getNodeByName \"{0}\"", att.GetMaxName());
                    Maxscript.Command("attachpoint = {0}[{1}]", attachDummyArray, i + 1);
                    Maxscript.AnimateAtTime(time, "attachpoint.rotation = {0}", att.GetMaxTransform());
                    Maxscript.AnimateAtTime(time, "attachpoint.position = {0}", att.GetMaxPosition());
                    if (this.uniformAttachpointScale)
                    {
                        Maxscript.AnimateAtTime(time, "attachpoint.scale = [1,1,1]");
                    }
                    else
                    {
                        Maxscript.AnimateAtTime(time, "attachpoint.scale = {0}", att.GetMaxScale());
                    }
                }
                else
                {
                    string attachDummy;
                    if (this.uniformAttachpointScale)
                    {
                        attachDummy = Maxscript.NewDummy("attachDummy", att.GetMaxName(), att.GetMaxTransform(), att.GetMaxPosition(), "[0.25,0.25,0.25]", "[1,1,1]");
                    }
                    else
                    {
                        attachDummy = Maxscript.NewDummy("attachDummy", att.GetMaxName(), att.GetMaxTransform(), att.GetMaxPosition(), att.GetMaxBoxSize(), att.GetMaxScale());
                    }
                    Maxscript.Command("append {0} {1}", attachDummyArray, attachDummy);
                }
            }
        }
        private string ImportBrgMaterial(BrgMaterial mat)
        {
            Maxscript.Command("mat = StandardMaterial()");
            this.ImportMaterialNameFromFlags(mat);
            Maxscript.Command("mat.adLock = false");
            Maxscript.Command("mat.useSelfIllumColor = true");
            Maxscript.Command("mat.diffuse = color {0} {1} {2}", mat.DiffuseColor.R * 255f, mat.DiffuseColor.G * 255f, mat.DiffuseColor.B * 255f);
            Maxscript.Command("mat.ambient = color {0} {1} {2}", mat.AmbientColor.R * 255f, mat.AmbientColor.G * 255f, mat.AmbientColor.B * 255f);
            Maxscript.Command("mat.specular = color {0} {1} {2}", mat.SpecularColor.R * 255f, mat.SpecularColor.G * 255f, mat.SpecularColor.B * 255f);
            Maxscript.Command("mat.selfIllumColor = color {0} {1} {2}", mat.EmissiveColor.R * 255f, mat.EmissiveColor.G * 255f, mat.EmissiveColor.B * 255f);
            Maxscript.Command("mat.opacity = {0}", mat.Opacity * 100f);
            Maxscript.Command("mat.specularLevel = {0}", mat.SpecularExponent);
            
            if (mat.Flags.HasFlag(BrgMatFlag.SubtractiveBlend))
            {
                Maxscript.Command("mat.opacityType = 1");
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.AdditiveBlend))
            {
                Maxscript.Command("mat.opacityType = 2");
            }

            if (mat.Flags.HasFlag(BrgMatFlag.TwoSided))
            {
                Maxscript.Command("mat.twoSided = true");
            }

            if (mat.Flags.HasFlag(BrgMatFlag.FaceMap))
            {
                Maxscript.Command("mat.faceMap = true");
            }

            if (mat.Flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
            {
                Maxscript.Command("rTex = BitmapTexture()");
                Maxscript.Command("rTex.name = \"{0}\"", Path.GetFileNameWithoutExtension(mat.sfx[0].Name));
                Maxscript.Command("rTex.filename = \"{0}\"", Path.GetFileNameWithoutExtension(mat.sfx[0].Name) + ".tga");
                Maxscript.Command("mat.reflectionMap = rTex");
            }
            if (mat.Flags.HasFlag(BrgMatFlag.BumpMap))
            {
                Maxscript.Command("aTex = BitmapTexture()");
                Maxscript.Command("aTex.name = \"{0}\"", mat.BumpMap);
                Maxscript.Command("aTex.filename = \"{0}\"", mat.BumpMap + ".tga");
                Maxscript.Command("mat.bumpMap = aTex");
            }
            if (mat.Flags.HasFlag(BrgMatFlag.WrapUTx1) && mat.Flags.HasFlag(BrgMatFlag.WrapVTx1))
            {
                Maxscript.Command("tex = BitmapTexture()");
                Maxscript.Command("tex.name = \"{0}\"", mat.DiffuseMap);
                Maxscript.Command("tex.filename = \"{0}\"", mat.DiffuseMap + ".tga");
                Maxscript.Command("mat.diffusemap = tex");
                if (mat.Flags.HasFlag(BrgMatFlag.PixelXForm1))
                {
                    //Maxscript.Command("pcTex = BitmapTexture()");
                    //Maxscript.Command("pcTex.name = \"{0}\"", mat.DiffuseMap);
                    //Maxscript.Command("pcTex.filename = \"{0}\"", mat.DiffuseMap + ".tga");
                    Maxscript.Command("mat.filtermap = tex");
                }
            }

            return "mat";
        }
        private void ImportMaterialNameFromFlags(BrgMaterial mat)
        {
            //Maxscript.Command("mat.name = \"{0}\"", mat.DiffuseMap);
            string name = Path.GetFileNameWithoutExtension(mat.DiffuseMap);

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

            Maxscript.Command("mat.name = \"{0}\"", name);
        }

        public void Export()
        {
            BrgFile brg = this.File;

            Maxscript.Command("ExportBrgData()");
            int meshCount = Maxscript.QueryInteger("brgMeshes.count");
            if (meshCount == 0)
            {
                throw new Exception("No Editable_Mesh objects detected!");
            }

            // Call GetKeys function from max
            Maxscript.Command("GetModelAnimKeys()");
            if (Maxscript.QueryBoolean("keys.Count == 0"))
            {
                throw new Exception("Could not acquire animation keys!");
            }

            brg.Header.NumMeshes = Maxscript.QueryInteger("keys.Count");
            brg.Animation = new Animation();
            for (int i = 1; i <= brg.Header.NumMeshes; ++i)
            {
                brg.Animation.MeshKeys.Add(Maxscript.QueryFloat("keys[{0}]", i));
            }
            if (brg.Header.NumMeshes == 1)
            {
                brg.Animation.Duration = 0;
            }
            else
            {
                brg.Animation.Duration = Maxscript.QueryFloat("(animationRange.end.ticks - animationRange.start.ticks) / 4800.0");
            }
            brg.Animation.TimeStep = brg.Animation.Duration / (float)brg.Header.NumMeshes;

            string mainObject = "mainObject";
            bool hadEditNormMod = false;
            brg.Materials = new List<BrgMaterial>();
            brg.Meshes = new List<BrgMesh>(brg.Header.NumMeshes);
            for (int m = 0; m < meshCount; ++m)
            {
                Maxscript.Command("{0} = brgMeshes[{1}]", mainObject, m + 1);

                // Materials
                Dictionary<int, int> matIdMapping = new Dictionary<int, int>();
                if (Maxscript.QueryBoolean("classof {0}.material == Multimaterial", mainObject))
                {
                    brg.Header.NumMaterials = Maxscript.QueryInteger("{0}.material.materialList.count", mainObject);
                    for (int i = 0; i < brg.Header.NumMaterials; i++)
                    {
                        BrgMaterial mat = new BrgMaterial(brg);
                        mat.Id = brg.Materials.Count + 1;
                        Maxscript.Command("mat = {0}.material.materialList[{1}]", mainObject, i + 1);
                        this.ExportBrgMaterial(mainObject, mat);

                        int matListIndex = brg.Materials.IndexOf(mat);
                        if (matListIndex >= 0)
                        {
                            matIdMapping.Add(Maxscript.QueryInteger("{0}.material.materialIdList[{1}]", mainObject, i + 1), brg.Materials[matListIndex].Id);
                        }
                        else
                        {
                            brg.Materials.Add(mat);
                            matIdMapping.Add(Maxscript.QueryInteger("{0}.material.materialIdList[{1}]", mainObject, i + 1), mat.Id);
                        }
                    }
                }
                else if (Maxscript.QueryBoolean("classof {0}.material == Standardmaterial", mainObject))
                {
                    BrgMaterial mat = new BrgMaterial(brg);
                    mat.Id = brg.Materials.Count + 1;
                    Maxscript.Command("mat = {0}.material", mainObject);
                    this.ExportBrgMaterial(mainObject, mat);

                    int matListIndex = brg.Materials.IndexOf(mat);
                    if (matListIndex >= 0)
                    {
                        matIdMapping.Add(1, brg.Materials[matListIndex].Id);
                    }
                    else
                    {
                        brg.Materials.Add(mat);
                        matIdMapping.Add(1, mat.Id);
                    }
                }
                else
                {
                    throw new Exception("Not all meshes have a material applied!");
                }

                // Add Edit_Normals Mod
                hadEditNormMod = false;
                if (Maxscript.QueryBoolean("{0}.modifiers[#edit_normals] == undefined", mainObject))
                {
                    Maxscript.Command("addModifier {0} (Edit_Normals())", mainObject);
                }
                else { hadEditNormMod = true; }
                Maxscript.Command("modPanel.setCurrentObject {0}.modifiers[#edit_normals] ui:true", mainObject);

                // Mesh Animations
                for (int i = 0; i < brg.Header.NumMeshes; i++)
                {
                    if (i > 0)
                    {
                        if (m == 0)
                        {
                            brg.Meshes[0].MeshAnimations.Add(new BrgMesh(brg));
                        }
                        brg.UpdateMeshSettings(i, this.Flags, this.Format, this.AnimationType, this.InterpolationType);
                        this.ExportBrgMesh(mainObject, (BrgMesh)brg.Meshes[0].MeshAnimations[i - 1], brg.Animation.MeshKeys[i], matIdMapping);
                    }
                    else
                    {
                        if (m == 0)
                        {
                            brg.Meshes.Add(new BrgMesh(brg));
                        }
                        brg.UpdateMeshSettings(i, this.Flags, this.Format, this.AnimationType, this.InterpolationType);
                        this.ExportBrgMesh(mainObject, brg.Meshes[i], brg.Animation.MeshKeys[i], matIdMapping);
                    }
                }

                // Delete normals mod if it wasn't there in the first place
                if (!hadEditNormMod)
                {
                    Maxscript.Command("deleteModifier {0} {0}.modifiers[#edit_normals]", mainObject);
                }
            }
            brg.Header.NumMaterials = brg.Materials.Count;

            // Export Attachpoints, and Update some Mesh data
            HashSet<int> usedFaceMaterials = new HashSet<int>();
            for (int i = 0; i < brg.Header.NumMeshes; i++)
            {
                BrgMesh mesh;
                if (i > 0)
                {
                    mesh = (BrgMesh)brg.Meshes[0].MeshAnimations[i - 1];
                }
                else
                {
                    mesh = brg.Meshes[i];
                }

                this.ExportAttachpoints(mesh, brg.Animation.MeshKeys[i]);
                HashSet<int> diffFaceMats = new HashSet<int>();
                if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                {
                    for (int j = 0; j < mesh.Faces.Count; ++j)
                    {
                        if (mesh.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                        {
                            diffFaceMats.Add(mesh.Faces[j].MaterialIndex);
                        }
                    }

                    if (diffFaceMats.Count > 0)
                    {
                        mesh.ExtendedHeader.NumMaterials = (byte)(diffFaceMats.Count - 1);
                        mesh.ExtendedHeader.NumUniqueMaterials = diffFaceMats.Count;
                    }
                }
                usedFaceMaterials.UnionWith(diffFaceMats);
                mesh.ExtendedHeader.AnimationLength = this.File.Animation.Duration;
            }
            List<BrgMaterial> usedMats = new List<BrgMaterial>(brg.Materials.Count);
            for (int i = 0; i < brg.Materials.Count; ++i)
            {
                if (usedFaceMaterials.Contains(brg.Materials[i].Id))
                {
                    usedMats.Add(brg.Materials[i]);
                }
            }
            brg.Materials = usedMats;
        }
        private void ExportBrgMesh(string mainObject, BrgMesh mesh, float time, Dictionary<int, int> matIdMapping)
        {
            time += Maxscript.QueryFloat("animationRange.start.ticks / 4800.0");

            Maxscript.SetVarAtTime(time, "meshCenter", "{0}.center", mainObject);
            mesh.Header.CenterPosition = new Vector3D
            (
                -Maxscript.QueryFloat("meshCenter.x"),
                Maxscript.QueryFloat("meshCenter.z"),
                -Maxscript.QueryFloat("meshCenter.y")
            );

            Maxscript.Command("grnd = getNodeByName \"Dummy_hotspot\"");
            if (!Maxscript.QueryBoolean("grnd == undefined"))
            {
                mesh.Header.HotspotPosition = new Vector3D
                (
                    -Maxscript.QueryFloat("grnd.position.x"),
                    Maxscript.QueryFloat("grnd.position.z"),
                    -Maxscript.QueryFloat("grnd.position.y")
                );
            }

            Maxscript.SetVarAtTime(time, "{0}BBMax", "{0}.max", mainObject);
            Maxscript.SetVarAtTime(time, "{0}BBMin", "{0}.min", mainObject);
            Vector3<float> bBoxMax = new Vector3<float>(Maxscript.QueryFloat("{0}BBMax.X", mainObject), Maxscript.QueryFloat("{0}BBMax.Y", mainObject), Maxscript.QueryFloat("{0}BBMax.Z", mainObject));
            Vector3<float> bBoxMin = new Vector3<float>(Maxscript.QueryFloat("{0}BBMin.X", mainObject), Maxscript.QueryFloat("{0}BBMin.Y", mainObject), Maxscript.QueryFloat("{0}BBMin.Z", mainObject));
            Vector3<float> bBox = (bBoxMax - bBoxMin) / 2;
            mesh.Header.MinimumExtent = new Vector3D(-bBox.X, -bBox.Z, -bBox.Y);
            mesh.Header.MaximumExtent = new Vector3D(bBox.X, bBox.Z, bBox.Y);

            string mainMesh = "mainMesh";

            // Figure out the proper data to import
            Maxscript.Command("GetExportData {0}", time);
            int numVertices = Maxscript.QueryInteger("{0}.numverts", mainMesh);
            int numFaces = Maxscript.QueryInteger("{0}.numfaces", mainMesh);
            int currNumVertices = mesh.Vertices.Count;

            //System.Windows.Forms.MessageBox.Show("1 " + numVertices);
            for (int i = 0; i < numVertices; i++)
            {
                //System.Windows.Forms.MessageBox.Show("1.1");
                try
                {
                    Maxscript.Command("vertex = getVert {0} {1}", mainMesh, i + 1);
                    //System.Windows.Forms.MessageBox.Show("1.4");
                    mesh.Vertices.Add(new Vector3D(-Maxscript.QueryFloat("vertex.x"), Maxscript.QueryFloat("vertex.z"), -Maxscript.QueryFloat("vertex.y")));

                    //System.Windows.Forms.MessageBox.Show("1.5");
                    mesh.Normals.Add(new Vector3D(
                        -Maxscript.QueryFloat("{0}[{1}].x", "averagedNormals", i + 1),
                        Maxscript.QueryFloat("{0}[{1}].z", "averagedNormals", i + 1),
                        -Maxscript.QueryFloat("{0}[{1}].y", "averagedNormals", i + 1)));
                    //System.Windows.Forms.MessageBox.Show("1.7");
                }
                catch (Exception ex)
                {
                    throw new Exception("Error at Export Verts/Normals " + i, ex);
                }
            }

            // Map Verts to TVerts
            int[] vertexMask = new int[numVertices];
            for (int i = 0; i < numFaces; i++)
            {
                Maxscript.Command("face = getFace {0} {1}", mainMesh, i + 1);
                Maxscript.Command("tFace = getTVFace {0} {1}", mainMesh, i + 1);
                int vert1 = Maxscript.QueryInteger("face[1]") - 1;
                int vert2 = Maxscript.QueryInteger("face[2]") - 1;
                int vert3 = Maxscript.QueryInteger("face[3]") - 1;
                int tVert1 = Maxscript.QueryInteger("tFace[1]");
                int tVert2 = Maxscript.QueryInteger("tFace[2]");
                int tVert3 = Maxscript.QueryInteger("tFace[3]");

                vertexMask[vert1] = tVert1;
                vertexMask[vert2] = tVert2;
                vertexMask[vert3] = tVert3;
            }
            //System.Windows.Forms.MessageBox.Show("2");
            if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || mesh.Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
            {
                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                {
                    for (int i = 0; i < numVertices; i++)
                    {
                        if (vertexMask[i] > 0)
                        {
                            Maxscript.Command("tVert = getTVert {0} {1}", mainMesh, vertexMask[i]);// i + 1);
                            mesh.TextureCoordinates.Add(new Vector3D(Maxscript.QueryFloat("tVert.x"), Maxscript.QueryFloat("tVert.y"), 0f));
                        }
                    }
                }
            }

            //System.Windows.Forms.MessageBox.Show("3");
            if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
            {
                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                {
                    mesh.VertexMaterials.AddRange(new Int16[numVertices]);
                }
                for (int i = 0; i < numFaces; ++i)
                {
                    Face f = new Face();
                    mesh.Faces.Add(f);

                    if (mesh.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        f.MaterialIndex = (Int16)matIdMapping[Maxscript.QueryInteger("getFaceMatID {0} {1}", mainMesh, i + 1)];
                        if (f.MaterialIndex == 2)
                        {
                            //MaxPluginForm.DebugBox("yo " + i);
                        }
                    }

                    //System.Windows.Forms.MessageBox.Show("3.1");
                    Maxscript.Command("face = getFace {0} {1}", mainMesh, i + 1);
                    f.Indices.Add((Int16)(Maxscript.QueryInteger("face.x") - 1 + currNumVertices));
                    f.Indices.Add((Int16)(Maxscript.QueryInteger("face.z") - 1 + currNumVertices));
                    f.Indices.Add((Int16)(Maxscript.QueryInteger("face.y") - 1 + currNumVertices));

                    //System.Windows.Forms.MessageBox.Show("3.2");
                    if (mesh.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                    {
                        mesh.VertexMaterials[f.Indices[0]] = f.MaterialIndex;
                        mesh.VertexMaterials[f.Indices[1]] = f.MaterialIndex;
                        mesh.VertexMaterials[f.Indices[2]] = f.MaterialIndex;
                    }
                }
            }
        }
        private void ExportAttachpoints(BrgMesh mesh, float time)
        {
            time += Maxscript.QueryFloat("animationRange.start.ticks / 4800.0");
            //System.Windows.Forms.MessageBox.Show("4");
            string attachDummy = Maxscript.NewArray("attachDummy");
            Maxscript.SetVarAtTime(time, attachDummy, "for helpObj in ($helpers/Dummy_*) where classof helpObj == Dummy collect helpObj");//"$helpers/Dummy_* as array");
            //Maxscript.SetVarAtTime(time, attachDummy, "$helpers/atpt??* as array");
            int numAttachpoints = Maxscript.QueryInteger("{0}.count", attachDummy);

            //System.Windows.Forms.MessageBox.Show("5 " + numAttachpoints);
            if (mesh.Header.Flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                mesh.Attachpoints = new List<BrgAttachpoint>();
                for (int i = 0; i < numAttachpoints; i++)
                {
                    string aName = Maxscript.QueryString("{0}[{1}].name", attachDummy, i + 1);
                    int nameId;
                    if (!BrgAttachpoint.TryGetIdByName(aName.Substring(6), out nameId)) continue;
                    BrgAttachpoint att = new BrgAttachpoint();
                    //System.Windows.Forms.MessageBox.Show(aName);
                    //int index = Convert.ToInt32((Maxscript.QueryString("{0}[{1}].name", attachDummy, i + 1)).Substring(4, 2));
                    //System.Windows.Forms.MessageBox.Show("5.1");
                    //System.Windows.Forms.MessageBox.Show(mesh.Attachpoints.Count + " " + i);
                    att.NameId = nameId;
                    Maxscript.Command("{0}[{1}].name = \"{2}\"", attachDummy, i + 1, att.GetMaxName());
                    //System.Windows.Forms.MessageBox.Show("5.2");
                    Maxscript.SetVarAtTime(time, "{0}Transform", "{0}[{1}].rotation as matrix3", attachDummy, i + 1);
                    Maxscript.SetVarAtTime(time, "{0}Position", "{0}[{1}].position", attachDummy, i + 1);
                    Maxscript.SetVarAtTime(time, "{0}Scale", "{0}[{1}].scale * {0}[{1}].boxsize", attachDummy, i + 1);
                    //System.Windows.Forms.MessageBox.Show("5.3");
                    Vector3<float> scale = new Vector3<float>(Maxscript.QueryFloat("{0}Scale.X", attachDummy), Maxscript.QueryFloat("{0}Scale.Y", attachDummy), Maxscript.QueryFloat("{0}Scale.Z", attachDummy));
                    Vector3<float> bBox = scale / 2;
                    //System.Windows.Forms.MessageBox.Show("5.4");

                    att.XVector.X = -Maxscript.QueryFloat("{0}Transform[1].z", attachDummy);
                    att.XVector.Y = Maxscript.QueryFloat("{0}Transform[3].z", attachDummy);
                    att.XVector.Z = -Maxscript.QueryFloat("{0}Transform[2].z", attachDummy);

                    att.YVector.X = -Maxscript.QueryFloat("{0}Transform[1].y", attachDummy);
                    att.YVector.Y = Maxscript.QueryFloat("{0}Transform[3].y", attachDummy);
                    att.YVector.Z = -Maxscript.QueryFloat("{0}Transform[2].y", attachDummy);

                    att.ZVector.X = -Maxscript.QueryFloat("{0}Transform[1].x", attachDummy);
                    att.ZVector.Y = Maxscript.QueryFloat("{0}Transform[3].x", attachDummy);
                    att.ZVector.Z = -Maxscript.QueryFloat("{0}Transform[2].x", attachDummy);

                    att.Position.X = -Maxscript.QueryFloat("{0}Position.x", attachDummy);
                    att.Position.Z = -Maxscript.QueryFloat("{0}Position.y", attachDummy);
                    att.Position.Y = Maxscript.QueryFloat("{0}Position.z", attachDummy);
                    //System.Windows.Forms.MessageBox.Show("5.5");

                    att.BoundingBoxMin.X = -bBox.X;
                    att.BoundingBoxMin.Z = -bBox.Y;
                    att.BoundingBoxMin.Y = -bBox.Z;
                    att.BoundingBoxMax.X = bBox.X;
                    att.BoundingBoxMax.Z = bBox.Y;
                    att.BoundingBoxMax.Y = bBox.Z;

                    mesh.Attachpoints.Add(att);
                }
                //System.Windows.Forms.MessageBox.Show("# Atpts: " + Attachpoint.Count);
            }
        }
        private void ExportBrgMaterial(string mainObject, BrgMaterial mat)
        {
            //mat.id = Maxscript.QueryInteger("{0}.material.materialIDList[{1}]", mainObject, materialIndex + 1);
            //Maxscript.Command("mat = {0}.material[{1}]", mainObject, mat.id);

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

            mat.SpecularExponent = Maxscript.QueryFloat("mat.specularLevel");
            if (mat.SpecularExponent > 0)
            {
                mat.Flags |= BrgMatFlag.SpecularExponent;
            }

            mat.Opacity = Maxscript.QueryFloat("mat.opacity") / 100f;
            if (mat.Opacity < 1f)
            {
                mat.Flags |= BrgMatFlag.Alpha;
            }

            int opacityType = Maxscript.QueryInteger("mat.opacityType");
            if (opacityType == 1)
            {
                mat.Flags |= BrgMatFlag.SubtractiveBlend;
            }
            else if (opacityType == 2)
            {
                mat.Flags |= BrgMatFlag.AdditiveBlend;
            }

            if (Maxscript.QueryBoolean("mat.twoSided"))
            {
                mat.Flags |= BrgMatFlag.TwoSided;
            }

            if (Maxscript.QueryBoolean("mat.faceMap"))
            {
                mat.Flags |= BrgMatFlag.FaceMap;
            }

            if (Maxscript.QueryBoolean("(classof mat.reflectionMap) == BitmapTexture"))
            {
                mat.Flags |= BrgMatFlag.WrapUTx1 | BrgMatFlag.WrapVTx1 | BrgMatFlag.REFLECTIONTEXTURE;
                BrgMatSFX sfxMap = new BrgMatSFX();
                sfxMap.Id = 30;
                sfxMap.Name = Maxscript.QueryString("getFilenameFile(mat.reflectionMap.filename)") + ".cub";
                mat.sfx.Add(sfxMap);
            }
            if (Maxscript.QueryBoolean("(classof mat.bumpMap) == BitmapTexture"))
            {
                mat.BumpMap = Maxscript.QueryString("getFilenameFile(mat.bumpMap.filename)");
                if (mat.BumpMap.Length > 0)
                {
                    mat.Flags |= BrgMatFlag.WrapUTx3 | BrgMatFlag.WrapVTx3 | BrgMatFlag.BumpMap;
                }
            }
            if (Maxscript.QueryBoolean("(classof mat.diffusemap) == BitmapTexture"))
            {
                mat.DiffuseMap = Maxscript.QueryString("getFilenameFile(mat.diffusemap.filename)");
                if (mat.DiffuseMap.Length > 0)
                {
                    mat.Flags |= BrgMatFlag.WrapUTx1 | BrgMatFlag.WrapVTx1;
                    if (Maxscript.QueryBoolean("mat.diffusemap.filename == mat.filtermap.filename"))
                    {
                        mat.Flags |= BrgMatFlag.PixelXForm1;
                    }
                }
            }
            else if (Maxscript.QueryBoolean("(classof mat.diffusemap) == CompositeTextureMap") && Maxscript.QueryBoolean("(classof mat.diffusemap.mapList[1]) == BitmapTexture"))
            {
                mat.Flags |= BrgMatFlag.WrapUTx1 | BrgMatFlag.WrapVTx1 | BrgMatFlag.PixelXForm1;
                mat.DiffuseMap = Maxscript.QueryString("getFilenameFile(mat.diffusemap.mapList[1].filename)");
            }

            this.ExportMaterialFlagsFromName(mat);
        }
        private void ExportMaterialFlagsFromName(BrgMaterial mat)
        {
            string flags = Maxscript.QueryString("mat.name").ToLower();
            StringComparison cmp = StringComparison.InvariantCultureIgnoreCase;

            if (flags.Contains("colorxform1", cmp))
            {
                mat.Flags |= BrgMatFlag.PlayerXFormColor1;
            }

            if (flags.Contains("2-sided", cmp) ||
                flags.Contains("2 sided", cmp) ||
                flags.Contains("2sided", cmp))
            {
                mat.Flags |= BrgMatFlag.TwoSided;
            }

            if (flags.Contains("pixelxform1", cmp))
            {
                mat.Flags |= BrgMatFlag.PixelXForm1;
            }
        }
        #endregion

        #region UI
        public void LoadUi()
        {
            this.Plugin.Text = MaxPluginForm.PluginTitle + " - " + Path.GetFileName(this.FileName);
            // Materials
            this.LoadUiMaterial();

            // General Info
            this.Plugin.matsValueToolStripStatusLabel.Text = this.File.Materials.Count.ToString();
            this.Plugin.brgImportAttachScaleCheckBox.Checked = this.uniformAttachpointScale;
            this.Plugin.brgImportCenterModelCheckBox.Checked = this.modelAtCenter;
            //MessageBox.Show("1");

            if (this.File.Meshes.Count > 0)
            {
                this.Plugin.vertsValueToolStripStatusLabel.Text = this.File.Meshes[0].Vertices.Count.ToString();
                this.Plugin.facesValueToolStripStatusLabel.Text = this.File.Meshes[0].Faces.Count.ToString();
                this.Plugin.meshesValueToolStripStatusLabel.Text = (this.File.Meshes[0].MeshAnimations.Count + 1).ToString();
                //MessageBox.Show("2.1");
                this.Plugin.animLengthValueToolStripStatusLabel.Text = this.File.Meshes[0].ExtendedHeader.AnimationLength.ToString();
                //MessageBox.Show("2.2");
                this.Plugin.interpolationTypeCheckBox.Checked = Convert.ToBoolean(this.File.Meshes[0].Header.InterpolationType);
                //MessageBox.Show("2.3");

                // Attachpoints
                LoadUiAttachpoint();

                for (int i = 0; i < this.Plugin.genMeshFlagsCheckedListBox.Items.Count; i++)
                {
                    if (this.File.Meshes[0].Header.Flags.HasFlag((BrgMeshFlag)this.Plugin.genMeshFlagsCheckedListBox.Items[i]))
                    {
                        this.Plugin.genMeshFlagsCheckedListBox.SetItemChecked(i, true);
                    }
                    else
                    {
                        this.Plugin.genMeshFlagsCheckedListBox.SetItemChecked(i, false);
                    }
                }
                for (int i = 0; i < this.Plugin.genMeshFormatCheckedListBox.Items.Count; i++)
                {
                    if (this.File.Meshes[0].Header.Format.HasFlag((BrgMeshFormat)this.Plugin.genMeshFormatCheckedListBox.Items[i]))
                    {
                        this.Plugin.genMeshFormatCheckedListBox.SetItemChecked(i, true);
                    }
                    else
                    {
                        this.Plugin.genMeshFormatCheckedListBox.SetItemChecked(i, false);
                    }
                }
                if (this.File.Meshes[0].Header.AnimationType == BrgMeshAnimType.KeyFrame)
                {
                    this.Plugin.keyframeRadioButton.Checked = true;
                }
                else if (this.File.Meshes[0].Header.AnimationType == BrgMeshAnimType.NonUniform)
                {
                    this.Plugin.nonuniRadioButton.Checked = true;
                }
                else if (this.File.Meshes[0].Header.AnimationType == BrgMeshAnimType.SkinBone)
                {
                    this.Plugin.skinBoneRadioButton.Checked = true;
                }
            }
            else
            {
                this.Plugin.vertsValueToolStripStatusLabel.Text = "0";
                this.Plugin.facesValueToolStripStatusLabel.Text = "0";
                this.Plugin.meshesValueToolStripStatusLabel.Text = "0";
                this.Plugin.animLengthValueToolStripStatusLabel.Text = "0.0";
                this.Plugin.interpolationTypeCheckBox.Checked = Convert.ToBoolean(this.InterpolationType);
                this.Plugin.SetCheckedListBoxSelectedEnums<BrgMeshFlag>(this.Plugin.genMeshFlagsCheckedListBox, (uint)this.Flags);
                this.Plugin.SetCheckedListBoxSelectedEnums<BrgMeshFormat>(this.Plugin.genMeshFormatCheckedListBox, (uint)this.Format);

                if (this.AnimationType == BrgMeshAnimType.KeyFrame)
                {
                    this.Plugin.keyframeRadioButton.Checked = true;
                }
                else if (this.AnimationType == BrgMeshAnimType.NonUniform)
                {
                    this.Plugin.nonuniRadioButton.Checked = true;
                }
                else if (this.AnimationType == BrgMeshAnimType.SkinBone)
                {
                    this.Plugin.skinBoneRadioButton.Checked = true;
                }
            }
        }
        public void LoadUiMaterial()
        {
            this.Plugin.materialListBox.DataSource = null;
            this.Plugin.materialListBox.DataSource = this.File.Materials;
            this.Plugin.materialListBox.DisplayMember = "EditorName";

        }
        public void LoadUiMaterialData()
        {
            if (this.lastMatSelected != null)
            {
                this.lastMatSelected.Flags = this.Plugin.
                    GetCheckedListBoxSelectedEnums<BrgMatFlag>(this.Plugin.materialFlagsCheckedListBox);
            }
            BrgMaterial mat = this.File.Materials[this.Plugin.materialListBox.SelectedIndex];
            this.lastMatSelected = mat;
            // Update Info
            this.Plugin.diffuseMaxTextBox.BackColor = System.Drawing.Color.FromArgb(Convert.ToByte(mat.DiffuseColor.R * Byte.MaxValue),
                Convert.ToByte(mat.DiffuseColor.G * Byte.MaxValue),
                Convert.ToByte(mat.DiffuseColor.B * Byte.MaxValue));
            this.Plugin.diffuseMaxTextBox.ForeColor = this.Plugin.ContrastColor(this.Plugin.diffuseMaxTextBox.BackColor);
            this.Plugin.ambientMaxTextBox.BackColor = System.Drawing.Color.FromArgb(Convert.ToByte(mat.AmbientColor.R * Byte.MaxValue),
                Convert.ToByte(mat.AmbientColor.G * Byte.MaxValue),
                Convert.ToByte(mat.AmbientColor.B * Byte.MaxValue));
            this.Plugin.ambientMaxTextBox.ForeColor = this.Plugin.ContrastColor(this.Plugin.ambientMaxTextBox.BackColor);
            this.Plugin.specularMaxTextBox.BackColor = System.Drawing.Color.FromArgb(Convert.ToByte(mat.SpecularColor.R * Byte.MaxValue),
                Convert.ToByte(mat.SpecularColor.G * Byte.MaxValue),
                Convert.ToByte(mat.SpecularColor.B * Byte.MaxValue));
            this.Plugin.specularMaxTextBox.ForeColor = this.Plugin.ContrastColor(this.Plugin.specularMaxTextBox.BackColor);
            this.Plugin.selfIllumMaxTextBox.BackColor = System.Drawing.Color.FromArgb(Convert.ToByte(mat.EmissiveColor.R * Byte.MaxValue),
                Convert.ToByte(mat.EmissiveColor.G * Byte.MaxValue),
                Convert.ToByte(mat.EmissiveColor.B * Byte.MaxValue));
            this.Plugin.selfIllumMaxTextBox.ForeColor = this.Plugin.ContrastColor(this.Plugin.selfIllumMaxTextBox.BackColor);
            this.Plugin.specularLevelMaxTextBox.Text = mat.SpecularExponent.ToString();
            this.Plugin.opacityMaxTextBox.Text = mat.Opacity.ToString();
            this.Plugin.textureMaxTextBox.Text = mat.DiffuseMap;
            if (mat.sfx.Count > 0)
            {
                this.Plugin.reflectionMaxTextBox.Text = mat.sfx[0].Name;
            }
            else { this.Plugin.reflectionMaxTextBox.Text = string.Empty; }
            this.Plugin.bumpMapMaxTextBox.Text = mat.BumpMap;

            // Update Flags box
            for (int i = 0; i < this.Plugin.materialFlagsCheckedListBox.Items.Count; i++)
            {
                if (mat.Flags.HasFlag((BrgMatFlag)this.Plugin.materialFlagsCheckedListBox.Items[i]))
                {
                    this.Plugin.materialFlagsCheckedListBox.SetItemChecked(i, true);
                }
                else
                {
                    this.Plugin.materialFlagsCheckedListBox.SetItemChecked(i, false);
                }
            }
        }
        public void LoadUiAttachpoint()
        {
            //attachpointComboBox.DataSource = null;
            //attachpointComboBox.DataSource = file.Mesh[0].attachpoints.Values;
            this.Plugin.attachpointComboBox.Items.Clear();
            foreach (BrgAttachpoint att in this.File.Meshes[0].Attachpoints)
            {
                this.Plugin.attachpointComboBox.Items.Add(att);
            }
            this.Plugin.attachpointComboBox.ValueMember = "Index";
            this.Plugin.attachpointComboBox.DisplayMember = "MaxName";
        }

        public void SaveUi()
        {
            this.uniformAttachpointScale = this.Plugin.brgImportAttachScaleCheckBox.Checked;
            this.modelAtCenter = this.Plugin.brgImportCenterModelCheckBox.Checked;
            this.InterpolationType = (BrgMeshInterpolationType)Convert.ToByte(this.Plugin.interpolationTypeCheckBox.Checked);
            this.Flags = this.Plugin.
                GetCheckedListBoxSelectedEnums<BrgMeshFlag>(this.Plugin.genMeshFlagsCheckedListBox);
            this.Format = this.Plugin.
                GetCheckedListBoxSelectedEnums<BrgMeshFormat>(this.Plugin.genMeshFormatCheckedListBox);
            if (this.Plugin.keyframeRadioButton.Checked)
            {
                this.AnimationType = BrgMeshAnimType.KeyFrame;
            }
            else if (this.Plugin.nonuniRadioButton.Checked)
            {
                this.AnimationType = BrgMeshAnimType.NonUniform;
            }
            else if (this.Plugin.skinBoneRadioButton.Checked)
            {
                this.AnimationType = BrgMeshAnimType.SkinBone;
            }
            this.File.UpdateMeshSettings(this.Flags, this.Format, this.AnimationType, this.InterpolationType);
            if (this.lastMatSelected != null)
            {
                this.lastMatSelected.Flags = this.Plugin.
                    GetCheckedListBoxSelectedEnums<BrgMatFlag>(this.Plugin.materialFlagsCheckedListBox);
            }
        }
        #endregion
    }
}
