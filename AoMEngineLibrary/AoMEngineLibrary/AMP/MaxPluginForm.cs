#define DEBUGBOX // allows the use of messageboxes in debug mode

namespace AoMEngineLibrary.AMP
{
    using AoMEngineLibrary.Graphics;
    using AoMEngineLibrary.Graphics.Brg;
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
        // 7.0 -- Improved material export, Grn support, Multiple Mesh Export Support
        public static string PluginTitle = "AMP 7.0";
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
        IModelMaxUi model;
        BrgMax brg;
        GrnMax grn;

        public MaxPluginForm()
        {
            InitializeComponent();
            //this.mainTabControl.TabPages.Remove(this.grnSettingsTabPage);
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

            // General Tab
            generalDataGroupBox.ForeColor = uiUp.GetTextColor();
            brgImportGroupBox.ForeColor = uiUp.GetTextColor();
            interpTypeGroupBox.ForeColor = uiUp.GetTextColor();
            attachpointGroupBox.ForeColor = uiUp.GetTextColor();
            genMeshFlagsGroupBox.ForeColor = uiUp.GetTextColor();
            genMeshFormatGroupBox.ForeColor = uiUp.GetTextColor();
            genMeshAnimTypeGroupBox.ForeColor = uiUp.GetTextColor();

            genMeshFlagsCheckedListBox.BackColor = uiUp.GetEditControlColor();
            genMeshFlagsCheckedListBox.ForeColor = uiUp.GetTextColor();
            genMeshFlagsCheckedListBox.BorderStyle = BorderStyle.None;
            genMeshFlagsCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMeshFlag));
            genMeshFormatCheckedListBox.BackColor = uiUp.GetEditControlColor();
            genMeshFormatCheckedListBox.ForeColor = uiUp.GetTextColor();
            genMeshFormatCheckedListBox.BorderStyle = BorderStyle.None;
            genMeshFormatCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMeshFormat));

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
            string[] atpts = new string[55];
            Array.Copy(BrgAttachpoint.AttachpointNames, atpts, 55);
            Array.Sort(atpts);
            attachpointListBox.DataSource = atpts;

            // Materials
            extractMatButton.FlatStyle = FlatStyle.Flat;
            extractMatButton.BackColor = uiUp.GetEditControlColor();
            extractMatButton.ForeColor = uiUp.GetTextColor();
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
            materialSideGroupBox.BackColor = uiUp.GetControlColor();
            materialSideGroupBox.ForeColor = uiUp.GetTextColor();
            materialFlagsGroupBox.BackColor = uiUp.GetControlColor();
            materialFlagsGroupBox.ForeColor = uiUp.GetTextColor();
            materialListBox.SelectedIndexChanged += materialListBox_SelectedIndexChanged;
            materialListBox.BackColor = uiUp.GetEditControlColor();
            materialListBox.ForeColor = uiUp.GetTextColor();
            materialFlagsCheckedListBox.BackColor = uiUp.GetEditControlColor();
            materialFlagsCheckedListBox.ForeColor = uiUp.GetTextColor();
            materialFlagsCheckedListBox.BorderStyle = BorderStyle.FixedSingle;
            materialFlagsCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMatFlag));

            // Grn Settings
            grnSettingsGroupBox.ForeColor = uiUp.GetTextColor();
            grnExportGroupBox.ForeColor = uiUp.GetTextColor();
            grnObjectsGroupBox.ForeColor = uiUp.GetTextColor();
            grnPropsGroupBox.ForeColor = uiUp.GetTextColor();

            grnObjectsListBox.SelectedIndexChanged += grnObjectsListBox_SelectedIndexChanged;
            grnObjectsListBox.BackColor = uiUp.GetEditControlColor();
            grnObjectsListBox.ForeColor = uiUp.GetTextColor();
            grnObjectsListBox.BorderStyle = BorderStyle.None;
            grnPropsListBox.BackColor = uiUp.GetEditControlColor();
            grnPropsListBox.ForeColor = uiUp.GetTextColor();
            grnPropsListBox.BorderStyle = BorderStyle.None;

            //plugin = new MaxPlugin();
            //this.Controls.Add(plugin);
            //plugin.Dock = DockStyle.Fill;
            Settings.Read();
            brg = new BrgMax(this);
            grn = new GrnMax(this);
            brg.LoadUi();
            grn.LoadUi();
            model = brg;
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
            model.LoadUi();
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
                    model.LoadUi();
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
            using (TextWriter writer = File.CreateText(@"C:\Users\Petar\Desktop\Nieuwe map (3)\AoM Grn\lp skult.grn.txt.ms"))//Path.GetFileName(file.FileName) + ".txt"))
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
                    model.SaveUi();
                    model.Write(File.Open(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read));
                    model.LoadUi();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save file!" + Environment.NewLine + Environment.NewLine + ex.Message, MaxPluginForm.PluginTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
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

            model.SaveUi();
            //ProgDialog = new ProgressDialog();
            //Thread importThread = new Thread(model.Import);
            //importThread.IsBackground = true;
            //importThread.Start();
            //ProgDialog.ShowDialog();
            //importThread.Join();
            model.Import();
            model.LoadUi();
            debug();
            Maxscript.Output.Clear();
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to export model!" + Environment.NewLine + Environment.NewLine + ex.Message, MaxPluginForm.PluginTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.SaveUi();
            model.Export();
            model.LoadUi();
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to import model!" + Environment.NewLine + Environment.NewLine + ex.Message, MaxPluginForm.PluginTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void readMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://petar.outer-heaven.net/downloads/aom/amp/ReadME.html");
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
        void attachpointListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = attachpointListBox.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                BrgAttachpoint att = new BrgAttachpoint();
                att.NameId = BrgAttachpoint.GetIdByName((string)attachpointListBox.Items[index]);
                Maxscript.NewDummy("newDummy", att.GetMaxName(), att.GetMaxTransform(), att.GetMaxPosition(), att.GetMaxBoxSize(), att.GetMaxScale());

                //if (brg.File.Meshes.Count == 0)
                //{
                //    brg.File.Meshes.Add(new BrgMesh(brg.File));
                //}
                //brg.File.Meshes[0].Attachpoints.Add(att);
                //brg.LoadUiAttachpoint();
                //attachpointComboBox.SelectedIndex = attachpointComboBox.Items.Count - 1;
            }
        }
        void attachpointComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (attachpointComboBox.SelectedItem != null && brg.File.Meshes.Count > 0)
            {
                Maxscript.Command("selectDummy = getNodeByName \"{0}\"", ((BrgAttachpoint)attachpointComboBox.SelectedItem).GetMaxName());
                if (Maxscript.QueryBoolean("selectDummy != undefined"))
                {
                    Maxscript.Command("select selectDummy");
                }
                //else
                //{
                //    DialogResult dlgR = MessageBox.Show("Could not find \"" + ((BrgAttachpoint)attachpointComboBox.SelectedItem).GetMaxName() + "\"! Would you like to delete it?", "ABE",
                //        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                //    if (dlgR == DialogResult.Yes)
                //    {
                //        brg.File.Meshes[0].Attachpoints.Remove(((BrgAttachpoint)attachpointComboBox.SelectedItem).Index);
                //        brg.LoadUiAttachpoint();
                //    }
                //}
            }
        }

        private void materialFlagsCheckedListBox_MouseEnter(object sender, EventArgs e)
        {
            materialFlagsCheckedListBox.Focus();
        }
        void materialListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialListBox.SelectedIndex >= 0)
            {
                brg.LoadUiMaterialData();
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
                brg.SaveUi();
                for (int i = 0; i < brg.File.Materials.Count; i++)
                {
                    MtrlFile mtrl = new MtrlFile(brg.File.Materials[i]);
                    mtrl.Write(File.Open(Path.Combine(folderBrowserDialog.SelectedPath, Path.GetFileNameWithoutExtension(brg.FileName) + "_" + i + ".mtrl"), FileMode.Create, FileAccess.Write, FileShare.Read));
                    //brg.File.Materials[i].WriteExternal(File.Open(Path.Combine(folderBrowserDialog.SelectedPath, Path.GetFileNameWithoutExtension(brg.FileName) + "_" + i + ".mtrl"), FileMode.Create, FileAccess.Write, FileShare.Read));
                }
                brg.LoadUi();
            }
        }

        public void SetCheckedListBoxSelectedEnums<T>(CheckedListBox box, uint enumVal)
        {
            for (int i = 0; i < box.Items.Count; i++)
            {
                uint prop = Operator.Convert<T, uint>(Operator.Convert<object, T>(box.Items[i]));
                if ((enumVal & prop) == prop)
                {
                    box.SetItemChecked(i, true);
                }
                else
                {
                    box.SetItemChecked(i, false);
                }
            }
        }
        public T GetCheckedListBoxSelectedEnums<T>(CheckedListBox box)
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
                    grn.SaveUi();
                    grn.Write(File.Open(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read));
                    grn.LoadUi();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save file!" + Environment.NewLine + Environment.NewLine + ex.Message, "ABE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void grnObjectsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (grnObjectsListBox.SelectedIndex >= 0)
            {
                grnPropsListBox.Items.Clear();
                foreach (KeyValuePair<string, string> prop in grn.File.DataExtensions[grnObjectsListBox.SelectedIndex])
                {
                    grnPropsListBox.Items.Add(prop.Key + " -- " + prop.Value);
                }
            }
        }
        #endregion

        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            model.SaveUi();

            if (mainTabControl.SelectedIndex == 0 ||
                mainTabControl.SelectedIndex == 1)
            {
                this.model = this.brg;
                //MessageBox.Show("brg");
            }
            else
            {
                this.model = this.grn;
                //MessageBox.Show("grn");
            }

            model.LoadUi();
        }

        [System.Diagnostics.Conditional("DEBUG")] // Don't allow calls to this func in release mode
        public static void DebugBox(string message)
        {
#if DEBUGBOX
            MessageBox.Show(message);
#endif
        }
    }
}
