using AoMEngineLibrary.Graphics;
using AoMEngineLibrary.Graphics.Brg;
using AoMEngineLibrary.Graphics.Model;
using ManagedServices;
using MaxCustomControls;
using MiscUtil;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace AoMEngineLibrary
{
    public partial class MaxPluginForm : MaxForm
    {
        public static class Settings
        {
            private static string fileName = Application.StartupPath + "\\AoMEngineLibraryPluginSettings.xml";
            public static string OpenFileDialogFileName;
            public static string SaveFileDialogFileName;
            public static string MtrlFolderDialogDirectory;

            public static void Read()
            {
                try
                {
                    if (!File.Exists(fileName))
                    {
                        return;
                    }
                    using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        XmlDocument settings = new XmlDocument();
                        settings.Load(fs);
                        foreach (XmlElement elem in settings.DocumentElement)
                        {
                            if (elem.Name == "defaultOpenDirectory")
                            {
                                OpenFileDialogFileName = elem.InnerText;
                            }
                            else if (elem.Name == "defaultSaveDirectory")
                            {
                                SaveFileDialogFileName = elem.InnerText;
                            }
                            else if (elem.Name == "defaultMtrlSaveDirectory")
                            {
                                MtrlFolderDialogDirectory = elem.InnerText;
                            }
                        }
                    }
                }
                catch { }
            }

            public static void Write()
            {

            }
        }

        public float ExportedScaleFactor
        {
            get
            {
                return Single.Parse(timeMultMaxTextBox.Text);
            }
        }
        public Byte InterpolationType
        {
            get
            {
                return (byte)Int16.Parse(interpolationTypeMaxTextBox.Text);
            }
        }
        public BrgMeshFlag Flags
        {
            get
            {
                return getCheckedListBoxSelectedEnums<BrgMeshFlag>(genMeshFlagsCheckedListBox);
            }
        }
        public BrgMeshFormat Format
        {
            get
            {
                return getCheckedListBoxSelectedEnums<BrgMeshFormat>(genMeshFormatCheckedListBox);
            }
        }
        public BrgMeshAnimType AnimationType
        {
            get
            {
                return getCheckedListBoxSelectedEnums<BrgMeshAnimType>(genMeshPropsCheckedListBox);
            }
        }
        public BrgMatFlag MatFlags
        {
            get
            {
                return getCheckedListBoxSelectedEnums<BrgMatFlag>(materialFlagsCheckedListBox);
            }
        }

        public static CuiUpdater uiUp;
        BrgFile file;
        bool isExportedToMax;

        public MaxPluginForm()
        {
            InitializeComponent();
            this.TopMost = true;
            this.MaximizeBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            openFileDialog.Filter = "brg files|*.brg";
            saveFileDialog.Filter = "brg files|*.brg";
            saveFileDialog.AddExtension = false;

            // Update Colors
            uiUp = CuiUpdater.GetInstance();
            mainMenuStrip.Renderer = new ToolStripMaxPluginRenderer();

            for (int i = 0; i < mainTabControl.TabPages.Count; i++)
            {
                mainTabControl.TabPages[i].BackColor = uiUp.GetControlColor();
                mainTabControl.TabPages[i].ForeColor = uiUp.GetTextColor();
            }

            // General Tab
            generalDataGroupBox.ForeColor = uiUp.GetTextColor();
            attachpointGroupBox.ForeColor = uiUp.GetTextColor();
            genMeshFlagsGroupBox.ForeColor = uiUp.GetTextColor();
            genMeshFormatGroupBox.ForeColor = uiUp.GetTextColor();
            genMeshPropsGroupBox.ForeColor = uiUp.GetTextColor();

            numVertsMaxTextBox.BackColor = uiUp.GetEditControlColor();
            numVertsMaxTextBox.ForeColor = uiUp.GetTextColor();
            numVertsMaxTextBox.BorderStyle = BorderStyle.FixedSingle;
            numFacesMaxTextBox.BackColor = uiUp.GetEditControlColor();
            numFacesMaxTextBox.ForeColor = uiUp.GetTextColor();
            numFacesMaxTextBox.BorderStyle = BorderStyle.FixedSingle;
            numMeshMaxTextBox.BackColor = uiUp.GetEditControlColor();
            numMeshMaxTextBox.ForeColor = uiUp.GetTextColor();
            numMeshMaxTextBox.BorderStyle = BorderStyle.FixedSingle;
            numMatMaxTextBox.BackColor = uiUp.GetEditControlColor();
            numMatMaxTextBox.ForeColor = uiUp.GetTextColor();
            numMatMaxTextBox.BorderStyle = BorderStyle.FixedSingle;
            animTimeMaxTextBox.BackColor = uiUp.GetEditControlColor();
            animTimeMaxTextBox.ForeColor = uiUp.GetTextColor();
            animTimeMaxTextBox.BorderStyle = BorderStyle.FixedSingle;
            timeMultMaxTextBox.BackColor = uiUp.GetEditControlColor();
            timeMultMaxTextBox.ForeColor = uiUp.GetTextColor();
            timeMultMaxTextBox.BorderStyle = BorderStyle.FixedSingle;
            interpolationTypeMaxTextBox.BackColor = uiUp.GetEditControlColor();
            interpolationTypeMaxTextBox.ForeColor = uiUp.GetTextColor();
            interpolationTypeMaxTextBox.BorderStyle = BorderStyle.FixedSingle;
            updateSettingsButton.FlatStyle = FlatStyle.Flat;
            updateSettingsButton.BackColor = uiUp.GetEditControlColor();
            updateSettingsButton.ForeColor = uiUp.GetTextColor();

            genMeshFlagsCheckedListBox.BackColor = uiUp.GetEditControlColor();
            genMeshFlagsCheckedListBox.ForeColor = uiUp.GetTextColor();
            genMeshFlagsCheckedListBox.BorderStyle = BorderStyle.None;
            genMeshFlagsCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMeshFlag));
            genMeshFormatCheckedListBox.BackColor = uiUp.GetEditControlColor();
            genMeshFormatCheckedListBox.ForeColor = uiUp.GetTextColor();
            genMeshFormatCheckedListBox.BorderStyle = BorderStyle.None;
            genMeshFormatCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMeshFormat));
            genMeshPropsCheckedListBox.BackColor = uiUp.GetEditControlColor();
            genMeshPropsCheckedListBox.ForeColor = uiUp.GetTextColor();
            genMeshPropsCheckedListBox.BorderStyle = BorderStyle.None;
            genMeshPropsCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMeshAnimType));

            // Attachpoints
            attachpointComboBox.SelectedIndexChanged += attachpointComboBox_SelectedIndexChanged;
            //attachpointComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //attachpointComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            attachpointComboBox.FlatStyle = FlatStyle.Flat;
            attachpointComboBox.BackColor = uiUp.GetEditControlColor();
            attachpointComboBox.ForeColor = uiUp.GetTextColor();
            attachpointListBox.MouseDoubleClick += attachpointListBox_MouseDoubleClick;
            attachpointListBox.BackColor = uiUp.GetEditControlColor();
            attachpointListBox.ForeColor = uiUp.GetTextColor();
            attachpointListBox.DataSource = BrgAttachpoint.AttachpointNames;

            // Materials
            materialGroupBox.BackColor = uiUp.GetControlColor();
            materialGroupBox.ForeColor = uiUp.GetTextColor();
            diffuseMaxTextBox.BackColor = uiUp.GetEditControlColor();
            diffuseMaxTextBox.ForeColor = uiUp.GetTextColor();
            ambientMaxTextBox.BackColor = uiUp.GetEditControlColor();
            ambientMaxTextBox.ForeColor = uiUp.GetTextColor();
            specularMaxTextBox.BackColor = uiUp.GetEditControlColor();
            specularMaxTextBox.ForeColor = uiUp.GetTextColor();
            selfIllumMaxTextBox.BackColor = uiUp.GetEditControlColor();
            selfIllumMaxTextBox.ForeColor = uiUp.GetTextColor();
            textureMaxTextBox.BackColor = uiUp.GetEditControlColor();
            textureMaxTextBox.ForeColor = uiUp.GetTextColor();
            reflectionMaxTextBox.BackColor = uiUp.GetEditControlColor();
            reflectionMaxTextBox.ForeColor = uiUp.GetTextColor();
            unknownMaxTextBox.BackColor = uiUp.GetEditControlColor();
            unknownMaxTextBox.ForeColor = uiUp.GetTextColor();
            opacityMaxTextBox.BackColor = uiUp.GetEditControlColor();
            opacityMaxTextBox.ForeColor = uiUp.GetTextColor();
            specularLevelMaxTextBox.BackColor = uiUp.GetEditControlColor();
            specularLevelMaxTextBox.ForeColor = uiUp.GetTextColor();
            updateMatSettingsButton.FlatStyle = FlatStyle.Flat;
            updateMatSettingsButton.BackColor = uiUp.GetEditControlColor();
            updateMatSettingsButton.ForeColor = uiUp.GetTextColor();
            materialFlagsGroupBox.BackColor = uiUp.GetControlColor();
            materialFlagsGroupBox.ForeColor = uiUp.GetTextColor();
            materialListBox.SelectedIndexChanged += materialListBox_SelectedIndexChanged;
            materialListBox.BackColor = uiUp.GetEditControlColor();
            materialListBox.ForeColor = uiUp.GetTextColor();
            materialFlagsCheckedListBox.BackColor = uiUp.GetEditControlColor();
            materialFlagsCheckedListBox.ForeColor = uiUp.GetTextColor();
            materialFlagsCheckedListBox.BorderStyle = BorderStyle.FixedSingle;
            materialFlagsCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMatFlag));

            //plugin = new MaxPlugin();
            //this.Controls.Add(plugin);
            //plugin.Dock = DockStyle.Fill;
            Settings.Read();
            newToolStripMenuItem_Click(this, new EventArgs());
        }

        #region UiColors
        private Color ContrastColor(Color color)
        {
            int d = 0;

            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;

            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font

            return Color.FromArgb(d, d, d);
        }

        void materialListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialListBox.SelectedIndex >= 0)
            {
                BrgMaterial mat = file.Materials[materialListBox.SelectedIndex];
                // Update Info
                diffuseMaxTextBox.BackColor = Color.FromArgb(Convert.ToByte(mat.DiffuseColor.R * Byte.MaxValue),
                    Convert.ToByte(mat.DiffuseColor.G * Byte.MaxValue), 
                    Convert.ToByte(mat.DiffuseColor.B * Byte.MaxValue));
                diffuseMaxTextBox.ForeColor = ContrastColor(diffuseMaxTextBox.BackColor);
                ambientMaxTextBox.BackColor = Color.FromArgb(Convert.ToByte(mat.AmbientColor.R * Byte.MaxValue),
                    Convert.ToByte(mat.AmbientColor.G * Byte.MaxValue),
                    Convert.ToByte(mat.AmbientColor.B * Byte.MaxValue));
                ambientMaxTextBox.ForeColor = ContrastColor(ambientMaxTextBox.BackColor);
                specularMaxTextBox.BackColor = Color.FromArgb(Convert.ToByte(mat.SpecularColor.R * Byte.MaxValue),
                    Convert.ToByte(mat.SpecularColor.G * Byte.MaxValue),
                    Convert.ToByte(mat.SpecularColor.B * Byte.MaxValue));
                specularMaxTextBox.ForeColor = ContrastColor(specularMaxTextBox.BackColor);
                selfIllumMaxTextBox.BackColor = Color.FromArgb(Convert.ToByte(mat.EmissiveColor.R * Byte.MaxValue),
                    Convert.ToByte(mat.EmissiveColor.G * Byte.MaxValue),
                    Convert.ToByte(mat.EmissiveColor.B * Byte.MaxValue));
                selfIllumMaxTextBox.ForeColor = ContrastColor(selfIllumMaxTextBox.BackColor);
                specularLevelMaxTextBox.Text = mat.SpecularExponent.ToString();
                opacityMaxTextBox.Text = mat.Opacity.ToString();
                textureMaxTextBox.Text = mat.DiffuseMap;
                if (mat.sfx.Count > 0)
                {
                    reflectionMaxTextBox.Text = mat.sfx[0].Name;
                }
                else { reflectionMaxTextBox.Text = string.Empty; }
                unknownMaxTextBox.Text = mat.BumpMap;

                // Update Flags box
                for (int i = 0; i < materialFlagsCheckedListBox.Items.Count; i++)
                {
                    if (mat.Flags.HasFlag((BrgMatFlag)materialFlagsCheckedListBox.Items[i]))
                    {
                        materialFlagsCheckedListBox.SetItemChecked(i, true);
                    }
                    else
                    {
                        materialFlagsCheckedListBox.SetItemChecked(i, false);
                    }
                }
            }
        }

        private class ToolStripMaxPluginRenderer : ToolStripProfessionalRenderer
        {
            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
                e.ToolStrip.BackColor = uiUp.GetControlColor();
                //base.OnRenderImageMargin(e);
            }

            protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
            {
                e.ToolStripPanel.BackColor = uiUp.GetControlColor();
                //base.OnRenderToolStripPanelBackground(e);
            }
            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                e.ToolStrip.BackColor = uiUp.GetControlColor();
                e.ToolStrip.ForeColor = uiUp.GetTextColor();
                //base.OnRenderToolStripBackground(e);
                //Rectangle rc = new Rectangle(Point.Empty, e.ToolStrip.Size);
                //e.Graphics.FillRectangle(new SolidBrush(uiUp.GetControlColor()), rc);
                //e.Graphics.DrawRectangle(Pens.Blue, 1, 0, rc.Width - 4, rc.Height - 1);
            }
            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                if (e.ToolStrip.GetType() == typeof(MenuStrip))
                {
                    // skip render border of main
                }
                else
                {
                    // do render border
                    //base.OnRenderToolStripBorder(e);
                    Rectangle rc = new Rectangle(Point.Empty, e.ToolStrip.Size);
                    //e.Graphics.FillRectangle(new SolidBrush(uiUp.GetEditControlColor()), rc);
                    e.Graphics.DrawRectangle(Pens.Black, 0, 0, rc.Width - 1, rc.Height - 1);
                }
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (!e.Item.Selected)
                {
                    //for (int i = 0; i < e.Item.)
                    //base.OnRenderMenuItemBackground(e);
                    //Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
                    //e.Graphics.FillRectangle(new SolidBrush(uiUp.GetControlColor()), rc);
                    //e.Graphics.DrawRectangle(Pens.Black, 1, 0, rc.Width - 4, rc.Height - 1);
                }
                else
                {
                    // Highlight selected item
                    Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
                    e.Graphics.FillRectangle(new SolidBrush(uiUp.GetEditControlColor()), 3, 1, rc.Width - 6, rc.Height - 3);
                    e.Graphics.DrawRectangle(Pens.Black, 3, 1, rc.Width - 6, rc.Height - 3);
                }
                //base.OnRenderMenuItemBackground(e);
            }
        }
        #endregion

        #region MainMenu
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            file = new BrgFile();
            loadUI();
            genMeshFlagsCheckedListBox.SetItemChecked(1, true);
            genMeshFlagsCheckedListBox.SetItemChecked(6, true);
            genMeshFlagsCheckedListBox.SetItemChecked(8, true);
            genMeshFormatCheckedListBox.SetItemChecked(2, true);
            genMeshFormatCheckedListBox.SetItemChecked(3, true);
            this.Text = "ABE - Untitled";
            isExportedToMax = true;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Settings.OpenFileDialogFileName))
            {
                openFileDialog.InitialDirectory = Settings.OpenFileDialogFileName;
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    file = new BrgFile(File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
                    //file.ExportToMax();
                    //debug();

                    loadUI();
                    this.Text = "ABE - " + Path.GetFileName(openFileDialog.FileName);
                    isExportedToMax = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to open file!" + Environment.NewLine + Environment.NewLine + ex.Message, "ABE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void debug()
        {
            using (TextWriter writer = File.CreateText(Path.GetFileName(file.FileName) + ".txt"))
            {
                for (int i = 0; i < Maxscript.Output.Count; i++)
                {
                    writer.Write(Maxscript.Output[i]);
                    writer.Write(writer.NewLine);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (file == null)
            {
                return;
            }

            //saveFileDialog.FileName = "archerTest.brg";
            if (!string.IsNullOrEmpty(Settings.SaveFileDialogFileName))
            {
                saveFileDialog.InitialDirectory = Settings.SaveFileDialogFileName;
            }
            else if (!string.IsNullOrEmpty(file.FileName))
            {
                saveFileDialog.FileName = Path.GetFileNameWithoutExtension(file.FileName);
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(file.FileName);
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    file.Write(File.Open(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read));
                    this.Text = "ABE - " + Path.GetFileName(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save file!" + Environment.NewLine + Environment.NewLine + ex.Message, "ABE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void exportToMaxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (file == null)
            {
                return;
            }


            DialogResult dlgR = MessageBox.Show("Do you want to clear the scene?", "ABE", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
            if (dlgR == DialogResult.Yes)
            {
                Maxscript.Command("resetMaxFile #noprompt");
                //Maxscript.Command("if checkForSave() do resetMaxFile #noprompt");
            }
            else if (dlgR == DialogResult.Cancel)
            {
                return;
            }

            ExportBrgToMax();
            //debug();
            isExportedToMax = true;
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to export model!" + Environment.NewLine + Environment.NewLine + ex.Message, "ABE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void importFromMaxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (file == null)
            {
                return;
            }

            bool objectSelected = Maxscript.QueryBoolean("classOf selection[1] == Editable_mesh");
            if (!objectSelected)
            {
                throw new Exception("No object selected!");
            }
            ImportBrgFromMax();
            loadUI();
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to import model!" + Environment.NewLine + Environment.NewLine + ex.Message, "ABE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void extractMTRLFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (file == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(Settings.MtrlFolderDialogDirectory))
            {
                folderBrowserDialog.SelectedPath = Settings.MtrlFolderDialogDirectory;
            }
            else if (!string.IsNullOrEmpty(file.FileName))
            {
                folderBrowserDialog.SelectedPath = Path.GetDirectoryName(file.FileName);
            }

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < file.Materials.Count; i++)
                {
                    file.Materials[i].WriteExternal(File.Open(Path.Combine(folderBrowserDialog.SelectedPath, Path.GetFileNameWithoutExtension(file.FileName) + "_" + i + ".mtrl"), FileMode.Create, FileAccess.Write, FileShare.Read));
                }
            }
        }

        private void flagDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.google.com/spreadsheets/d/1m6AZHjt3YU4fxH_-w9Smi1WEBa651PXgavQrC_a1_1o/edit?usp=sharing");
        }
        #endregion

        #region Brg
        void attachpointListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!isExportedToMax)
            {
                return;
            }
            int index = attachpointListBox.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches && file != null)
            {
                if (file.Meshes.Count == 0)
                {
                    file.Meshes.Add(new BrgMesh(file));
                }
                BrgAttachpoint att = new BrgAttachpoint();
                att.NameId = BrgAttachpoint.GetIdByName((string)attachpointListBox.Items[index]);
                file.Meshes[0].Attachpoints.Add(att);
                //MessageBox.Show(file.Mesh[0].attachpoints.Count.ToString());
                Maxscript.NewDummy("newDummy", att.GetMaxName(), att.GetMaxTransform(), att.GetMaxPosition(), att.GetMaxBoxSize(), att.GetMaxScale());
                loadUIAttachpoint();
                attachpointComboBox.SelectedIndex = attachpointComboBox.Items.Count - 1;
            }
        }
        void attachpointComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isExportedToMax)
            {
                return;
            }
            if (attachpointComboBox.SelectedItem != null && file != null && file.Meshes.Count > 0)
            {
                Maxscript.Command("selectDummy = getNodeByName \"{0}\"", ((BrgAttachpoint)attachpointComboBox.SelectedItem).GetMaxName());
                if (Maxscript.QueryBoolean("selectDummy != undefined"))
                {
                    Maxscript.Command("select selectDummy");
                }
                else
                {
                    DialogResult dlgR = MessageBox.Show("Could not find \"" + ((BrgAttachpoint)attachpointComboBox.SelectedItem).GetMaxName() + "\"! Would you like to delete it?", "ABE",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dlgR == DialogResult.Yes)
                    {
                        file.Meshes[0].Attachpoints.Remove(((BrgAttachpoint)attachpointComboBox.SelectedItem).Index);
                        loadUIAttachpoint();
                    }
                }
            }
        }

        private void loadUIMaterial()
        {
            materialListBox.DataSource = null;
            materialListBox.DataSource = file.Materials;
            materialListBox.DisplayMember = "EditorName";

        }
        private void loadUIAttachpoint()
        {
            //attachpointComboBox.DataSource = null;
            //attachpointComboBox.DataSource = file.Mesh[0].attachpoints.Values;
            attachpointComboBox.Items.Clear();
            foreach (BrgAttachpoint att in file.Meshes[0].Attachpoints)
            {
                attachpointComboBox.Items.Add(att);
            }
            attachpointComboBox.ValueMember = "Index";
            attachpointComboBox.DisplayMember = "MaxName";
        }
        private void loadUI()
        {
            // Materials
            loadUIMaterial();

            // General Info
            numVertsMaxTextBox.Text = "0";
            numFacesMaxTextBox.Text = "0";
            if (file.Meshes.Count > 0)
            {
                numMeshMaxTextBox.Text = (file.Meshes[0].MeshAnimations.Count + 1).ToString();
            }
            else
            {
                numMeshMaxTextBox.Text = 0.ToString();
            }
            numMatMaxTextBox.Text = file.Materials.Count.ToString();
            animTimeMaxTextBox.Text = "0";
            timeMultMaxTextBox.Text = "1";
            interpolationTypeMaxTextBox.Text = "0";
            //MessageBox.Show("1");

            if (file.Meshes.Count > 0)
            {
                numVertsMaxTextBox.Text = file.Meshes[0].Vertices.Count.ToString();
                numFacesMaxTextBox.Text = file.Meshes[0].Faces.Count.ToString();
                //MessageBox.Show("2.1");
                animTimeMaxTextBox.Text = file.Meshes[0].ExtendedHeader.AnimationLength.ToString();
                timeMultMaxTextBox.Text = file.Meshes[0].ExtendedHeader.ExportedScaleFactor.ToString();
                //MessageBox.Show("2.2");
                interpolationTypeMaxTextBox.Text = file.Meshes[0].Header.InterpolationType.ToString();
                //MessageBox.Show("2.3");

                // Attachpoints
                loadUIAttachpoint();

                for (int i = 0; i < genMeshFlagsCheckedListBox.Items.Count; i++)
                {
                    if (file.Meshes[0].Header.Flags.HasFlag((BrgMeshFlag)genMeshFlagsCheckedListBox.Items[i]))
                    {
                        genMeshFlagsCheckedListBox.SetItemChecked(i, true);
                    }
                    else
                    {
                        genMeshFlagsCheckedListBox.SetItemChecked(i, false);
                    }
                }
                for (int i = 0; i < genMeshFormatCheckedListBox.Items.Count; i++)
                {
                    if (file.Meshes[0].Header.Format.HasFlag((BrgMeshFormat)genMeshFormatCheckedListBox.Items[i]))
                    {
                        genMeshFormatCheckedListBox.SetItemChecked(i, true);
                    }
                    else
                    {
                        genMeshFormatCheckedListBox.SetItemChecked(i, false);
                    }
                }
                for (int i = 0; i < genMeshPropsCheckedListBox.Items.Count; i++)
                {
                    if (file.Meshes[0].Header.AnimationType.HasFlag((BrgMeshAnimType)genMeshPropsCheckedListBox.Items[i]))
                    {
                        genMeshPropsCheckedListBox.SetItemChecked(i, true);
                    }
                    else
                    {
                        genMeshPropsCheckedListBox.SetItemChecked(i, false);
                    }
                }
            }
        }
        private T getCheckedListBoxSelectedEnums<T>(CheckedListBox box)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            T enumVal = Operator.Convert<int, T>(0);
            for (int i = 0; i < box.CheckedItems.Count; i++)
            {
                enumVal = Operator.Convert<int, T>(Operator.Or<int>(Operator.Convert<T, int>(enumVal), Operator.Convert<T, int>(Operator.Convert<object, T>(box.CheckedItems[i]))));
            }

            return enumVal;
        }

        private void updateSettingsButton_Click(object sender, EventArgs e)
        {
            if (file == null)
            {
                return;
            }

            this.file.UpdateMeshSettings(this.Flags, this.Format, this.AnimationType, this.InterpolationType, this.ExportedScaleFactor);

            loadUI();
        }
        private void updateMatSettingsButton_Click(object sender, EventArgs e)
        {
            if (file == null)
            {
                return;
            }

            ((BrgMaterial)materialListBox.SelectedItems[0]).Flags = this.MatFlags;
        }

        public void ExportBrgToMax()
        {
            BrgFile brg = file as BrgFile;
            Maxscript.Command("frameRate = {0}", Math.Round(1 / file.Animation.TimeStep));
            Maxscript.Interval(0, file.Animation.Duration);

            if (file.Meshes.Count > 0)
            {
                string mainObject = "mainObj";
                //System.Windows.Forms.MessageBox.Show(file.Meshes[0].MeshAnimations.Count + " " + file.Animation.MeshChannel.MeshTimes.Count);
                for (int i = 0; i <= file.Meshes[0].MeshAnimations.Count; i++)
                {
                    Maxscript.CommentTitle("ANIMATE FRAME " + i);
                    if (i > 0)
                    {
                        ExportBrgMeshToMax(mainObject, ((BrgMesh)brg.Meshes[0].MeshAnimations[i - 1]), brg.Animation.MeshChannel.MeshTimes[i]);
                    }
                    else
                    {
                        ExportBrgMeshToMax(mainObject, brg.Meshes[0], brg.Animation.MeshChannel.MeshTimes[0]);
                    }
                }

                // Still can't figure out why it updates/overwrites normals ( geometry:false topology:false)
                // Seems like it was fixed in 3ds Max 2015 with setNormal command
                Maxscript.Command("update {0} geometry:false topology:false normals:false", mainObject);
                Maxscript.Command("select {0}", mainObject);
                //Maxscript.Command("addModifier {0} (Edit_Normals())", mainObject);
                Maxscript.Command("max zoomext sel all");

                if (file.Materials.Count > 0)
                {
                    Maxscript.CommentTitle("LOAD MATERIALS");
                    Maxscript.Command("matGroup = multimaterial numsubs:{0}", file.Materials.Count);
                    for (int i = 0; i < file.Materials.Count; i++)
                    {
                        Maxscript.Command("matGroup[{0}] = {1}", i + 1, ExportBrgMaterialToMax(brg.Materials[i]));
                        Maxscript.Command("matGroup.materialIDList[{0}] = {1}", i + 1, file.Materials[i].id);
                    }
                    Maxscript.Command("{0}.material = matGroup", mainObject);
                }
            }
        }
        public void ExportBrgMeshToMax(string mainObject, BrgMesh mesh, float time)
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
                        Maxscript.NewPoint3Literal<float>(-mesh.Vertices[i].X, -mesh.Vertices[i].Z, mesh.Vertices[i].Y));
                    //Maxscript.AnimateAtTime(time, "setNormal {0} {1} {2}", mainObject, i + 1,
                    //    Maxscript.NewPoint3Literal<float>(-this.Normals[i].X, -this.Normals[i].Z, this.Normals[i].Y));
                    //Maxscript.AnimateAtTime(time, "{0}.Edit_Normals.SetNormal {1} {2}", mainObject, i + 1,
                    //    Maxscript.NewPoint3Literal<float>(-this.Normals[i].X, -this.Normals[i].Z, this.Normals[i].Y));
                    //Maxscript.AnimateAtTime(time, "{0}.Edit_Normals.SetNormalExplicit {1}", mainObject, i + 1);

                    if (mesh.Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS) &&
                        mesh.Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                    {
                        Maxscript.Animate("{0}.Unwrap_UVW.SetVertexPosition {1}s {2} {3}", mainObject, time, i + 1,
                            Maxscript.NewPoint3Literal<float>(mesh.TextureCoordinates[i].X, mesh.TextureCoordinates[i].Y, 0));
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
                    //    -this.Normals[i].X, -this.Normals[i].Z, this.Normals[i].Y));

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
                    Maxscript.Append(faceArray, Maxscript.NewPoint3<Int32>("fV", face.Indices[0] + 1, face.Indices[1] + 1, face.Indices[2] + 1));
                }

                Maxscript.AnimateAtTime(time, Maxscript.NewMeshLiteral(mainObject, vertArray, normArray, faceArray, faceMats, texVerts));
                Maxscript.Command("{0} = getNodeByName \"{0}\"", mainObject);

                string groundPlanePos = Maxscript.NewPoint3<float>("groundPlanePos", -mesh.Header.HotspotPosition.X, -mesh.Header.HotspotPosition.Z, mesh.Header.HotspotPosition.Y);
                Maxscript.Command("plane name:\"ground\" pos:{0} length:10 width:10", groundPlanePos);

                Maxscript.CommentTitle("TVert Hack"); // Needed <= 3ds Max 2014; idk about 2015+
                Maxscript.Command("buildTVFaces {0}", mainObject);
                for (int i = 1; i <= mesh.Faces.Count; i++)
                {
                    Maxscript.Command("setTVFace {0} {1} (getFace {0} {1})", mainObject, i);
                }

                Maxscript.CommentTitle("Load Normals for first Frame");
                Maxscript.Command("max modify mode");
                //Maxscript.Command("select {0}", mainObject);
                //Maxscript.Command("addModifier {0} (Edit_Normals())", mainObject);
                for (int i = 0; i < mesh.Vertices.Count; i++)
                {
                    Maxscript.AnimateAtTime(time, "setNormal {0} {1} {2}", mainObject, i + 1,
                        Maxscript.NewPoint3Literal<float>(-mesh.Normals[i].X, -mesh.Normals[i].Z, mesh.Normals[i].Y));
                    //Maxscript.Command("{0}.Edit_Normals.SetNormal {1} {2}", mainObject, i + 1,
                    //    Maxscript.NewPoint3Literal<float>(-this.Normals[i].X, -this.Normals[i].Z, this.Normals[i].Y));
                    //Maxscript.Command("{0}.Edit_Normals.SetNormalExplicit {1}", mainObject, i + 1);
                }

                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
                {
                    Maxscript.Command("select {0}", mainObject);
                    Maxscript.Command("addModifier {0} (Unwrap_UVW())", mainObject);

                    Maxscript.Command("select {0}.verts", mainObject);
                    Maxscript.Animate("{0}.Unwrap_UVW.moveSelected [0,0,0]", mainObject);
                }
            }

            Maxscript.CommentTitle("Load Attachpoints");
            foreach (BrgAttachpoint att in mesh.Attachpoints)
            {
                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
                {
                    Maxscript.Command("attachpoint = getNodeByName \"{0}\"", att.GetMaxName());
                    Maxscript.AnimateAtTime(time, "attachpoint.rotation = {0}", att.GetMaxTransform());
                    Maxscript.AnimateAtTime(time, "attachpoint.position = {0}", att.GetMaxPosition());
                    Maxscript.AnimateAtTime(time, "attachpoint.scale = {0}", att.GetMaxScale());
                }
                else
                {
                    string attachDummy = Maxscript.NewDummy("attachDummy", att.GetMaxName(), att.GetMaxTransform(), att.GetMaxPosition(), att.GetMaxBoxSize(), att.GetMaxScale());
                }
            }
        }
        public string ExportBrgMaterialToMax(BrgMaterial mat)
        {
            Maxscript.Command("mat = StandardMaterial()");
            Maxscript.Command("mat.name = \"{0}\"", mat.DiffuseMap);
            Maxscript.Command("mat.adLock = false");
            Maxscript.Command("mat.useSelfIllumColor = true");
            Maxscript.Command("mat.diffuse = color {0} {1} {2}", mat.DiffuseColor.R * 255f, mat.DiffuseColor.G * 255f, mat.DiffuseColor.B * 255f);
            Maxscript.Command("mat.ambient = color {0} {1} {2}", mat.AmbientColor.R * 255f, mat.AmbientColor.G * 255f, mat.AmbientColor.B * 255f);
            Maxscript.Command("mat.specular = color {0} {1} {2}", mat.SpecularColor.R * 255f, mat.SpecularColor.G * 255f, mat.SpecularColor.B * 255f);
            Maxscript.Command("mat.selfIllumColor = color {0} {1} {2}", mat.EmissiveColor.R * 255f, mat.EmissiveColor.G * 255f, mat.EmissiveColor.B * 255f);
            Maxscript.Command("mat.opacity = {0}", mat.Opacity * 100f);
            Maxscript.Command("mat.specularLevel = {0}", mat.SpecularExponent);
            //MaxHelper.Command("print \"{0}\"", name);
            if (mat.Flags.HasFlag(BrgMatFlag.SubtractiveBlend))
            {
                Maxscript.Command("mat.opacityType = 1");
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.AdditiveBlend))
            {
                Maxscript.Command("mat.opacityType = 2");
            }

            Maxscript.Command("tex = BitmapTexture()");
            Maxscript.Command("tex.name = \"{0}\"", mat.DiffuseMap);
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
                if (mat.Flags.HasFlag(BrgMatFlag.PixelXForm1))
                {
                    Maxscript.Command("pcCompTex = CompositeTextureMap()");

                    Maxscript.Command("pcTex = BitmapTexture()");
                    Maxscript.Command("pcTex.name = \"{0}\"", mat.DiffuseMap);
                    Maxscript.Command("pcTex.filename = \"{0}\"", mat.DiffuseMap + ".tga");

                    Maxscript.Command("pcTex2 = BitmapTexture()");
                    Maxscript.Command("pcTex2.name = \"{0}\"", mat.DiffuseMap);
                    Maxscript.Command("pcTex2.filename = \"{0}\"", mat.DiffuseMap + ".tga");
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
                    //MaxHelper.Command("print {0}", name);
                    Maxscript.Command("tex.filename = \"{0}\"", mat.DiffuseMap + ".tga");
                    Maxscript.Command("mat.diffusemap = tex");
                }
            }

            return "mat";
        }

        private void ImportBrgFromMax()
        {
            BrgFile brg = file as BrgFile;
            string mainObject = "mainObject";
            Maxscript.Command("{0} = selection[1]", mainObject);
            bool objectSelected = Maxscript.QueryBoolean("classOf {0} == Editable_mesh", mainObject);
            if (!objectSelected)
            {
                throw new Exception("No object selected!");
            }

            // Call GetKeys function from max
            Maxscript.Command("GetModelAnimKeys()");

            brg.Header.NumMeshes = Maxscript.QueryInteger("keys.Count");
            brg.Animation = new Animation();
            for (int i = 1; i <= brg.Header.NumMeshes; ++i)
            {
                brg.Animation.MeshChannel.MeshTimes.Add(Maxscript.QueryFloat("keys[{0}]", i));
            }

            if (Maxscript.QueryBoolean("{0}.modifiers[#edit_normals] == undefined", mainObject))
            {
                Maxscript.Command("addModifier {0} (Edit_Normals())", mainObject);
            }
            Maxscript.Command("modPanel.setCurrentObject {0}.modifiers[#edit_normals] ui:true", mainObject);

            brg.Header.NumMaterials = Maxscript.QueryInteger("{0}.material.materialList.count", mainObject);
            //System.Windows.Forms.MessageBox.Show(Header.numMeshes + " " + Header.numMaterials);
            if (brg.Header.NumMaterials > 0)
            {
                brg.Materials = new List<BrgMaterial>(brg.Header.NumMaterials);
                for (int i = 0; i < brg.Header.NumMaterials; i++)
                {
                    brg.Materials.Add(new BrgMaterial(brg));
                    this.ImportBrgMaterialFromMax(mainObject, i);
                }
            }

            brg.Meshes = new List<BrgMesh>(brg.Header.NumMeshes);
            for (int i = 0; i < brg.Header.NumMeshes; i++)
            {
                if (i > 0)
                {
                    brg.Meshes[0].MeshAnimations.Add(new BrgMesh(brg));
                    brg.UpdateMeshSettings(i, this.Flags, this.Format, this.AnimationType, this.InterpolationType, this.ExportedScaleFactor);
                    ImportBrgMeshFromMax(mainObject, (BrgMesh)brg.Meshes[0].MeshAnimations[i - 1], brg.Animation.MeshChannel.MeshTimes[i]);
                }
                else
                {
                    brg.Meshes.Add(new BrgMesh(brg));
                    brg.UpdateMeshSettings(i, this.Flags, this.Format, this.AnimationType, this.InterpolationType, this.ExportedScaleFactor);
                    ImportBrgMeshFromMax(mainObject, brg.Meshes[i], brg.Animation.MeshChannel.MeshTimes[i]);
                }
            }

            brg.Animation.Duration = brg.Meshes[0].ExtendedHeader.AnimationLength;
            brg.Animation.TimeStep = brg.Meshes[0].ExtendedHeader.AnimationLength / (float)(brg.Meshes[0].MeshAnimations.Count + 1);
            if (brg.Meshes[0].Header.AnimationType.HasFlag(BrgMeshAnimType.NONUNIFORM))
            {
                brg.Meshes[0].NonUniformKeys = new float[brg.Meshes[0].MeshAnimations.Count + 1];
                for (int i = 0; i <= brg.Meshes[0].MeshAnimations.Count; i++)
                {
                    brg.Meshes[0].NonUniformKeys[i] = brg.Animation.MeshChannel.MeshTimes[i] / brg.Animation.Duration;
                }
            }
        }
        private void ImportBrgMeshFromMax(string mainObject, BrgMesh mesh, float time)
        {
            BrgFile brg = file as BrgFile;

            mesh.Header.CenterPosition = new Vector3D
            (
                -Maxscript.QueryFloat("{0}.center.x", mainObject),
                Maxscript.QueryFloat("{0}.center.z", mainObject),
                -Maxscript.QueryFloat("{0}.center.y", mainObject)
            );

            Maxscript.Command("grnd = getNodeByName \"ground\"");
            if (!Maxscript.QueryBoolean("grnd == undefined"))
            {
                mesh.Header.HotspotPosition = new Vector3D
                (
                    -Maxscript.QueryFloat("grnd.position.x"),
                    Maxscript.QueryFloat("grnd.position.z"),
                    -Maxscript.QueryFloat("grnd.position.y")
                );
            }

            Maxscript.Command("{0}BBMax = {0}.max", mainObject);
            Maxscript.Command("{0}BBMin = {0}.min", mainObject);
            Vector3<float> bBoxMax = new Vector3<float>(Maxscript.QueryFloat("{0}BBMax.X", mainObject), Maxscript.QueryFloat("{0}BBMax.Y", mainObject), Maxscript.QueryFloat("{0}BBMax.Z", mainObject));
            Vector3<float> bBoxMin = new Vector3<float>(Maxscript.QueryFloat("{0}BBMin.X", mainObject), Maxscript.QueryFloat("{0}BBMin.Y", mainObject), Maxscript.QueryFloat("{0}BBMin.Z", mainObject));
            Vector3<float> bBox = (bBoxMax - bBoxMin) / 2;
            mesh.Header.MinimumExtent = new Vector3D(-bBox.X, -bBox.Z, -bBox.Y);
            mesh.Header.MaximumExtent = new Vector3D(bBox.X, bBox.Z, bBox.Y);

            string mainMesh = "mainMesh";
            string attachDummy = Maxscript.NewArray("attachDummy");
            Maxscript.SetVarAtTime(time, attachDummy, "$helpers/atpt??* as array");
            int numAttachpoints = Maxscript.QueryInteger("{0}.count", attachDummy);

            // Figure out the proper data to import
            Maxscript.Command("GetExportData {0}", time);
            int numVertices = Maxscript.QueryInteger("{0}.numverts", mainMesh);
            int numFaces = Maxscript.QueryInteger("{0}.numfaces", mainMesh);

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
                    throw new Exception("Import Verts/Normals " + i.ToString(), ex);
                }
            }

            //System.Windows.Forms.MessageBox.Show("2");
            if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) || mesh.Header.Flags.HasFlag(BrgMeshFlag.ANIMTEXCOORDS))
            {
                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.TEXCOORDSA))
                {
                    List<Vector3D> texVerticesList = new List<Vector3D>(numVertices);
                    for (int i = 0; i < numVertices; i++)
                    {
                        Maxscript.Command("tVert = getTVert {0} {1}", mainMesh, i + 1);
                        texVerticesList.Add(new Vector3D(Maxscript.QueryFloat("tVert.x"), Maxscript.QueryFloat("tVert.y"), 0f));
                    }
                    mesh.TextureCoordinates = texVerticesList;
                }
            }

            //System.Windows.Forms.MessageBox.Show("3");
            HashSet<int> diffFaceMats = new HashSet<int>();
            if (!mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH))
            {
                mesh.Faces = new List<Face>(numFaces);
                for (int i = 0; i < numFaces; ++i)
                {
                    mesh.Faces.Add(new Face());
                }

                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                {
                    for (int i = 0; i < numFaces; i++)
                    {
                        mesh.Faces[i].MaterialIndex = (Int16)Maxscript.QueryInteger("getFaceMatID {0} {1}", mainMesh, i + 1);
                        diffFaceMats.Add(mesh.Faces[i].MaterialIndex);
                    }
                }

                //System.Windows.Forms.MessageBox.Show("3.1");
                for (int i = 0; i < mesh.Faces.Count; i++)
                {
                    Maxscript.Command("face = getFace {0} {1}", mainMesh, i + 1);
                    mesh.Faces[i].Indices.Add((Int16)(Maxscript.QueryInteger("face.x") - 1));
                    mesh.Faces[i].Indices.Add((Int16)(Maxscript.QueryInteger("face.y") - 1));
                    mesh.Faces[i].Indices.Add((Int16)(Maxscript.QueryInteger("face.z") - 1));
                }

                //System.Windows.Forms.MessageBox.Show("3.2");
                if (mesh.Header.Flags.HasFlag(BrgMeshFlag.MATERIAL))
                {
                    mesh.VertexMaterials = new Int16[mesh.Vertices.Count];
                    for (int i = 0; i < mesh.Faces.Count; i++)
                    {
                        mesh.VertexMaterials[mesh.Faces[i].Indices[0]] = mesh.Faces[i].MaterialIndex;
                        mesh.VertexMaterials[mesh.Faces[i].Indices[1]] = mesh.Faces[i].MaterialIndex;
                        mesh.VertexMaterials[mesh.Faces[i].Indices[2]] = mesh.Faces[i].MaterialIndex;
                    }
                }
            }

            //System.Windows.Forms.MessageBox.Show("4");
            if (mesh.Header.Flags.HasFlag(BrgMeshFlag.SECONDARYMESH) && diffFaceMats.Count > 0)
            {
                mesh.ExtendedHeader.NumMaterials = (byte)(diffFaceMats.Count - 1);
                mesh.ExtendedHeader.NumUniqueMaterials = diffFaceMats.Count;
            }
            mesh.ExtendedHeader.AnimationLength = Maxscript.QueryFloat("keys[keys.Count]");//Maxscript.QueryFloat("animationRange.end.ticks / 4800 as float");

            //System.Windows.Forms.MessageBox.Show("5 " + numAttachpoints);
            if (mesh.Header.Flags.HasFlag(BrgMeshFlag.ATTACHPOINTS))
            {
                mesh.Attachpoints = new BrgMesh.BrgAttachpointCollection();
                for (int i = 0; i < numAttachpoints; i++)
                {
                    int index = Convert.ToInt32((Maxscript.QueryString("{0}[{1}].name", attachDummy, i + 1)).Substring(4, 2));
                    index = mesh.Attachpoints.Add(index);
                    //System.Windows.Forms.MessageBox.Show("5.1");
                    mesh.Attachpoints[index].NameId = BrgAttachpoint.GetIdByName((Maxscript.QueryString("{0}[{1}].name", attachDummy, i + 1)).Substring(7));
                    Maxscript.Command("{0}[{1}].name = \"{2}\"", attachDummy, i + 1, mesh.Attachpoints[index].GetMaxName());
                    //System.Windows.Forms.MessageBox.Show("5.2");
                    Maxscript.SetVarAtTime(time, "{0}Transform", "{0}[{1}].rotation as matrix3", attachDummy, i + 1);
                    Maxscript.SetVarAtTime(time, "{0}Position", "{0}[{1}].position", attachDummy, i + 1);
                    Maxscript.SetVarAtTime(time, "{0}Scale", "{0}[{1}].scale", attachDummy, i + 1);
                    //System.Windows.Forms.MessageBox.Show("5.3");
                    Vector3<float> scale = new Vector3<float>(Maxscript.QueryFloat("{0}Scale.X", attachDummy), Maxscript.QueryFloat("{0}Scale.Y", attachDummy), Maxscript.QueryFloat("{0}Scale.Z", attachDummy));
                    bBox = scale / 2;
                    //System.Windows.Forms.MessageBox.Show("5.4");

                    mesh.Attachpoints[index].XVector.X = -Maxscript.QueryFloat("{0}Transform[1].z", attachDummy);
                    mesh.Attachpoints[index].XVector.Y = Maxscript.QueryFloat("{0}Transform[3].z", attachDummy);
                    mesh.Attachpoints[index].XVector.Z = -Maxscript.QueryFloat("{0}Transform[2].z", attachDummy);

                    mesh.Attachpoints[index].YVector.X = -Maxscript.QueryFloat("{0}Transform[1].y", attachDummy);
                    mesh.Attachpoints[index].YVector.Y = Maxscript.QueryFloat("{0}Transform[3].y", attachDummy);
                    mesh.Attachpoints[index].YVector.Z = -Maxscript.QueryFloat("{0}Transform[2].y", attachDummy);

                    mesh.Attachpoints[index].ZVector.X = -Maxscript.QueryFloat("{0}Transform[1].x", attachDummy);
                    mesh.Attachpoints[index].ZVector.Y = Maxscript.QueryFloat("{0}Transform[3].x", attachDummy);
                    mesh.Attachpoints[index].ZVector.Z = -Maxscript.QueryFloat("{0}Transform[2].x", attachDummy);

                    mesh.Attachpoints[index].Position.X = -Maxscript.QueryFloat("{0}Position.x", attachDummy);
                    mesh.Attachpoints[index].Position.Z = -Maxscript.QueryFloat("{0}Position.y", attachDummy);
                    mesh.Attachpoints[index].Position.Y = Maxscript.QueryFloat("{0}Position.z", attachDummy);
                    //System.Windows.Forms.MessageBox.Show("5.5");

                    mesh.Attachpoints[index].BoundingBoxMin.X = -bBox.X;
                    mesh.Attachpoints[index].BoundingBoxMin.Z = -bBox.Y;
                    mesh.Attachpoints[index].BoundingBoxMin.Y = -bBox.Z;
                    mesh.Attachpoints[index].BoundingBoxMax.X = bBox.X;
                    mesh.Attachpoints[index].BoundingBoxMax.Z = bBox.Y;
                    mesh.Attachpoints[index].BoundingBoxMax.Y = bBox.Z;
                }
                //System.Windows.Forms.MessageBox.Show("# Atpts: " + Attachpoint.Count);
            }
        }
        private void ImportBrgMaterialFromMax(string mainObject, int materialIndex)
        {
            BrgFile brg = file as BrgFile;
            BrgMaterial mat = brg.Materials[materialIndex];
            mat.id = Maxscript.QueryInteger("{0}.material.materialIDList[{1}]", mainObject, materialIndex + 1);
            Maxscript.Command("mat = {0}.material[{1}]", mainObject, mat.id);

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
            Opacity = Maxscript.QueryFloat("mat.opacity") / 100f;
            mat.SpecularExponent = Maxscript.QueryFloat("mat.specularLevel");
            int opacityType = Maxscript.QueryInteger("mat.opacityType");
            if (mat.SpecularExponent > 0)
            {
                mat.Flags |= BrgMatFlag.SpecularExponent;
            }
            if (Opacity < 1f)
            {
                mat.Flags |= BrgMatFlag.Alpha;
            }
            if (opacityType == 1)
            {
                mat.Flags |= BrgMatFlag.SubtractiveBlend;
            }
            else if (opacityType == 2)
            {
                mat.Flags |= BrgMatFlag.AdditiveBlend;
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
                }
            }
            else if (Maxscript.QueryBoolean("(classof mat.diffusemap) == CompositeTextureMap") && Maxscript.QueryBoolean("(classof mat.diffusemap.mapList[1]) == BitmapTexture"))
            {
                mat.Flags |= BrgMatFlag.WrapUTx1 | BrgMatFlag.WrapVTx1 | BrgMatFlag.PixelXForm1;
                mat.DiffuseMap = Maxscript.QueryString("getFilenameFile(mat.diffusemap.mapList[1].filename)");
            }
        }
        #endregion
    }
}
