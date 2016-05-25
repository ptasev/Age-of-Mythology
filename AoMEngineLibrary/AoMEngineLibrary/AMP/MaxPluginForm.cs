#define DEBUGBOX // allows the use of messageboxes in debug mode

namespace AoMEngineLibrary.AMP
{
    using AoMEngineLibrary.Extensions;
    using AoMEngineLibrary.Graphics;
    using AoMEngineLibrary.Graphics.Brg;
    using AoMEngineLibrary.Graphics.Grn;
    using BrightIdeasSoftware;
    using ManagedServices;
    using MaxCustomControls;
    using MiscUtil;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;

    public partial class MaxPluginForm : MaxForm
    {
        // 7.2 -- Fix getting all the animation keys in brg, Fix normal export with skinned mesh in brg, Fix bug with grn skin not exporting on the first run of AMP
        public static string PluginTitle = "AMP 7.1";
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

        public static CuiUpdater uiUp;
        IModelMaxUI model;
        BrgMax brg;
        GrnMax grn;

        public MaxPluginForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.MaximizeBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            openFileDialog.Filter = "brg files|*.brg|grn files|*.grn";
            saveFileDialog.Filter = "brg files|*.brg|grn files|*.grn";
            saveFileDialog.AddExtension = false;

            // Update Colors
            uiUp = CuiUpdater.GetInstance();
            mainMenuStrip.Renderer = new ToolStripMaxPluginRenderer();
            mainStatusStrip.SizingGrip = false;
            mainStatusStrip.BackColor = uiUp.GetControlColor();
            mainStatusStrip.ForeColor = uiUp.GetTextColor();

            for (int i = 0; i < mainTabControl.TabPages.Count; i++)
            {
                mainTabControl.TabPages[i].BackColor = uiUp.GetControlColor();
                mainTabControl.TabPages[i].ForeColor = uiUp.GetTextColor();
            }

            // Brg Tab
            brgDataSplitContainer.Panel2MinSize = 350;
            brgObjectsGroupBox.ForeColor = uiUp.GetTextColor();
            brgImportGroupBox.ForeColor = uiUp.GetTextColor();
            brgMeshInterpTypeGroupBox.ForeColor = uiUp.GetTextColor();
            attachpointGroupBox.ForeColor = uiUp.GetTextColor();
            brgMeshFlagsGroupBox.ForeColor = uiUp.GetTextColor();
            brgMeshFormatGroupBox.ForeColor = uiUp.GetTextColor();
            brgMeshAnimTypeGroupBox.ForeColor = uiUp.GetTextColor();
            brgOptionsGroupBox.ForeColor = uiUp.GetTextColor();
            brgExportGroupBox.ForeColor = uiUp.GetTextColor();

