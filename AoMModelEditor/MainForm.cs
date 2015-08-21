#define DEBUGBOX // allows the use of messageboxes in debug mode

namespace AoMModelEditor
{
    using AoMEngineLibrary.Graphics;
    using AoMEngineLibrary.Graphics.Brg;
    using AoMEngineLibrary.Graphics.Grn;
    using AoMEngineLibrary.Graphics.Model;
    using BrightIdeasSoftware;
    using MiscUtil;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;

    public partial class MainForm : Form
    {
        public static string PluginTitle = "AME 1.0";
        public static class Settings
        {
            private static string fileName = Application.StartupPath + "\\AoMModelEditorSettings.xml";
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

        IModelUI model;
        BrgUi brg;
        GrnUi grn;

        public MainForm()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Cyclops;

            //BrgFile f = new BrgFile(File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\cavalry g prodromos_attacka.brg", FileMode.Open, FileAccess.Read, FileShare.Read));
            //BrgFile f2 = new BrgFile(File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\cavalry g prodromos_attacka.brg", FileMode.Open, FileAccess.Read, FileShare.Read));
            //f2.Materials[0].id = 12212;
            //int eq = f.Materials.IndexOf(f2.Materials[0]);

            //foreach (string s in Directory.GetFiles(@"C:\Games\Steam\steamapps\common\Age of Mythology\mods\AoSW\models", "*.brg", SearchOption.AllDirectories))
            //{
            //    BrgFile file = new BrgFile(File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read));
            //    for (int i = 0; i < file.Materials.Count; i++)
            //    {
            //        MtrlFile mtrl = new MtrlFile(file.Materials[i]);
            //        mtrl.Write(File.Open(Path.Combine(@"C:\Games\Steam\steamapps\common\Age of Mythology\mods\AoSW\materials", Path.GetFileNameWithoutExtension(s) + "_" + i + ".mtrl"), FileMode.Create, FileAccess.Write, FileShare.Read));
            //    }
            //}

            // Brg Objects Viewer
            this.brgObjectListView.FormatCell += objectListView1_FormatCell;
            this.brgObjectListView.CellEditStarting += objectListView1_CellEditStarting;
            this.brgObjectListView.CellEditFinishing += objectListView1_CellEditFinishing;
            this.brgObjectListView.MouseEnter += ObjectListView_MouseEnter;
            this.brgObjectListView.CellEditActivation = ObjectListView.CellEditActivateMode.DoubleClick;
            this.brgObjectListView.ShowGroups = false;
            this.brgObjectListView.OwnerDraw = true;
            this.brgObjectListView.UseCellFormatEvents = true;

            // Brg Tree View
            this.brgObjectsTreeListView.MouseEnter += TreeListView_MouseEnter;
            brgObjectsTreeListView.FullRowSelect = true;
            brgObjectsTreeListView.HideSelection = false;
            brgObjectsTreeListView.CanExpandGetter = delegate(object rowObject)
            {
                if (rowObject is BrgMesh)
                {
                    return ((BrgMesh)rowObject).MeshAnimations.Count > 0;
                }

                return false;
            };
            brgObjectsTreeListView.ChildrenGetter = delegate(object rowObject)
            {
                if (rowObject is BrgMesh)
                {
                    return ((BrgMesh)rowObject).MeshAnimations;
                }

                return null;
            };
            OLVColumn nameCol = new OLVColumn("Name", "Name");
            nameCol.FillsFreeSpace = true;
            brgObjectsTreeListView.Columns.Add(nameCol);

            // Grn Objects Viewer
            //this.grnObjectListView.FormatCell += objectListView1_FormatCell;
            //this.grnObjectListView.CellEditStarting += objectListView1_CellEditStarting;
            //this.grnObjectListView.CellEditFinishing += objectListView1_CellEditFinishing;
            this.grnObjectListView.MouseEnter += ObjectListView_MouseEnter;
            this.grnObjectListView.CellEditActivation = ObjectListView.CellEditActivateMode.DoubleClick;
            this.grnObjectListView.ShowGroups = false;
            //this.grnObjectListView.OwnerDraw = true;
            //this.grnObjectListView.UseCellFormatEvents = true;

