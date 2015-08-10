namespace AoMModelEditor
{
    using AoMEngineLibrary.Graphics;
    using AoMEngineLibrary.Graphics.Brg;
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class BrgUi : IModelUi
    {
        public BrgFile File { get; set; }
        public MainForm Plugin { get; set; }
        public string FileName { get; set; }
        public int FilterIndex { get { return 1; } }

        public Byte InterpolationType { get; set; }
        public BrgMeshFlag Flags { get; set; }
        public BrgMeshFormat Format { get; set; }
        public BrgMeshAnimType AnimationType { get; set; }

        private BrgMaterial lastMatSelected;
        private bool uniformAttachpointScale;
        private bool modelAtCenter;

        public BrgUi(MainForm plugin)
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
            this.AnimationType = BrgMeshAnimType.KEYFRAME;
        }
        #endregion

        #region Import/Export
        public void Import()
        {
        }

        public void Export()
        {
        }
        #endregion

        #region UI
        public void LoadUi()
        {
            this.Plugin.Text = MainForm.PluginTitle + " - " + Path.GetFileName(this.FileName);
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
                if (this.File.Meshes[0].Header.AnimationType == BrgMeshAnimType.KEYFRAME)
                {
                    this.Plugin.keyframeRadioButton.Checked = true;
                }
                else if (this.File.Meshes[0].Header.AnimationType == BrgMeshAnimType.NONUNIFORM)
                {
                    this.Plugin.nonuniRadioButton.Checked = true;
                }
                else if (this.File.Meshes[0].Header.AnimationType == BrgMeshAnimType.SKINBONE)
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

                if (this.AnimationType == BrgMeshAnimType.KEYFRAME)
                {
                    this.Plugin.keyframeRadioButton.Checked = true;
                }
                else if (this.AnimationType == BrgMeshAnimType.NONUNIFORM)
                {
                    this.Plugin.nonuniRadioButton.Checked = true;
                }
                else if (this.AnimationType == BrgMeshAnimType.SKINBONE)
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
            this.Plugin.attachpointListBox.DataSource = this.File.Meshes[0].Attachpoints;
        }

        public void SaveUi()
        {
            this.uniformAttachpointScale = this.Plugin.brgImportAttachScaleCheckBox.Checked;
            this.modelAtCenter = this.Plugin.brgImportCenterModelCheckBox.Checked;
            this.InterpolationType = Convert.ToByte(this.Plugin.interpolationTypeCheckBox.Checked);
            this.Flags = this.Plugin.
                GetCheckedListBoxSelectedEnums<BrgMeshFlag>(this.Plugin.genMeshFlagsCheckedListBox);
            this.Format = this.Plugin.
                GetCheckedListBoxSelectedEnums<BrgMeshFormat>(this.Plugin.genMeshFormatCheckedListBox);
            if (this.Plugin.keyframeRadioButton.Checked)
            {
                this.AnimationType = BrgMeshAnimType.KEYFRAME;
            }
            else if (this.Plugin.nonuniRadioButton.Checked)
            {
                this.AnimationType = BrgMeshAnimType.NONUNIFORM;
            }
            else if (this.Plugin.skinBoneRadioButton.Checked)
            {
                this.AnimationType = BrgMeshAnimType.SKINBONE;
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