            brgMeshFlagsCheckedListBox.ItemCheck += brgMeshFlagsCheckedListBox_ItemCheck;
            brgMeshFlagsCheckedListBox.BackColor = uiUp.GetEditControlColor();
            brgMeshFlagsCheckedListBox.ForeColor = uiUp.GetTextColor();
            brgMeshFlagsCheckedListBox.BorderStyle = BorderStyle.FixedSingle;
            brgMeshFlagsCheckedListBox.MultiColumn = true;
            brgMeshFlagsCheckedListBox.CheckOnClick = true;
            brgMeshFlagsCheckedListBox.ColumnWidth = 150;
            brgMeshFlagsCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMeshFlag));
            brgMeshFormatCheckedListBox.ItemCheck += brgMeshFormatCheckedListBox_ItemCheck;
            brgMeshFormatCheckedListBox.BackColor = uiUp.GetEditControlColor();
            brgMeshFormatCheckedListBox.ForeColor = uiUp.GetTextColor();
            brgMeshFormatCheckedListBox.BorderStyle = BorderStyle.FixedSingle;
            brgMeshFormatCheckedListBox.MultiColumn = true;
            brgMeshFormatCheckedListBox.CheckOnClick = true;
            brgMeshFormatCheckedListBox.ColumnWidth = 150;
            brgMeshFormatCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMeshFormat));

            keyframeRadioButton.CheckedChanged += brgMeshAnimTypeRadioButton_CheckedChanged;
            nonuniRadioButton.CheckedChanged += brgMeshAnimTypeRadioButton_CheckedChanged;
            skinBoneRadioButton.CheckedChanged += brgMeshAnimTypeRadioButton_CheckedChanged;
            interpolationTypeCheckBox.CheckStateChanged += brgMeshInterpolationTypeCheckBox_CheckStateChanged;

            // Tree List View Style
            HeaderFormatStyle treelistviewstyle = new HeaderFormatStyle();
            treelistviewstyle.SetBackColor(uiUp.GetControlColor());
            treelistviewstyle.SetForeColor(uiUp.GetTextColor());
            treelistviewstyle.Normal.FrameColor = uiUp.GetButtonLightShadow();
            treelistviewstyle.Normal.FrameWidth = 1;
            treelistviewstyle.Hot.BackColor = uiUp.GetEditControlColor();
            treelistviewstyle.Hot.FrameColor = uiUp.GetButtonLightShadow();
            treelistviewstyle.Hot.FrameWidth = 1;

            // Brg Objects View
            this.brgObjectsTreeListView.MouseEnter += TreeListView_MouseEnter;
            this.brgObjectsTreeListView.SelectedIndexChanged += brgObjectsTreeListView_SelectionChanged;
            this.brgObjectsTreeListView.OwnerDraw = true;
            this.brgObjectsTreeListView.RowHeight = 10;
            this.brgObjectsTreeListView.BorderStyle = BorderStyle.FixedSingle;
            this.brgObjectsTreeListView.OverlayText.BorderColor = uiUp.GetButtonDarkShadow();
            this.brgObjectsTreeListView.OverlayText.BorderWidth = 2;
            this.brgObjectsTreeListView.BackColor = uiUp.GetEditControlColor();
            this.brgObjectsTreeListView.ForeColor = uiUp.GetTextColor();
            this.brgObjectsTreeListView.HeaderFormatStyle = treelistviewstyle;
            this.brgObjectsTreeListView.FullRowSelect = true;
            this.brgObjectsTreeListView.HideSelection = false;
            this.brgObjectsTreeListView.CanExpandGetter = delegate(object rowObject)
            {
                if (rowObject is BrgMesh)
                {
                    return ((BrgMesh)rowObject).MeshAnimations.Count > 0;
                }

                return false;
            };
            this.brgObjectsTreeListView.ChildrenGetter = delegate(object rowObject)
            {
                if (rowObject is BrgMesh)
                {
                    return ((BrgMesh)rowObject).MeshAnimations;
                }

                return null;
            };
            OLVColumn nameCol = new OLVColumn("Name", "Name");
            nameCol.Width = 675;
            this.brgObjectsTreeListView.Columns.Add(nameCol);


            // Attachpoints
            attachpointListBox.MouseDoubleClick += attachpointListBox_MouseDoubleClick;
            attachpointListBox.BackColor = uiUp.GetEditControlColor();
            attachpointListBox.ForeColor = uiUp.GetTextColor();
            string[] atpts = new string[55];
            Array.Copy(BrgAttachpoint.AttachpointNames, atpts, 55);
            Array.Sort(atpts);
            attachpointListBox.DataSource = atpts;

            // Materials
            extractMatButton.FlatStyle = FlatStyle.Flat;
            extractMatButton.BackColor = uiUp.GetEditControlColor();
            extractMatButton.ForeColor = uiUp.GetTextColor();
            extractMatButton2.FlatStyle = FlatStyle.Flat;
            extractMatButton2.BackColor = uiUp.GetEditControlColor();
            extractMatButton2.ForeColor = uiUp.GetTextColor();
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
            bumpMapMaxTextBox.BackColor = uiUp.GetEditControlColor();
            bumpMapMaxTextBox.ForeColor = uiUp.GetTextColor();
            opacityMaxTextBox.BackColor = uiUp.GetEditControlColor();
            opacityMaxTextBox.ForeColor = uiUp.GetTextColor();
            specularLevelMaxTextBox.BackColor = uiUp.GetEditControlColor();
            specularLevelMaxTextBox.ForeColor = uiUp.GetTextColor();
            materialFlagsGroupBox.BackColor = uiUp.GetControlColor();
            materialFlagsGroupBox.ForeColor = uiUp.GetTextColor();
            materialFlagsCheckedListBox.ItemCheck += materialFlagsCheckedListBox_ItemCheck;
            materialFlagsCheckedListBox.BackColor = uiUp.GetEditControlColor();
            materialFlagsCheckedListBox.ForeColor = uiUp.GetTextColor();
            materialFlagsCheckedListBox.BorderStyle = BorderStyle.FixedSingle;
            materialFlagsCheckedListBox.MultiColumn = true;
            materialFlagsCheckedListBox.CheckOnClick = true;
            materialFlagsCheckedListBox.ColumnWidth = 150;
            materialFlagsCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMatFlag));

            // Grn Tab
            grnDataSplitContainer.Panel2MinSize = 350;
            grnExportGroupBox.ForeColor = uiUp.GetTextColor();
            grnObjectsGroupBox.ForeColor = uiUp.GetTextColor();
            grnPropsGroupBox.ForeColor = uiUp.GetTextColor();

            grnPropsListBox.BackColor = uiUp.GetEditControlColor();
            grnPropsListBox.ForeColor = uiUp.GetTextColor();
            grnPropsListBox.BorderStyle = BorderStyle.None;

            // Grn Tree View
            this.grnObjectsTreeListView.MouseEnter += TreeListView_MouseEnter;
            this.grnObjectsTreeListView.SelectedIndexChanged += grnObjectsTreeListView_SelectedIndexChanged;
            this.grnObjectsTreeListView.OwnerDraw = true;
            this.grnObjectsTreeListView.RowHeight = 10;
            this.grnObjectsTreeListView.BorderStyle = BorderStyle.FixedSingle;
            this.grnObjectsTreeListView.OverlayText.BorderColor = uiUp.GetButtonDarkShadow();
            this.grnObjectsTreeListView.OverlayText.BorderWidth = 2;
            this.grnObjectsTreeListView.BackColor = uiUp.GetEditControlColor();
            this.grnObjectsTreeListView.ForeColor = uiUp.GetTextColor();
            this.grnObjectsTreeListView.HeaderFormatStyle = treelistviewstyle;
            this.grnObjectsTreeListView.FullRowSelect = true;
            this.grnObjectsTreeListView.HideSelection = false;
            this.grnObjectsTreeListView.CanExpandGetter = delegate(object rowObject)
            {
                if (rowObject is GrnBone)
                {
                    int rowIndex = grn.File.Bones.IndexOf((GrnBone)rowObject);
                    return grn.File.Bones.Exists(x => x.ParentIndex == rowIndex);
                }
                else if (rowObject is GrnMaterial)
                {
                    return ((GrnMaterial)rowObject).DiffuseTexture != null;
                }

                return false;
            };
            this.grnObjectsTreeListView.ChildrenGetter = delegate(object rowObject)
            {
                if (rowObject is GrnBone)
                {
                    int rowIndex = grn.File.Bones.IndexOf((GrnBone)rowObject);
                    List<GrnBone> bones = grn.File.Bones.FindAll(x => x.ParentIndex == rowIndex);
                    bones.Remove((GrnBone)rowObject);
                    return bones;
                }
                else if (rowObject is GrnMaterial)
                {
                    return new object[] { ((GrnMaterial)rowObject).DiffuseTexture };
                }

                return null;
            };
            nameCol = new OLVColumn("Name", "Name");
            nameCol.Width = 675;
            this.grnObjectsTreeListView.Columns.Add(nameCol);
        }
        private void MaxPluginForm_Load(object sender, EventArgs e)
        {
#if !DEBUG
            HideDebugUI();
#endif
            Settings.Read();
            brg = new BrgMax(this);
            grn = new GrnMax(this);
            brg.LoadUI();
            grn.LoadUI();
            model = brg;
        }
        private void HideDebugUI()
        {
            Maxscript.OutputCommands = false;
            this.grnTestToolStripMenuItem.Visible = false;
            this.richTextBox1.Visible = false;
            this.grnMainTableLayoutPanel.SetRowSpan(this.grnPropsGroupBox, 2);
        }

        private void TreeListView_MouseEnter(object sender, EventArgs e)
        {
            ((TreeListView)sender).Focus();
        }

        #region UiColors
        public Color ContrastColor(Color color)
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

        private class ToolStripMaxPluginRenderer : MaxToolStripSystemRenderer//ToolStripProfessionalRenderer
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
            model.Clear();
            model.LoadUI();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Settings.OpenFileDialogFileName))
            {
                openFileDialog.InitialDirectory = Settings.OpenFileDialogFileName;
            }
            else if (!string.IsNullOrEmpty(model.FileName))
            {
                openFileDialog.InitialDirectory = Path.GetDirectoryName(model.FileName);
            }
            openFileDialog.FileName = Path.GetFileNameWithoutExtension(model.FileName);
            openFileDialog.FilterIndex = model.FilterIndex;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    model.Read(File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
                    model.LoadUI();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to open file!" + Environment.NewLine + Environment.NewLine + ex.Message, MaxPluginForm.PluginTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void debug()
        {
            using (TextWriter writer = File.CreateText(@"C:\Users\Petar\Desktop\lp skult.grn.txt.ms"))//Path.GetFileName(file.FileName) + ".txt"))
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
            if (!string.IsNullOrEmpty(Settings.SaveFileDialogFileName))
            {
                saveFileDialog.InitialDirectory = Settings.SaveFileDialogFileName;
            }
            else if (!string.IsNullOrEmpty(model.FileName))
            {
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(model.FileName);
            }
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(model.FileName);
            saveFileDialog.FilterIndex = model.FilterIndex;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    model.SaveUI();
                    model.Write(File.Open(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read));
                    model.LoadUI();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save file!" + Environment.NewLine + Environment.NewLine + ex.Message, MaxPluginForm.PluginTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dlgR = MessageBox.Show("Do you want to clear the scene?", MaxPluginForm.PluginTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
            if (dlgR == DialogResult.Yes)
            {
                Maxscript.Command("resetMaxFile #noprompt");
                //Maxscript.Command("if checkForSave() do resetMaxFile #noprompt");
            }
            else if (dlgR == DialogResult.Cancel)
            {
                return;
            }

            //ProgressDialog ProgDialog = new ProgressDialog();
            //Thread importThread = new Thread(model.Import);
            //importThread.IsBackground = true;
            //importThread.Start();
            //ProgDialog.Show(this);
            //importThread.Join();

            try
            {
                this.Enabled = false;
                model.SaveUI();
                model.Import();
                model.LoadUI();
                debug();
                Maxscript.Output.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to import model!" + Environment.NewLine + Environment.NewLine + ex.Message, MaxPluginForm.PluginTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Enabled = true;
                //ProgDialog.Close();
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                model.SaveUI();
                model.Export();
                model.LoadUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to export model!" + Environment.NewLine + Environment.NewLine + ex.Message, MaxPluginForm.PluginTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Enabled = true;
            }
        }

        private void readMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.petartasev.com/modding/age-of-mythology/model-plugin/");
        }

        private void beginnersGuideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://aom.heavengames.com/cgi-bin/forums/display.cgi?action=ct&f=19,29360,0,365");
        }

        private void brgSettingsInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.google.com/spreadsheets/d/1m6AZHjt3YU4fxH_-w9Smi1WEBa651PXgavQrC_a1_1o/edit?usp=sharing");
        }

        private void sourceCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Ryder25/Age-of-Mythology");
        }
        #endregion

        #region Brg
        private void attachpointListBox_MouseEnter(object sender, EventArgs e)
        {
            attachpointListBox.Focus();
        }
        private void attachpointListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = attachpointListBox.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                BrgAttachpoint att = new BrgAttachpoint();
                att.NameId = BrgAttachpoint.GetIdByName((string)attachpointListBox.Items[index]);
                Maxscript.NewDummy("newDummy", att.GetMaxName(), att.GetMaxTransform(), att.GetMaxPosition(), att.GetMaxBoxSize(), att.GetMaxScale());
            }
        }

        private void materialFlagsCheckedListBox_MouseEnter(object sender, EventArgs e)
        {
            materialFlagsCheckedListBox.Focus();
        }
        private void materialFlagsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.brgObjectsTreeListView.SelectedObject == null ||
                !(this.brgObjectsTreeListView.SelectedObject is BrgMaterial))
            {
                return;
            }

            BrgMaterial mat = (BrgMaterial)this.brgObjectsTreeListView.SelectedObject;
            mat.Flags = this.materialFlagsCheckedListBox.GetEnum<BrgMatFlag>();
            if (e.NewValue == CheckState.Checked)
            {
                mat.Flags |= (BrgMatFlag)this.materialFlagsCheckedListBox.Items[e.Index];
            }
            else
            {
                mat.Flags &= ~(BrgMatFlag)this.materialFlagsCheckedListBox.Items[e.Index];
            }
        }
        private void extractMatButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Settings.MtrlFolderDialogDirectory))
            {
                folderBrowserDialog.SelectedPath = Settings.MtrlFolderDialogDirectory;
            }
            else if (!string.IsNullOrEmpty(brg.FileName))
            {
                folderBrowserDialog.SelectedPath = Path.GetDirectoryName(brg.FileName);
            }

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                brg.SaveUI();
                for (int i = 0; i < brg.File.Materials.Count; i++)
                {
                    MtrlFile mtrl = new MtrlFile(brg.File.Materials[i]);
                    mtrl.Write(File.Open(Path.Combine(folderBrowserDialog.SelectedPath, Path.GetFileNameWithoutExtension(brg.FileName) + "_" + i + ".mtrl"), FileMode.Create, FileAccess.Write, FileShare.Read));
                    //brg.File.Materials[i].WriteExternal(File.Open(Path.Combine(folderBrowserDialog.SelectedPath, Path.GetFileNameWithoutExtension(brg.FileName) + "_" + i + ".mtrl"), FileMode.Create, FileAccess.Write, FileShare.Read));
                }
                brg.LoadUI();
            }
        }

        private void brgObjectsTreeListView_SelectionChanged(object sender, EventArgs e)
        {
            if (this.brgObjectsTreeListView.SelectedObject == null)
            {
                return;
            }

            if (this.brgObjectsTreeListView.SelectedObject is BrgAttachpoint)
            {
                Maxscript.Command("selectDummy = getNodeByName \"{0}\"", ((BrgAttachpoint)this.brgObjectsTreeListView.SelectedObject).GetMaxName());
                if (Maxscript.QueryBoolean("selectDummy != undefined"))
                {
                    Maxscript.Command("select selectDummy");
                }
                this.brgAttachpointTableLayoutPanel.BringToFront();
            }
            else if (this.brgObjectsTreeListView.SelectedObject is BrgMesh)
            {
                brgMeshFlagsCheckedListBox.ItemCheck -= brgMeshFlagsCheckedListBox_ItemCheck;
                brgMeshFormatCheckedListBox.ItemCheck -= brgMeshFormatCheckedListBox_ItemCheck;
                keyframeRadioButton.CheckedChanged -= brgMeshAnimTypeRadioButton_CheckedChanged;
                nonuniRadioButton.CheckedChanged -= brgMeshAnimTypeRadioButton_CheckedChanged;
                skinBoneRadioButton.CheckedChanged -= brgMeshAnimTypeRadioButton_CheckedChanged;
                interpolationTypeCheckBox.CheckStateChanged -= brgMeshInterpolationTypeCheckBox_CheckStateChanged;

                brg.LoadMeshUI();
                this.brgFlagsTableLayoutPanel.BringToFront();

                brgMeshFlagsCheckedListBox.ItemCheck += brgMeshFlagsCheckedListBox_ItemCheck;
                brgMeshFormatCheckedListBox.ItemCheck += brgMeshFormatCheckedListBox_ItemCheck;
                keyframeRadioButton.CheckedChanged += brgMeshAnimTypeRadioButton_CheckedChanged;
                nonuniRadioButton.CheckedChanged += brgMeshAnimTypeRadioButton_CheckedChanged;
                skinBoneRadioButton.CheckedChanged += brgMeshAnimTypeRadioButton_CheckedChanged;
                interpolationTypeCheckBox.CheckStateChanged += brgMeshInterpolationTypeCheckBox_CheckStateChanged;
            }
            else if (this.brgObjectsTreeListView.SelectedObject is BrgMaterial)
            {
                materialFlagsCheckedListBox.ItemCheck -= materialFlagsCheckedListBox_ItemCheck;

                brg.LoadMaterialUI();
                this.brgMaterialTableLayoutPanel.BringToFront();

                materialFlagsCheckedListBox.ItemCheck += materialFlagsCheckedListBox_ItemCheck;
            }
        }
        private void brgMeshFlagsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.brgObjectsTreeListView.SelectedObject == null ||
                !(this.brgObjectsTreeListView.SelectedObject is BrgMesh))
            {
                return;
            }

            brgMeshFlagsCheckedListBox.ItemCheck -= brgMeshFlagsCheckedListBox_ItemCheck;

            BrgMesh mesh = (BrgMesh)this.brgObjectsTreeListView.SelectedObject;
            mesh.Header.Flags = this.brgMeshFlagsCheckedListBox.GetEnum<BrgMeshFlag>();
            if (e.NewValue == CheckState.Checked)
            {
                mesh.Header.Flags |= (BrgMeshFlag)this.brgMeshFlagsCheckedListBox.Items[e.Index];
            }
            else
            {
                mesh.Header.Flags &= ~(BrgMeshFlag)this.brgMeshFlagsCheckedListBox.Items[e.Index];
            }

            brg.File.UpdateMeshSettings(mesh.Header.Flags, mesh.Header.Format, mesh.Header.AnimationType, mesh.Header.InterpolationType);
            this.brgMeshFlagsCheckedListBox.SetEnum<BrgMeshFlag>(mesh.Header.Flags);

            brgMeshFlagsCheckedListBox.ItemCheck += brgMeshFlagsCheckedListBox_ItemCheck;
        }
        private void brgMeshFormatCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.brgObjectsTreeListView.SelectedObject == null ||
                !(this.brgObjectsTreeListView.SelectedObject is BrgMesh))
            {
                return;
            }

            brgMeshFormatCheckedListBox.ItemCheck -= brgMeshFormatCheckedListBox_ItemCheck;

            BrgMesh mesh = (BrgMesh)this.brgObjectsTreeListView.SelectedObject;
            mesh.Header.Format = this.brgMeshFormatCheckedListBox.GetEnum<BrgMeshFormat>();
            if (e.NewValue == CheckState.Checked)
            {
                mesh.Header.Format |= (BrgMeshFormat)this.brgMeshFormatCheckedListBox.Items[e.Index];
            }
            else
            {
                mesh.Header.Format &= ~(BrgMeshFormat)this.brgMeshFormatCheckedListBox.Items[e.Index];
            }

            brg.File.UpdateMeshSettings(mesh.Header.Flags, mesh.Header.Format, mesh.Header.AnimationType, mesh.Header.InterpolationType);
            this.brgMeshFormatCheckedListBox.SetEnum<BrgMeshFormat>(mesh.Header.Format);

            brgMeshFormatCheckedListBox.ItemCheck += brgMeshFormatCheckedListBox_ItemCheck;
        }
        private void brgMeshAnimTypeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.brgObjectsTreeListView.SelectedObject == null ||
                !(this.brgObjectsTreeListView.SelectedObject is BrgMesh))
            {
                return;
            }

            keyframeRadioButton.CheckedChanged -= brgMeshAnimTypeRadioButton_CheckedChanged;
            nonuniRadioButton.CheckedChanged -= brgMeshAnimTypeRadioButton_CheckedChanged;
            skinBoneRadioButton.CheckedChanged -= brgMeshAnimTypeRadioButton_CheckedChanged;

            BrgMesh mesh = (BrgMesh)this.brgObjectsTreeListView.SelectedObject;

            if (this.keyframeRadioButton.Checked)
            {
                mesh.Header.AnimationType = BrgMeshAnimType.KeyFrame;
            }
            else if (this.nonuniRadioButton.Checked)
            {
                mesh.Header.AnimationType = BrgMeshAnimType.NonUniform;
            }
            else if (this.skinBoneRadioButton.Checked)
            {
                mesh.Header.AnimationType = BrgMeshAnimType.SkinBone;
            }

            brg.File.UpdateMeshSettings(mesh.Header.Flags, mesh.Header.Format, mesh.Header.AnimationType, mesh.Header.InterpolationType);

            if (mesh.Header.AnimationType == BrgMeshAnimType.KeyFrame)
            {
                this.keyframeRadioButton.Checked = true;
            }
            else if (mesh.Header.AnimationType == BrgMeshAnimType.NonUniform)
            {
                this.nonuniRadioButton.Checked = true;
            }
            else if (mesh.Header.AnimationType == BrgMeshAnimType.SkinBone)
            {
                this.skinBoneRadioButton.Checked = true;
            }

            keyframeRadioButton.CheckedChanged += brgMeshAnimTypeRadioButton_CheckedChanged;
            nonuniRadioButton.CheckedChanged += brgMeshAnimTypeRadioButton_CheckedChanged;
            skinBoneRadioButton.CheckedChanged += brgMeshAnimTypeRadioButton_CheckedChanged;
        }
        private void brgMeshInterpolationTypeCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.brgObjectsTreeListView.SelectedObject == null ||
                !(this.brgObjectsTreeListView.SelectedObject is BrgMesh))
            {
                return;
            }

            interpolationTypeCheckBox.CheckStateChanged -= brgMeshInterpolationTypeCheckBox_CheckStateChanged;

            BrgMesh mesh = (BrgMesh)this.brgObjectsTreeListView.SelectedObject;
            mesh.Header.InterpolationType = (BrgMeshInterpolationType)Convert.ToByte(this.interpolationTypeCheckBox.Checked);
            brg.File.UpdateMeshSettings(mesh.Header.Flags, mesh.Header.Format, mesh.Header.AnimationType, mesh.Header.InterpolationType);
            this.interpolationTypeCheckBox.Checked = Convert.ToBoolean(mesh.Header.InterpolationType);

            interpolationTypeCheckBox.CheckStateChanged += brgMeshInterpolationTypeCheckBox_CheckStateChanged;
        }
        #endregion

        #region Grn
        private void openGrnTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.FilterIndex = 2;
            if (!string.IsNullOrEmpty(Settings.OpenFileDialogFileName))
            {
                openFileDialog.InitialDirectory = Settings.OpenFileDialogFileName;
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //@"C:\Users\Petar\Desktop\Nieuwe map (3)\AoM Grn\lp skult.grn"
                    grn.Read(File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
                    grn.Import();
                    debug();
                    this.Text = "ABE - " + Path.GetFileName(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to open file!" + Environment.NewLine + Environment.NewLine + ex.Message, "ABE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void exportGrnTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grn.Export();

            try
            {
                DialogResult dlgR = MessageBox.Show("Do you want to clear the scene?", "ABE", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
                if (dlgR == DialogResult.Yes)
                {
                    Maxscript.Command("resetMaxFile #noprompt");
                }
                else if (dlgR == DialogResult.Cancel)
                {
                    return;
                }
            }
            catch
            {
            }
            Maxscript.Output.Clear();
            debug();

            grn.Import();
        }

        private void saveGrnTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(grn.FileName))
            {
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(grn.FileName);
            }
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(grn.FileName);

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    grn.SaveUI();
                    grn.Write(File.Open(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read));
                    grn.LoadUI();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save file!" + Environment.NewLine + Environment.NewLine + ex.Message, "ABE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void grnObjectsTreeListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.grnObjectsTreeListView.SelectedObject == null)
            {
                return;
            }

            this.grnPropsListBox.Items.Clear();
            foreach (KeyValuePair<string, string> prop in grn.File.DataExtensions[((IGrnObject)this.grnObjectsTreeListView.SelectedObject).DataExtensionIndex])
            {
                this.grnPropsListBox.Items.Add(prop.Key + " -- " + prop.Value);
            }
        }
        #endregion

        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            model.SaveUI();

            if (mainTabControl.SelectedIndex == 0)
            {
                this.model = this.brg;
                this.brgObjectsTreeListView.Focus();
                //MessageBox.Show("brg");
            }
            else if (mainTabControl.SelectedIndex == 1)
            {
                this.model = this.grn;
                this.grnObjectsTreeListView.Focus();
                //MessageBox.Show("grn");
            }

            model.LoadUI();
        }

        [System.Diagnostics.Conditional("DEBUG")] // Don't allow calls to this func in release mode
        public static void DebugBox(string message)
        {
#if DEBUGBOX
            MessageBox.Show(message);
#endif
        }

        private void brgObjectsTreeListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                int itemCount = this.brgObjectsTreeListView.GetItemCount();
                int index = this.brgObjectsTreeListView.SelectedIndex;
                for (int i = 0; i < itemCount; ++i)
                {
                    index = ++index % itemCount;
                    if (this.brgObjectsTreeListView.GetModelObject(index) is BrgMaterial)
                    {
                        this.brgObjectsTreeListView.SelectedIndex = index;
                        e.Handled = true;
                        break;
                    }
                }
            }
        }
    }
}
