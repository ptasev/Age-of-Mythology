namespace AoMModelEditor
{
    using AoMEngineLibrary.Graphics.Brg;
    using BrightIdeasSoftware;
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
        }

        public void Export(string fileName)
        {
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
            this.Plugin.meshesValueToolStripStatusLabel.Text = (this.File.Meshes.Count).ToString();
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
