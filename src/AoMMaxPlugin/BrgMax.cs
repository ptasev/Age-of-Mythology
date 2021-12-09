namespace AoMMaxPlugin
{
    using AoMEngineLibrary.Extensions;
    using AoMEngineLibrary.Graphics;
    using AoMEngineLibrary.Graphics.Brg;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class BrgMax : IModelMaxUI
    {
        public BrgFile File { get; set; }
        public MaxPluginForm Plugin { get; set; }
        public string FileName { get; set; }
        public int FilterIndex { get { return 1; } }

        private bool uniformAttachpointScale;
        private bool modelAtCenter;

        public BrgMax(MaxPluginForm plugin)
        {
            this.Clear();
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
            this.File.Meshes.Add(new BrgMesh());
            this.FileName = Path.GetDirectoryName(this.FileName) + "\\Untitled";
            this.File.Meshes[0].Header.InterpolationType = BrgMeshInterpolationType.Default;
            this.File.Meshes[0].Header.Flags = BrgMeshFlag.Texture1 | BrgMeshFlag.Material | BrgMeshFlag.DummyObjects;
            this.File.Meshes[0].Header.Format = BrgMeshFormat.HasFaceNormals | BrgMeshFormat.AnimationLength;
            this.File.Meshes[0].Header.AnimationType = BrgMeshAnimType.KeyFrame;
        }
        #endregion

        #region Import/Export
        public void Import()
        {
            BrgFile brg = this.File as BrgFile;
            Maxscript.Output.Clear();
            Maxscript.Command("importStartTime = timeStamp()");
            Maxscript.Command("frameRate = 30");
            Maxscript.Interval(0, this.File.Animation.Duration);

            if (this.File.Meshes.Count > 0)
            {
                string mainObject = "mainObj";
                for (int i = 0; i < this.File.Meshes.Count; i++)
                {
                    Maxscript.CommentTitle("ANIMATE FRAME " + i);
                    ImportBrgMesh(mainObject, brg.Meshes[i], brg.Animation.MeshKeys[0]);
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

            Maxscript.Command("importEndTime = timeStamp()");
            Maxscript.Format("Import took % seconds\n", "((importEndTime - importStartTime) / 1000.0)");
        }
        private void ImportBrgMesh(string mainObject, BrgMesh mesh, float time)
        {
            string vertArray = "";
            string normArray = "";
            string texVerts = "";
            string faceMats = "";
            string faceArray = "";
            if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.Secondary))
            {
                vertArray = Maxscript.NewArray("vertArray");
                normArray = Maxscript.NewArray("normArray");
                texVerts = Maxscript.NewArray("texVerts");

                faceMats = Maxscript.NewArray("faceMats");
                faceArray = Maxscript.NewArray("faceArray");
            }

            Maxscript.CommentTitle("Load Vertices/Normals/UVWs");
            Maxscript.Command("uvwSetVertPosFunc = {0}.Unwrap_UVW.SetVertexPosition", mainObject);
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.Secondary))
                {
                    Maxscript.AnimateAtTime(time, "meshSetVertFunc {0} {1} {2}", mainObject, i + 1,
                        Maxscript.Point3Literal(-mesh.Vertices[i].X, -mesh.Vertices[i].Z, mesh.Vertices[i].Y));

                    if (mesh.Header.Flags.HasFlag(BrgMeshFlag.AnimTxCoords) &&
                        mesh.Header.Flags.HasFlag(BrgMeshFlag.Texture1))
                    {
                        Maxscript.Animate("uvwSetVertPosFunc {0}s {1} {2}", time, i + 1,
                            Maxscript.Point3Literal(mesh.TextureCoordinates[i].X, mesh.TextureCoordinates[i].Y, 0));
                    }

                    if (mesh.Header.Flags.HasFlag(BrgMeshFlag.AnimVertexColor))
                    {
                        if (mesh.Header.Flags.HasFlag(BrgMeshFlag.AlphaChannel))
                        {
                            Maxscript.AnimateAtTime(time, "meshop.setVertAlpha {0} -2 {1} {2}",
                                mainObject, i + 1, mesh.Colors[i].W);
                        }
                        else if (mesh.Header.Flags.HasFlag(BrgMeshFlag.ColorChannel))
                        {
                            Maxscript.AnimateAtTime(time, "meshop.setVertColor {0} 0 {1} (color {2} {3} {4})", mainObject, i + 1,
                                mesh.Colors[i].X, mesh.Colors[i].Y, mesh.Colors[i].Z);
                        }
                    }
                }
                else
                {
                    Maxscript.Append(vertArray, Maxscript.Point3Literal(-mesh.Vertices[i].X, -mesh.Vertices[i].Z, mesh.Vertices[i].Y));

                    if (mesh.Header.Flags.HasFlag(BrgMeshFlag.Texture1))
                    {
                        Maxscript.Append(texVerts, Maxscript.Point3Literal(mesh.TextureCoordinates[i].X, mesh.TextureCoordinates[i].Y, 0));
                    }

                    if (mesh.Header.Flags.HasFlag(BrgMeshFlag.AlphaChannel))
                    {
                        //Maxscript.Command("meshop.supportVAlphas {0}", mainObject);
                        Maxscript.Command("meshop.setVertAlpha {0} -2 {1} {2}",
                            mainObject, i + 1, mesh.Colors[i].W);
                    }
                    else if (mesh.Header.Flags.HasFlag(BrgMeshFlag.ColorChannel))
                    {
                        Maxscript.Command("meshop.setVertColor {0} 0 {1} (color {2} {3} {4})", mainObject, i + 1,
                            mesh.Colors[i].X, mesh.Colors[i].Y, mesh.Colors[i].Z);
                    }
                }
            }

            if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.Secondary))
            {
                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.Material))
                {
                    Maxscript.CommentTitle("Load Face Materials");
                    foreach (var fMat in mesh.Faces)
                    {
                        Maxscript.Append(faceMats, fMat.MaterialIndex);
                    }
                }

                Maxscript.CommentTitle("Load Faces");
                foreach (var face in mesh.Faces)
                {
                    Maxscript.Append(faceArray, Maxscript.Point3Literal(face.A + 1, face.C + 1, face.B + 1));
                }

                int mObjNum = 0;
                while (Maxscript.QueryInteger("($objects/{0}* as array).count", mainObject + mObjNum) > 0) { ++mObjNum; }
                Maxscript.Command("{0} = {1}", mainObject, Maxscript.NewMeshLiteral(mainObject + mObjNum, vertArray, faceArray, faceMats, texVerts));
                //Maxscript.Command("{0} = getNodeByName \"{0}\"", mainObject);

                Maxscript.Command("dummy name:\"Dummy_hotspot\" pos:{0} boxsize:[10,10,0]", Maxscript.Point3Literal(-mesh.Header.HotspotPosition.X, -mesh.Header.HotspotPosition.Z, mesh.Header.HotspotPosition.Y));

                Maxscript.CommentTitle("TVert Hack"); // Needed <= 3ds Max 2014; idk about 2015+
                Maxscript.Command("buildTVFaces {0}", mainObject);
                for (int i = 1; i <= mesh.Faces.Count; i++)
                {
                    Maxscript.Command("setTVFace {0} {1} {2}[{1}]", mainObject, i, faceArray);
                }

                Maxscript.CommentTitle("Load Normals for first Frame");
                Maxscript.Command("max modify mode");
                Maxscript.Command("select {0}", mainObject);
                Maxscript.Command("addModifier {0} (Edit_Normals()) ui:off", mainObject);
                Maxscript.Command("modPanel.setCurrentObject {0}.modifiers[#edit_normals]", mainObject);

                Maxscript.Command("{0}.modifiers[#edit_normals].Break selection:#{{1..{1}}}", mainObject, mesh.Normals.Count);
                Maxscript.Command("meshSetNormalIdFunc = {0}.modifiers[#edit_normals].SetNormalID", mainObject);
                for (int i = 0; i < mesh.Faces.Count; ++i)
                {
                    Maxscript.Command("meshSetNormalIdFunc {0} {1} {2}",
                        i + 1, 1, mesh.Faces[i].A + 1);
                    Maxscript.Command("meshSetNormalIdFunc {0} {1} {2}",
                        i + 1, 2, mesh.Faces[i].C + 1);
                    Maxscript.Command("meshSetNormalIdFunc {0} {1} {2}",
                        i + 1, 3, mesh.Faces[i].B + 1);
                }
                Maxscript.Command("{0}.modifiers[#edit_normals].MakeExplicit selection:#{{1..{1}}}", mainObject, mesh.Normals.Count);
                Maxscript.Command("meshSetNormalFunc = {0}.modifiers[#edit_normals].SetNormal", mainObject);
                for (int i = 0; i < mesh.Normals.Count; i++)
                {
                    Maxscript.Command("meshSetNormalFunc {0} {1}", i + 1,
                        Maxscript.Point3Literal(-mesh.Normals[i].X, -mesh.Normals[i].Z, mesh.Normals[i].Y));
                }
                Maxscript.Command("collapseStack {0}", mainObject);

                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.AnimTxCoords))
                {
                    Maxscript.Command("select {0}", mainObject);
                    Maxscript.Command("addModifier {0} (Unwrap_UVW()) ui:off", mainObject);

                    Maxscript.Command("select {0}.verts", mainObject);
                    Maxscript.Animate("{0}.Unwrap_UVW.moveSelected [0,0,0]", mainObject);
                }
            }

            Maxscript.CommentTitle("Load Attachpoints");
            string attachDummyArray = "attachDummyArray";
            if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.Secondary))
            {
                Maxscript.NewArray(attachDummyArray);
            }
            for (int i = 0; i < mesh.Dummies.Count; ++i)
            {
                BrgDummy att = mesh.Dummies[i];
                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.Secondary))
                {
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
            Maxscript.Command("mat.diffuse = color {0} {1} {2}", mat.DiffuseColor.X * 255f, mat.DiffuseColor.Y * 255f, mat.DiffuseColor.Z * 255f);
            Maxscript.Command("mat.ambient = color {0} {1} {2}", mat.AmbientColor.X * 255f, mat.AmbientColor.Y * 255f, mat.AmbientColor.Z * 255f);
            Maxscript.Command("mat.specular = color {0} {1} {2}", mat.SpecularColor.X * 255f, mat.SpecularColor.Y * 255f, mat.SpecularColor.Z * 255f);
            Maxscript.Command("mat.selfIllumColor = color {0} {1} {2}", mat.EmissiveColor.X * 255f, mat.EmissiveColor.Y * 255f, mat.EmissiveColor.Z * 255f);
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

            if (mat.Flags.HasFlag(BrgMatFlag.CubeMapInfo))
            {
                Maxscript.Command("rTex = BitmapTexture()");
                Maxscript.Command("rTex.name = \"{0}\"", Path.GetFileNameWithoutExtension(mat.CubeMapInfo.CubeMapName));
                Maxscript.Command("rTex.filename = \"{0}\"", Path.GetFileNameWithoutExtension(mat.CubeMapInfo.CubeMapName) + ".tga");
                Maxscript.Command("mat.reflectionMap = rTex");
            }
            if (mat.Flags.HasFlag(BrgMatFlag.BumpMap))
            {
                Maxscript.Command("aTex = BitmapTexture()");
                Maxscript.Command("aTex.name = \"{0}\"", mat.BumpMapName);
                Maxscript.Command("aTex.filename = \"{0}\"", mat.BumpMapName + ".tga");
                Maxscript.Command("mat.bumpMap = aTex");
            }
            if (mat.Flags.HasFlag(BrgMatFlag.WrapUTx1) && mat.Flags.HasFlag(BrgMatFlag.WrapVTx1))
            {
                Maxscript.Command("tex = BitmapTexture()");
                Maxscript.Command("tex.name = \"{0}\"", mat.DiffuseMapName);
                Maxscript.Command("tex.filename = \"{0}\"", mat.DiffuseMapName + ".tga");
                Maxscript.Command("mat.diffusemap = tex");
                if (mat.Flags.HasFlag(BrgMatFlag.PixelXForm1))
                {
                    Maxscript.Command("mat.filtermap = tex");
                }
            }

            return "mat";
        }
        private void ImportMaterialNameFromFlags(BrgMaterial mat)
        {
            //Maxscript.Command("mat.name = \"{0}\"", mat.DiffuseMap);
            string name = Path.GetFileNameWithoutExtension(mat.DiffuseMapName);

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
            Maxscript.Command("exportStartTime = timeStamp()");
            //Maxscript.Command("print heapSize");
            BrgMeshFlag flags = brg.Meshes[0].Header.Flags;
            BrgMeshFormat format = brg.Meshes[0].Header.Format;
            BrgMeshAnimType animationType = brg.Meshes[0].Header.AnimationType;
            BrgMeshInterpolationType interpolationType = brg.Meshes[0].Header.InterpolationType;

            Maxscript.Command("ExportBrgData()");
            int totalNumVerts = Maxscript.QueryInteger("brgtotalNumVerts");
            int totalNumFaces = Maxscript.QueryInteger("brgtotalNumFaces");
            int meshCount = Maxscript.QueryInteger("brgMeshes.count");
            if (meshCount == 0)
            {
                throw new Exception("No Editable_Mesh objects detected!");
            }
            if (Maxscript.QueryBoolean("keys.Count == 0"))
            {
                throw new Exception("Could not acquire animation keys!");
            }

            brg.Header.NumMeshes = Maxscript.QueryInteger("keys.Count");
            brg.Animation = new BrgAnimation();
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

            string mainObject = "mainObject";
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
                        BrgMaterial mat = new BrgMaterial();
                        mat.Id = brg.Materials.Count + 1;
                        Maxscript.Command("mat = {0}.material.materialList[{1}]", mainObject, i + 1);
                        this.ExportBrgMaterial(mainObject, mat);

                        int matListIndex = brg.Materials.IndexOf(mat);
                        int actualMatId = Maxscript.QueryInteger("{0}.material.materialIdList[{1}]", mainObject, i + 1);
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
                else if (Maxscript.QueryBoolean("classof {0}.material == Standardmaterial", mainObject))
                {
                    BrgMaterial mat = new BrgMaterial();
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
                    if (flags.HasFlag(BrgMeshFlag.Material))
                    {
                        throw new Exception("Not all meshes have a material applied! " + Maxscript.QueryString("{0}.name", mainObject));
                    }
                }

                // Mesh Animations
                for (int i = 0; i < brg.Header.NumMeshes; i++)
                {
                    if (m == 0)
                    {
                        BrgMesh mesh = new BrgMesh();
                        mesh.Vertices = new List<Vector3>(totalNumVerts);
                        mesh.Normals = new List<Vector3>(totalNumVerts);
                        mesh.TextureCoordinates = new List<Vector2>(totalNumVerts);
                        mesh.Faces = new List<BrgFace>(totalNumFaces);
                        brg.Meshes.Add(mesh);
                    }
                    brg.UpdateMeshSettings(i, flags, format, animationType, interpolationType);
                    this.ExportBrgMesh(mainObject, brg.Meshes[i], brg.Animation.MeshKeys[i], matIdMapping);
                }
            }

            // Export Attachpoints, and Update some Mesh data
            HashSet<int> usedFaceMaterials = new HashSet<int>();
            string attachDummy = Maxscript.NewArray("attachDummy");
            Maxscript.Command("{0} = for helpObj in ($helpers/Dummy_*) where classof helpObj == Dummy collect helpObj", attachDummy);//"$helpers/Dummy_* as array");
            for (int i = 0; i < brg.Header.NumMeshes; i++)
            {
                BrgMesh mesh = brg.Meshes[i];

                this.ExportAttachpoints(attachDummy, mesh, brg.Animation.MeshKeys[i]);
                HashSet<int> diffFaceMats = new HashSet<int>();
                if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.Secondary) &&
                    mesh.Header.Flags.HasFlag(BrgMeshFlag.Material))
                {
                    for (int j = 0; j < mesh.Faces.Count; ++j)
                    {
                        diffFaceMats.Add(mesh.Faces[j].MaterialIndex);
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
            brg.Header.NumMaterials = brg.Materials.Count;

            //Maxscript.Command("print heapSize");
            Maxscript.Command("exportEndTime = timeStamp()");
            Maxscript.Format("Export took % seconds\n", "((exportEndTime - exportStartTime) / 1000.0)");
        }
        private void ExportBrgMesh(string mainObject, BrgMesh mesh, float time, Dictionary<int, int> matIdMapping)
        {
            time += Maxscript.QueryFloat("animationRange.start.ticks / 4800.0");

            string mainMesh = "mainMesh";
            string mainObjectName = Maxscript.QueryString("{0}.name", mainObject);
            // Figure out the proper data to import
            if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.Secondary))
            {
                Maxscript.Command("{0} = ExportPreservedTexCoordData (GetMeshSnapshotAtTime {1} {2})", mainMesh, mainObject, time);
            }
            else
            {
                Maxscript.Command("{0} = GetMeshSnapshotAtTime {1} {2}", mainMesh, mainObject, time);
            }
            Maxscript.Command("ExportBrgVertNormals {0}", time);

            Maxscript.SetVarAtTime(time, "meshCenter", "{0}.center", mainObject);
            mesh.Header.CenterPosition = new Vector3
            (
                -Maxscript.QueryFloat("meshCenter.x"),
                Maxscript.QueryFloat("meshCenter.z"),
                -Maxscript.QueryFloat("meshCenter.y")
            );

            Maxscript.Command("grnd = getNodeByName \"Dummy_hotspot\"");
            if (!Maxscript.QueryBoolean("grnd == undefined"))
            {
                mesh.Header.HotspotPosition = new Vector3
                (
                    -Maxscript.QueryFloat("grnd.position.x"),
                    Maxscript.QueryFloat("grnd.position.z"),
                    -Maxscript.QueryFloat("grnd.position.y")
                );
            }

            Maxscript.SetVarAtTime(time, "{0}BBMax", "{0}.max", mainObject);
            Maxscript.SetVarAtTime(time, "{0}BBMin", "{0}.min", mainObject);
            Vector3 bBoxMax = new Vector3(Maxscript.QueryFloat("{0}BBMax.X", mainObject), Maxscript.QueryFloat("{0}BBMax.Y", mainObject), Maxscript.QueryFloat("{0}BBMax.Z", mainObject));
            Vector3 bBoxMin = new Vector3(Maxscript.QueryFloat("{0}BBMin.X", mainObject), Maxscript.QueryFloat("{0}BBMin.Y", mainObject), Maxscript.QueryFloat("{0}BBMin.Z", mainObject));
            Vector3 bBox = (bBoxMax - bBoxMin) / 2;
            mesh.Header.MinimumExtent = new Vector3(-bBox.X, -bBox.Z, -bBox.Y);
            mesh.Header.MaximumExtent = new Vector3(bBox.X, bBox.Z, bBox.Y);

            int numVertices = Maxscript.QueryInteger("brgVertIndices.count");
            int numFaces = Maxscript.QueryInteger("brgFaceArray.count");
            int currNumVertices = mesh.Vertices.Count;

            //System.Windows.Forms.MessageBox.Show("1 " + numVertices);
            for (int i = 0; i < numVertices; i++)
            {
                //System.Windows.Forms.MessageBox.Show("1.1");
                try
                {
                    Maxscript.Command("vertex = getVert {0} brgVertIndices[{1}]", mainMesh, i + 1);
                    //System.Windows.Forms.MessageBox.Show("1.4");
                    mesh.Vertices.Add(new Vector3(-Maxscript.QueryFloat("vertex.x"), Maxscript.QueryFloat("vertex.z"), -Maxscript.QueryFloat("vertex.y")));

                    //System.Windows.Forms.MessageBox.Show("1.5");
                    mesh.Normals.Add(new Vector3(
                        -Maxscript.QueryFloat("{0}[{1}].x", "averagedNormals", i + 1),
                        Maxscript.QueryFloat("{0}[{1}].z", "averagedNormals", i + 1),
                        -Maxscript.QueryFloat("{0}[{1}].y", "averagedNormals", i + 1)));
                    //System.Windows.Forms.MessageBox.Show("1.7");
                }
                catch (Exception ex)
                {
                    throw new Exception("In mesh " + mainObjectName + " error exporting verts/normals index " + (i + 1) + ".", ex);
                }
            }

            //System.Windows.Forms.MessageBox.Show("2");
            if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.Secondary) || mesh.Header.Flags.HasFlag(BrgMeshFlag.AnimTxCoords))
            {
                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.Texture1))
                {
                    for (int i = 0; i < numVertices; i++)
                    {
                        Maxscript.Command("tVert = getTVert {0} brgVertTVIndices[{1}]", mainMesh, i + 1);
                        mesh.TextureCoordinates.Add(new Vector2(Maxscript.QueryFloat("tVert.x"), Maxscript.QueryFloat("tVert.y")));
                    }
                }
            }

            //System.Windows.Forms.MessageBox.Show("3");
            if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.Secondary) &&
                mesh.Header.Flags.HasFlag(BrgMeshFlag.Material))
            {
                mesh.VertexMaterials.AddRange(new Int16[numVertices]);
                for (int i = 0; i < numFaces; ++i)
                {
                    BrgFace f = new BrgFace();
                    mesh.Faces.Add(f);

                    int faceMatId = Maxscript.QueryInteger("getFaceMatId {0} {1}", mainMesh, i + 1);
                    if (matIdMapping.ContainsKey(faceMatId))
                    {
                        f.MaterialIndex = (Int16)matIdMapping[faceMatId];
                    }
                    else
                    {
                        throw new Exception("In mesh " + mainObjectName + " face index " + (i + 1) + " has an invalid material id " + faceMatId + ".");
                    }

                    //System.Windows.Forms.MessageBox.Show("3.1");
                    Maxscript.Command("face = brgFaceArray[{0}]", i + 1);
                    f.A = (ushort)(Maxscript.QueryInteger("face.x") - 1 + currNumVertices);
                    f.B = (ushort)(Maxscript.QueryInteger("face.z") - 1 + currNumVertices);
                    f.C = (ushort)(Maxscript.QueryInteger("face.y") - 1 + currNumVertices);

                    //System.Windows.Forms.MessageBox.Show("3.2");
                    mesh.VertexMaterials[f.A] = f.MaterialIndex;
                    mesh.VertexMaterials[f.B] = f.MaterialIndex;
                    mesh.VertexMaterials[f.C] = f.MaterialIndex;
                }
            }

            Maxscript.Command("delete {0}", mainMesh);
        }
        private void ExportAttachpoints(string attachDummy, BrgMesh mesh, float time)
        {
            time += Maxscript.QueryFloat("animationRange.start.ticks / 4800.0");
            //System.Windows.Forms.MessageBox.Show("4");
            int numAttachpoints = Maxscript.QueryInteger("{0}.count", attachDummy);

            //System.Windows.Forms.MessageBox.Show("5 " + numAttachpoints);
            if (mesh.Header.Flags.HasFlag(BrgMeshFlag.DummyObjects))
            {
                mesh.Dummies = new BrgDummyCollection();
                for (int i = 0; i < numAttachpoints; i++)
                {
                    string aName = Maxscript.QueryString("{0}[{1}].name", attachDummy, i + 1);
                    if (!BrgDummyTypeInfo.TryGetByName(aName.Substring(6), out var typeInfo)) continue;
                    var att = new BrgDummy()
                    {
                        Type = typeInfo.Type
                    };
                    //System.Windows.Forms.MessageBox.Show(aName);
                    //System.Windows.Forms.MessageBox.Show("5.1");
                    //System.Windows.Forms.MessageBox.Show(mesh.Attachpoints.Count + " " + i);
                    Maxscript.Command("{0}[{1}].name = \"{2}\"", attachDummy, i + 1, att.GetMaxName());
                    //System.Windows.Forms.MessageBox.Show("5.2");
                    Maxscript.SetVarAtTime(time, "{0}Transform", "{0}[{1}].rotation as matrix3", attachDummy, i + 1);
                    Maxscript.SetVarAtTime(time, "{0}Position", "{0}[{1}].position", attachDummy, i + 1);
                    Maxscript.SetVarAtTime(time, "{0}Scale", "{0}[{1}].scale * {0}[{1}].boxsize", attachDummy, i + 1);
                    //System.Windows.Forms.MessageBox.Show("5.3");
                    Vector3 scale = new Vector3(Maxscript.QueryFloat("{0}Scale.X", attachDummy), Maxscript.QueryFloat("{0}Scale.Y", attachDummy), Maxscript.QueryFloat("{0}Scale.Z", attachDummy));
                    Vector3 bBox = scale / 2;
                    //System.Windows.Forms.MessageBox.Show("5.4");

                    att.Up = new Vector3(
                        -Maxscript.QueryFloat("{0}Transform[1].z", attachDummy),
                        Maxscript.QueryFloat("{0}Transform[3].z", attachDummy),
                        -Maxscript.QueryFloat("{0}Transform[2].z", attachDummy));

                    att.Forward = new Vector3(
                        -Maxscript.QueryFloat("{0}Transform[1].y", attachDummy),
                        Maxscript.QueryFloat("{0}Transform[3].y", attachDummy),
                        -Maxscript.QueryFloat("{0}Transform[2].y", attachDummy));

                    att.Right = new Vector3(
                        -Maxscript.QueryFloat("{0}Transform[1].x", attachDummy),
                        Maxscript.QueryFloat("{0}Transform[3].x", attachDummy),
                        -Maxscript.QueryFloat("{0}Transform[2].x", attachDummy));

                    att.Position = new Vector3(
                        -Maxscript.QueryFloat("{0}Position.x", attachDummy),
                        Maxscript.QueryFloat("{0}Position.z", attachDummy),
                        -Maxscript.QueryFloat("{0}Position.y", attachDummy));
                    //System.Windows.Forms.MessageBox.Show("5.5");

                    att.BoundingBoxMin = new Vector3(-bBox.X, -bBox.Z, -bBox.Y);
                    att.BoundingBoxMax = new Vector3(bBox.X, bBox.Z, bBox.Y);

                    mesh.Dummies.Add(att);
                }
                //System.Windows.Forms.MessageBox.Show("# Atpts: " + Attachpoint.Count);
            }
        }
        private void ExportBrgMaterial(string mainObject, BrgMaterial mat)
        {
            //mat.id = Maxscript.QueryInteger("{0}.material.materialIDList[{1}]", mainObject, materialIndex + 1);
            //Maxscript.Command("mat = {0}.material[{1}]", mainObject, mat.id);

            mat.DiffuseColor = new Vector3(Maxscript.QueryFloat("mat.diffuse.r") / 255f,
                Maxscript.QueryFloat("mat.diffuse.g") / 255f,
                Maxscript.QueryFloat("mat.diffuse.b") / 255f);
            mat.AmbientColor = new Vector3(Maxscript.QueryFloat("mat.ambient.r") / 255f,
                Maxscript.QueryFloat("mat.ambient.g") / 255f,
                Maxscript.QueryFloat("mat.ambient.b") / 255f);
            mat.SpecularColor = new Vector3(Maxscript.QueryFloat("mat.specular.r") / 255f,
                Maxscript.QueryFloat("mat.specular.g") / 255f,
                Maxscript.QueryFloat("mat.specular.b") / 255f);
            mat.EmissiveColor = new Vector3(Maxscript.QueryFloat("mat.selfIllumColor.r") / 255f,
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
                mat.Flags |= BrgMatFlag.WrapUTx1 | BrgMatFlag.WrapVTx1 | BrgMatFlag.CubeMapInfo;
                BrgCubeMapInfo sfxMap = new BrgCubeMapInfo();
                sfxMap.TextureFactor = 30;
                sfxMap.CubeMapName = Maxscript.QueryString("getFilenameFile(mat.reflectionMap.filename)") + ".cub";
            }
            if (Maxscript.QueryBoolean("(classof mat.bumpMap) == BitmapTexture"))
            {
                mat.BumpMapName = Maxscript.QueryString("getFilenameFile(mat.bumpMap.filename)");
                if (mat.BumpMapName.Length > 0)
                {
                    mat.Flags |= BrgMatFlag.WrapUTx3 | BrgMatFlag.WrapVTx3 | BrgMatFlag.BumpMap;
                }
            }
            if (Maxscript.QueryBoolean("(classof mat.diffusemap) == BitmapTexture"))
            {
                mat.DiffuseMapName = Maxscript.QueryString("getFilenameFile(mat.diffusemap.filename)");
                int parenthIndex = mat.DiffuseMapName.IndexOf('(');
                if (parenthIndex > 0)
                {
                    mat.DiffuseMapName = mat.DiffuseMapName.Remove(parenthIndex);
                }

                if (mat.DiffuseMapName.Length > 0)
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
                mat.DiffuseMapName = Maxscript.QueryString("getFilenameFile(mat.diffusemap.mapList[1].filename)");
                int parenthIndex = mat.DiffuseMapName.IndexOf('(');
                if (parenthIndex > 0)
                {
                    mat.DiffuseMapName = mat.DiffuseMapName.Remove(parenthIndex);
                }
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
        public void LoadUI()
        {
            this.Plugin.Text = MaxPluginForm.PluginTitle + " - " + Path.GetFileName(this.FileName);

            this.Plugin.brgObjectsTreeListView.ClearObjects();
            this.Plugin.brgObjectsTreeListView.AddObject(this.File.Meshes[0]);
            this.Plugin.brgObjectsTreeListView.AddObjects(this.File.Materials);
            this.Plugin.brgObjectsTreeListView.AddObjects(this.File.Meshes[0].Dummies);
            this.Plugin.brgObjectsTreeListView.SelectObject(this.File.Meshes[0], true);

            // General Info
            this.Plugin.brgImportAttachScaleCheckBox.Checked = this.uniformAttachpointScale;
            this.Plugin.brgImportCenterModelCheckBox.Checked = this.modelAtCenter;

            this.Plugin.vertsValueToolStripStatusLabel.Text = this.File.Meshes[0].Vertices.Count.ToString();
            this.Plugin.facesValueToolStripStatusLabel.Text = this.File.Meshes[0].Faces.Count.ToString();
            this.Plugin.meshesValueToolStripStatusLabel.Text = (this.File.Meshes.Count).ToString();
            this.Plugin.animLengthValueToolStripStatusLabel.Text = this.File.Meshes[0].ExtendedHeader.AnimationLength.ToString();
            this.Plugin.matsValueToolStripStatusLabel.Text = this.File.Materials.Count.ToString();
        }
        public void LoadMeshUI()
        {
            BrgMesh mesh = (BrgMesh)this.Plugin.brgObjectsTreeListView.SelectedObject;

            this.Plugin.interpolationTypeCheckBox.Checked = Convert.ToBoolean(mesh.Header.InterpolationType);
            this.Plugin.brgMeshFlagsCheckedListBox.SetEnum<BrgMeshFlag>(mesh.Header.Flags);
            this.Plugin.brgMeshFormatCheckedListBox.SetEnum<BrgMeshFormat>(mesh.Header.Format);
            
            if (mesh.Header.AnimationType == BrgMeshAnimType.KeyFrame)
            {
                this.Plugin.keyframeRadioButton.Checked = true;
            }
            else if (mesh.Header.AnimationType == BrgMeshAnimType.NonUniform)
            {
                this.Plugin.nonuniRadioButton.Checked = true;
            }
            else if (mesh.Header.AnimationType == BrgMeshAnimType.SkinBone)
            {
                this.Plugin.skinBoneRadioButton.Checked = true;
            }
        }
        public void LoadMaterialUI()
        {
            BrgMaterial mat = (BrgMaterial)this.Plugin.brgObjectsTreeListView.SelectedObject;

            // Update Info
            this.Plugin.diffuseMaxTextBox.BackColor = System.Drawing.Color.FromArgb(Convert.ToByte(mat.DiffuseColor.X * Byte.MaxValue),
                Convert.ToByte(mat.DiffuseColor.Y * Byte.MaxValue),
                Convert.ToByte(mat.DiffuseColor.Z * Byte.MaxValue));
            this.Plugin.diffuseMaxTextBox.ForeColor = this.Plugin.ContrastColor(this.Plugin.diffuseMaxTextBox.BackColor);
            this.Plugin.ambientMaxTextBox.BackColor = System.Drawing.Color.FromArgb(Convert.ToByte(mat.AmbientColor.X * Byte.MaxValue),
                Convert.ToByte(mat.AmbientColor.Y * Byte.MaxValue),
                Convert.ToByte(mat.AmbientColor.Z * Byte.MaxValue));
            this.Plugin.ambientMaxTextBox.ForeColor = this.Plugin.ContrastColor(this.Plugin.ambientMaxTextBox.BackColor);
            this.Plugin.specularMaxTextBox.BackColor = System.Drawing.Color.FromArgb(Convert.ToByte(mat.SpecularColor.X * Byte.MaxValue),
                Convert.ToByte(mat.SpecularColor.Y * Byte.MaxValue),
                Convert.ToByte(mat.SpecularColor.Z * Byte.MaxValue));
            this.Plugin.specularMaxTextBox.ForeColor = this.Plugin.ContrastColor(this.Plugin.specularMaxTextBox.BackColor);
            this.Plugin.selfIllumMaxTextBox.BackColor = System.Drawing.Color.FromArgb(Convert.ToByte(mat.EmissiveColor.X * Byte.MaxValue),
                Convert.ToByte(mat.EmissiveColor.Y * Byte.MaxValue),
                Convert.ToByte(mat.EmissiveColor.Z * Byte.MaxValue));
            this.Plugin.selfIllumMaxTextBox.ForeColor = this.Plugin.ContrastColor(this.Plugin.selfIllumMaxTextBox.BackColor);
            this.Plugin.specularLevelMaxTextBox.Text = mat.SpecularExponent.ToString();
            this.Plugin.opacityMaxTextBox.Text = mat.Opacity.ToString();
            this.Plugin.textureMaxTextBox.Text = mat.DiffuseMapName;
            this.Plugin.reflectionMaxTextBox.Text = mat.CubeMapInfo.CubeMapName;
            this.Plugin.bumpMapMaxTextBox.Text = mat.BumpMapName;

            // Update Flags box
            this.Plugin.materialFlagsCheckedListBox.SetEnum<BrgMatFlag>(mat.Flags);
        }

        public void SaveUI()
        {
            this.uniformAttachpointScale = this.Plugin.brgImportAttachScaleCheckBox.Checked;
            this.modelAtCenter = this.Plugin.brgImportCenterModelCheckBox.Checked;
        }
        #endregion
    }
}