            // Grn Tree View
            this.grnObjectsTreeListView.SelectedIndexChanged += grnObjectsTreeListView_SelectedIndexChanged;
            this.grnObjectsTreeListView.MouseEnter += TreeListView_MouseEnter;
            grnObjectsTreeListView.FullRowSelect = true;
            grnObjectsTreeListView.HideSelection = false;
            grnObjectsTreeListView.CanExpandGetter = delegate(object rowObject)
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
            grnObjectsTreeListView.ChildrenGetter = delegate(object rowObject)
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
            nameCol.Width = 300;
            grnObjectsTreeListView.Columns.Add(nameCol);

            // Model Settings
            Settings.Read();
            brg = new BrgUi(this);
            grn = new GrnUi(this);
            brg.LoadUI();
            grn.LoadUI();
            model = brg;
        }

        private void ObjectListView_MouseEnter(object sender, EventArgs e)
        {
            ((ObjectListView)sender).Focus();
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
                    MessageBox.Show("Failed to open file!" + Environment.NewLine + Environment.NewLine + ex.Message, MainForm.PluginTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    model.SaveUI();
                    model.Write(File.Open(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read));
                    model.LoadUI();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save file!" + Environment.NewLine + Environment.NewLine + ex.Message, MainForm.PluginTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            model.SaveUI();
            model.Import();
            model.LoadUI();
            debug();
            Maxscript.Output.Clear();
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to export model!" + Environment.NewLine + Environment.NewLine + ex.Message, MainForm.PluginTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            model.SaveUI();
            model.Export();
            model.LoadUI();
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to import model!" + Environment.NewLine + Environment.NewLine + ex.Message, MainForm.PluginTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void brgObjectsTreeListView_SelectionChanged(object sender, EventArgs e)
        {
            if (this.brgObjectsTreeListView.SelectedObject == null)
            {
                return;
            }

            if (this.brgObjectsTreeListView.SelectedObject is BrgAttachpoint)
            {
                brg.LoadAttachpointUI();
            }
            else if (this.brgObjectsTreeListView.SelectedObject is BrgMesh)
            {
                brg.LoadMeshUI();
            }
            else if (this.brgObjectsTreeListView.SelectedObject is BrgMaterial)
            {
                brg.LoadMaterialUI();
            }

            this.brgObjectListView.SetObjects(new object[] { this.brgObjectsTreeListView.SelectedObject });
        }
        #endregion

        #region Grn
        private void grnObjectsTreeListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.grnObjectsTreeListView.SelectedObject == null)
            {
                return;
            }

            if (this.grnObjectsTreeListView.SelectedObject is GrnBone)
            {
                grn.LoadBoneUI();
            }
            else if (this.grnObjectsTreeListView.SelectedObject is GrnMesh)
            {
                grn.LoadMeshUI();
            }
            else if (this.grnObjectsTreeListView.SelectedObject is GrnMaterial)
            {
                grn.LoadMaterialUI();
            }
            else if (this.grnObjectsTreeListView.SelectedObject is GrnTexture)
            {
                grn.LoadTextureUI();
            }

            this.grnObjectListView.SetObjects(new object[] { this.grnObjectsTreeListView.SelectedObject });
        }
        #endregion

        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mainTabControl.SelectedIndex == 0)
            {
                this.model = this.brg;
                //MessageBox.Show("brg");
            }
            else
            {
                this.model = this.grn;
                //MessageBox.Show("grn");
            }
        }

        [System.Diagnostics.Conditional("DEBUG")] // Don't allow calls to this func in release mode
        public static void DebugBox(string message)
        {
#if DEBUGBOX
            MessageBox.Show(message);
#endif
        }

        private void objectListView1_CellEditStarting(object sender, CellEditEventArgs e)
        {
            if (e.Value is BrgMatFlag)
            {
                CheckedListBox clb = new CheckedListBox();
                Rectangle rect = new Rectangle(e.CellBounds.Left, e.CellBounds.Bottom, e.CellBounds.Width, 300);
                clb.Bounds = rect;

                foreach (object value in Enum.GetValues(typeof(BrgMatFlag)))
                {
                    clb.Items.Add(value);
                }

                this.SetCheckedListBoxSelectedEnums<BrgMatFlag>(clb, (uint)e.Value);
                e.Control = clb;
            }
            else if (e.Value is BrgMeshFlag)
            {
                CheckedListBox clb = new CheckedListBox();
                Rectangle rect = new Rectangle(e.CellBounds.Left, e.CellBounds.Bottom, e.CellBounds.Width, 300);
                clb.Bounds = rect;

                foreach (object value in Enum.GetValues(typeof(BrgMeshFlag)))
                {
                    clb.Items.Add(value);
                }

                this.SetCheckedListBoxSelectedEnums<BrgMeshFlag>(clb, (UInt16)e.Value);
                e.Control = clb;
            }
            else if (e.Value is BrgMeshFormat)
            {
                CheckedListBox clb = new CheckedListBox();
                Rectangle rect = new Rectangle(e.CellBounds.Left, e.CellBounds.Bottom, e.CellBounds.Width, 300);
                clb.Bounds = rect;

                foreach (object value in Enum.GetValues(typeof(BrgMeshFormat)))
                {
                    clb.Items.Add(value);
                }

                this.SetCheckedListBoxSelectedEnums<BrgMeshFormat>(clb, (UInt16)e.Value);
                e.Control = clb;
            }
            else if (e.Value is Color3D)
            {
                ThemeColorPicker tcp = new ThemeColorPicker();
                tcp.Location = new Point(e.CellBounds.Left, e.CellBounds.Bottom);

                Color3D col = (Color3D)e.Value;
                tcp.Color = Color.FromArgb(Convert.ToByte(col.R * Byte.MaxValue),
                    Convert.ToByte(col.G * Byte.MaxValue),
                    Convert.ToByte(col.B * Byte.MaxValue));
                e.Control = tcp;
            }
        }

        private void objectListView1_CellEditFinishing(object sender, CellEditEventArgs e)
        {
            if (e.Cancel)
                return;

            if (e.Value is BrgMatFlag)
            {
                e.NewValue = this.GetCheckedListBoxSelectedEnums<BrgMatFlag>((CheckedListBox)e.Control);
                //((BrgMaterial)e.RowObject).Flags = (BrgMatFlag)e.NewValue;

                // Any updating will have been down in the SelectedIndexChanged event handler
                // Here we simply make the list redraw the involved ListViewItem
                //((ObjectListView)sender).RefreshItem(e.ListViewItem);

                // We have updated the model object, so we cancel the auto update
                //e.Cancel = true;
            }
            else if (e.Value is BrgMeshFlag)
            {
                e.NewValue = this.GetCheckedListBoxSelectedEnums<BrgMeshFlag>((CheckedListBox)e.Control);
                //brg.File.UpdateMeshSettings((BrgMeshFlag)e.NewValue, brg.File.Meshes[0].Header.Format,
                //    brg.File.Meshes[0].Header.AnimationType, brg.File.Meshes[0].Header.InterpolationType);
                //((ObjectListView)sender).RefreshItem(e.ListViewItem);
                //e.Cancel = true;
            }
            else if (e.Value is BrgMeshFormat)
            {
                e.NewValue = this.GetCheckedListBoxSelectedEnums<BrgMeshFormat>((CheckedListBox)e.Control);
                //e.Cancel = true;
            }
            else if (e.Value is Color3D)
            {
                Color c = ((ThemeColorPicker)e.Control).Color;
                e.NewValue = new Color3D((float)c.R / byte.MaxValue, (float)c.G / byte.MaxValue, (float)c.B / byte.MaxValue);
            }
        }

        private void objectListView1_FormatCell(object sender, FormatCellEventArgs e)
        {
            if (e.CellValue is Color3D)
            {
                Color3D col = (Color3D)e.CellValue;
                e.SubItem.BackColor = Color.FromArgb(Convert.ToByte(col.R * Byte.MaxValue),
                    Convert.ToByte(col.G * Byte.MaxValue),
                    Convert.ToByte(col.B * Byte.MaxValue));
                //e.SubItem.ForeColor = this.ContrastColor(e.SubItem.BackColor);
                e.SubItem.Text = string.Empty;
            }
        }
    }
}
