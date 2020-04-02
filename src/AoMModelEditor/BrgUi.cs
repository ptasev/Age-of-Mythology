namespace AoMModelEditor
{
    using AoMEngineLibrary.Graphics;
    using AoMEngineLibrary.Graphics.Brg;
    using AoMEngineLibrary.Graphics.Model;
    using BrightIdeasSoftware;
    using grendgine_collada;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class BrgUi : IModelUI
    {
        public BrgFile File { get; set; }
        public MainForm Plugin { get; set; }
        public string FileName { get; set; }
        public int FilterIndex { get { return 1; } }

        private bool uniformAttachpointScale;
        private bool modelAtCenter;

        public BrgUi(MainForm plugin)
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
            this.File.Meshes.Add(new BrgMesh(this.File));
            this.FileName = Path.GetDirectoryName(this.FileName) + "\\Untitled";
            this.File.Meshes[0].Header.InterpolationType = BrgMeshInterpolationType.Default;
            this.File.Meshes[0].Header.Flags = BrgMeshFlag.TEXCOORDSA | BrgMeshFlag.MATERIAL | BrgMeshFlag.ATTACHPOINTS;
            this.File.Meshes[0].Header.Format = BrgMeshFormat.HASFACENORMALS | BrgMeshFormat.ANIMATED;
            this.File.Meshes[0].Header.AnimationType = BrgMeshAnimType.KeyFrame;
        }
        #endregion

        #region Import/Export
        public void Import(string fileName)
        {
            // Only need to Update the HEADER, and Animation.Duration, ASETHEADER is auto handled
            this.File = new BrgFile();
            BrgFile model = this.File; // just ref here so I don't have to rename model to this.File
            Grendgine_Collada cModel = Grendgine_Collada.Grendgine_Load_File(fileName);

            //Materials
            BrgMaterial mat = new BrgMaterial(model);
            model.Materials.Add(mat);
            mat.AmbientColor = new Color3D();
            mat.DiffuseColor = new Color3D();
            mat.SpecularColor = new Color3D(0.5f);
            mat.SpecularExponent = 5;
            mat.Id = 100;
            mat.Opacity = 1f;
            mat.Flags |= BrgMatFlag.HasTexture | BrgMatFlag.SpecularExponent;

            model.Header.NumMaterials = model.Materials.Count();

            //Meshes
            foreach (Grendgine_Collada_Geometry geo in cModel.Library_Geometries.Geometry)
            {
                BrgMesh mesh = new BrgMesh(model);
                model.Meshes.Add(mesh);

                mesh.Header.Flags |= BrgMeshFlag.MATERIAL;
                mesh.Header.Flags |= model.Meshes.Count == 1 ? 0 : BrgMeshFlag.SECONDARYMESH;
                mesh.Header.AnimationType |= BrgMeshAnimType.KeyFrame;
                mesh.Header.Format |= 0;
                mesh.ExtendedHeader.NumMaterials = (byte)model.Header.NumMaterials;
                mesh.ExtendedHeader.AnimationLength = 30;
                mesh.ExtendedHeader.NumUniqueMaterials = 0;

                string polyVertSourceID;
                string polyNormalsSourceID;
                string materialID;
                int[] vertCountPerPoly;
                int[] vertLinkPerPoly;
                int[] vertNormalBindings;

                //Locate the vertices and convert them
                string vertexPosSourceID = geo.Mesh.Vertices.Input.First<Grendgine_Collada_Input_Unshared>(x => x.Semantic == Grendgine_Collada_Input_Semantic.POSITION).source;
                Grendgine_Collada_Float_Array vertsArray = FindSourceByID(geo.Mesh, vertexPosSourceID).Float_Array;

                mesh.Vertices = FloatToVectorArray(vertsArray);
                mesh.Header.NumVertices = (short)mesh.Vertices.Count;

                //Check for polygons otherwise skip mesh
                if (geo.Mesh.Polylist != null || geo.Mesh.Polylist.Length > 0)
                {
                    mesh.Header.NumFaces = (short)geo.Mesh.Polylist[0].Count;
                    polyVertSourceID = geo.Mesh.Polylist[0].Input.First<Grendgine_Collada_Input_Unshared>(x => x.Semantic == Grendgine_Collada_Input_Semantic.VERTEX).source;
                    polyNormalsSourceID = geo.Mesh.Polylist[0].Input.First<Grendgine_Collada_Input_Unshared>(x => x.Semantic == Grendgine_Collada_Input_Semantic.NORMAL).source;
                    vertCountPerPoly = geo.Mesh.Polylist[0].VCount.Value();
                    materialID = geo.Mesh.Polylist[0].Material;
                    vertLinkPerPoly = geo.Mesh.Polylist[0].P.Value();

                    mesh.Faces = new List<Face>(mesh.Header.NumFaces);

                    vertNormalBindings = new int[mesh.Header.NumVertices];

                    int polyindex = 0;
                    Face ff;
                    foreach (int count in vertCountPerPoly)
                    {
                        if (count == 3) //If triangle
                        {
                            ff = new Face();
                            ff.Indices = new List<short>(3);
                            ff.Indices.Add((short)vertLinkPerPoly[polyindex]);
                            ff.Indices.Add((short)vertLinkPerPoly[polyindex + 4]);
                            ff.Indices.Add((short)vertLinkPerPoly[polyindex + 2]);
                            //List correct normal bindings
                            vertNormalBindings[ff.Indices[0]] = vertLinkPerPoly[polyindex + 1];
                            vertNormalBindings[ff.Indices[1]] = vertLinkPerPoly[polyindex + 5];
                            vertNormalBindings[ff.Indices[2]] = vertLinkPerPoly[polyindex + 3];
                            //Bind materials
                            if (mesh.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                                ff.MaterialIndex = (short)mat.Id;
                            mesh.Faces.Add(ff);
                        }
                        polyindex += count * 2; //Including face normal bindings
                    }
                }
                else
                {
                    break;
                }

                //Locate the vertex normals
                Grendgine_Collada_Float_Array normalsArray = FindSourceByID(geo.Mesh, polyNormalsSourceID).Float_Array;
                if (normalsArray.Count != vertsArray.Count)
                {
                    System.Windows.Forms.MessageBox.Show("The mesh hash only face normals instead of vertex normals. Be sure to export only smooth shaded models.", "Model Import Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                }
                List<Vector3> unsortedNormals = FloatToVectorArray(normalsArray);
                mesh.Normals = new List<Vector3>(mesh.Header.NumVertices);
                for (int i = 0; i < mesh.Header.NumVertices; i++)
                {
                    mesh.Normals.Add(unsortedNormals[vertNormalBindings[i]]);
                }

                mesh.VertexMaterials = new List<short>(mesh.Header.NumVertices);
                for (int i = 0; i < mesh.Header.NumVertices; i++)
                {
                    mesh.VertexMaterials.Add((short)mat.Id);
                }
            }

            model.Header.NumMeshes = model.Meshes.Count();
        }

        public void Export(string fileName)
        {
        }

        /// <summary>
        /// Search a Grendgine_Collada_Mesh for a source by a given ID
        /// </summary>
        /// <param name="id">Source id including #</param>
        private Grendgine_Collada_Source FindSourceByID(Grendgine_Collada_Mesh mesh, string id)
        {
            return mesh.Source.First<Grendgine_Collada_Source>(x => x.ID == id.TrimStart('#'));
        }
        /// <summary>
        /// Convert a list of Grendgine_Collada_Float_Array to an array of Vectors
        /// </summary>
        private List<Vector3> FloatToVectorArray(Grendgine_Collada_Float_Array colArray)
        {
            List<Vector3> vecArray = new List<Vector3>(colArray.Count / 3);

            float[] array = colArray.Value();

            for (int i = 0; i < colArray.Count / 3; i++)
            {
                vecArray.Add(new Vector3(
                        array[i * 3],
                        array[i * 3 + 2],
                        array[i * 3 + 1]));
            }

            return vecArray;
        }
        #endregion

        #region UI
        public void LoadUI()
        {
            this.Plugin.Text = MainForm.PluginTitle + " - " + Path.GetFileName(this.FileName);

            this.Plugin.brgObjectsTreeListView.ClearObjects();
            this.Plugin.brgObjectsTreeListView.AddObject(this.File.Meshes[0]);
            this.Plugin.brgObjectsTreeListView.AddObjects(this.File.Meshes[0].Attachpoints);
            this.Plugin.brgObjectsTreeListView.AddObjects(this.File.Materials);
            this.Plugin.brgObjectsTreeListView.SelectObject(this.File.Meshes[0], true);

            // General Info
            this.Plugin.brgImportAttachScaleCheckBox.Checked = this.uniformAttachpointScale;
            this.Plugin.brgImportCenterModelCheckBox.Checked = this.modelAtCenter;

            this.Plugin.vertsValueToolStripStatusLabel.Text = this.File.Meshes[0].Vertices.Count.ToString();
            this.Plugin.facesValueToolStripStatusLabel.Text = this.File.Meshes[0].Faces.Count.ToString();
            this.Plugin.meshesValueToolStripStatusLabel.Text = (this.File.Meshes[0].MeshAnimations.Count + 1).ToString();
            this.Plugin.matsValueToolStripStatusLabel.Text = this.File.Materials.Count.ToString();
            this.Plugin.animLengthValueToolStripStatusLabel.Text = this.File.Meshes[0].ExtendedHeader.AnimationLength.ToString();
        }
        public void LoadMeshUI()
        {
            this.Plugin.brgObjectListView.Columns.Clear();
            OLVColumn flagsCol = new OLVColumn("Flags", "Header.Flags");
            flagsCol.Width = 200;
            flagsCol.AspectPutter = delegate(object rowObject, object newValue)
            {
                this.File.UpdateMeshSettings((BrgMeshFlag)newValue, this.File.Meshes[0].Header.Format,
                    this.File.Meshes[0].Header.AnimationType, this.File.Meshes[0].Header.InterpolationType);
            };
            this.Plugin.brgObjectListView.Columns.Add(flagsCol);

            OLVColumn formatCol = new OLVColumn("Format", "Header.Format");
            formatCol.Width = 200;
            formatCol.AspectPutter = delegate(object rowObject, object newValue)
            {
                this.File.UpdateMeshSettings(this.File.Meshes[0].Header.Flags, (BrgMeshFormat)newValue,
                    this.File.Meshes[0].Header.AnimationType, this.File.Meshes[0].Header.InterpolationType);
            };
            this.Plugin.brgObjectListView.Columns.Add(formatCol);

            OLVColumn animTypeCol = new OLVColumn("Animation Type", "Header.AnimationType");
            animTypeCol.Width = 100;
            animTypeCol.AspectPutter = delegate(object rowObject, object newValue)
            {
                this.File.UpdateMeshSettings(this.File.Meshes[0].Header.Flags, this.File.Meshes[0].Header.Format,
                    (BrgMeshAnimType)newValue, this.File.Meshes[0].Header.InterpolationType);
            };
            this.Plugin.brgObjectListView.Columns.Add(animTypeCol);

            OLVColumn interpTypeCol = new OLVColumn("Interpolation Type", "Header.InterpolationType");
            interpTypeCol.Width = 100;
            interpTypeCol.AspectPutter = delegate(object rowObject, object newValue)
            {
                this.File.UpdateMeshSettings(this.File.Meshes[0].Header.Flags, this.File.Meshes[0].Header.Format,
                    this.File.Meshes[0].Header.AnimationType, (BrgMeshInterpolationType)newValue);
            };
            this.Plugin.brgObjectListView.Columns.Add(interpTypeCol);
        }
        public void LoadMaterialUI()
        {
            this.Plugin.brgObjectListView.Columns.Clear();
            OLVColumn idCol = new OLVColumn("ID", "Id");
            idCol.Width = 35;
            idCol.IsEditable = false;
            this.Plugin.brgObjectListView.Columns.Add(idCol);

            OLVColumn flagsCol = new OLVColumn("Flags", "Flags");
            flagsCol.Width = 200;
            this.Plugin.brgObjectListView.Columns.Add(flagsCol);

            OLVColumn diffColorCol = new OLVColumn("Diffuse Color", "DiffuseColor");
            this.Plugin.brgObjectListView.Columns.Add(diffColorCol);
            OLVColumn ambColorCol = new OLVColumn("Ambient Color", "AmbientColor");
            this.Plugin.brgObjectListView.Columns.Add(ambColorCol);
            OLVColumn specColorCol = new OLVColumn("Specular Color", "SpecularColor");
            this.Plugin.brgObjectListView.Columns.Add(specColorCol);
            OLVColumn emisColorCol = new OLVColumn("Emissive Color", "EmissiveColor");
            this.Plugin.brgObjectListView.Columns.Add(emisColorCol);

            OLVColumn specLevelCol = new OLVColumn("Specular Level", "SpecularExponent");
            this.Plugin.brgObjectListView.Columns.Add(specLevelCol);

            OLVColumn opacityCol = new OLVColumn("Opacity", "Opacity");
            this.Plugin.brgObjectListView.Columns.Add(opacityCol);

            OLVColumn diffMapCol = new OLVColumn("Diffuse Map", "DiffuseMap");
            this.Plugin.brgObjectListView.Columns.Add(diffMapCol);

            OLVColumn bumpMapCol = new OLVColumn("Bump Map", "BumpMap");
            this.Plugin.brgObjectListView.Columns.Add(bumpMapCol);

            OLVColumn reflMapCol = new OLVColumn("Reflection Map", string.Empty);
            reflMapCol.AspectGetter = delegate(object rowObject)
            {
                BrgMaterial mat = (BrgMaterial)rowObject;
                if (mat.sfx.Count > 0)
                {
                    return mat.sfx[0].Name;
                }
                else { return string.Empty; }
            };
            reflMapCol.AspectPutter = delegate(object rowObject, object newValue)
            {
                BrgMaterial mat = (BrgMaterial)rowObject;
                BrgMatSFX sfxMap = new BrgMatSFX();
                sfxMap.Id = 30;
                sfxMap.Name = (string)newValue;

                if (mat.sfx.Count > 0) { mat.sfx[0] = sfxMap; }
                else { mat.sfx.Add(sfxMap); }
            };
            this.Plugin.brgObjectListView.Columns.Add(reflMapCol);
        }
        public void LoadAttachpointUI()
        {
            this.Plugin.brgObjectListView.Columns.Clear();
            OLVColumn nameCol = new OLVColumn("Name", "Name");
            nameCol.Width = 100;
            nameCol.IsEditable = false;
            this.Plugin.brgObjectListView.Columns.Add(nameCol);

            OLVColumn posCol = new OLVColumn("Positon", "Position");
            posCol.Width = 250;
            posCol.IsEditable = false;
            this.Plugin.brgObjectListView.Columns.Add(posCol);
        }

        public void SaveUI()
        {
            this.uniformAttachpointScale = this.Plugin.brgImportAttachScaleCheckBox.Checked;
            this.modelAtCenter = this.Plugin.brgImportCenterModelCheckBox.Checked;
        }
        #endregion
    }
}
