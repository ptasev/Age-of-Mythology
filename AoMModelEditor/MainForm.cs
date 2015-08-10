#define DEBUGBOX // allows the use of messageboxes in debug mode

namespace AoMModelEditor
{
    using AoMEngineLibrary.Graphics;
    using AoMEngineLibrary.Graphics.Brg;
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

        IModelUi model;
        BrgUi brg;
        GrnUi grn;

        public MainForm()
        {
            InitializeComponent();

            //BrgFile f = new BrgFile(File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\cavalry g prodromos_attacka.brg", FileMode.Open, FileAccess.Read, FileShare.Read));
            //BrgFile f2 = new BrgFile(File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\cavalry g prodromos_attacka.brg", FileMode.Open, FileAccess.Read, FileShare.Read));
            //f2.Materials[0].id = 12212;
            //int eq = f.Materials.IndexOf(f2.Materials[0]);

            foreach (string s in Directory.GetFiles(@"C:\Games\Steam\steamapps\common\Age of Mythology\mods\AoSW\models", "*.brg", SearchOption.AllDirectories))
            {
                BrgFile file = new BrgFile(File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read));
                for (int i = 0; i < file.Materials.Count; i++)
                {
                    MtrlFile mtrl = new MtrlFile(file.Materials[i]);
                    mtrl.Write(File.Open(Path.Combine(@"C:\Games\Steam\steamapps\common\Age of Mythology\mods\AoSW\materials", Path.GetFileNameWithoutExtension(s) + "_" + i + ".mtrl"), FileMode.Create, FileAccess.Write, FileShare.Read));
                }
            }

            //this.mainTabControl.TabPages.Remove(this.grnSettingsTabPage);

            // General Tab
            genMeshFlagsCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMeshFlag));
            genMeshFormatCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMeshFormat));

            // Attachpoints
            this.attachpointListBox.ValueMember = "Index";
            this.attachpointListBox.DisplayMember = "MaxName";

            // Materials
            materialListBox.SelectedIndexChanged += materialListBox_SelectedIndexChanged;
            materialFlagsCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMatFlag));

            // Grn Settings
            grnObjectsListBox.SelectedIndexChanged += grnObjectsListBox_SelectedIndexChanged;

            //plugin = new MaxPlugin();
            //this.Controls.Add(plugin);
            //plugin.Dock = DockStyle.Fill;
            Settings.Read();
            brg = new BrgUi(this);
            grn = new GrnUi(this);
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
                    model.SaveUi();
                    model.Write(File.Open(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read));
                    model.LoadUi();
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

            model.SaveUi();
            model.Import();
            model.LoadUi();
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
            model.SaveUi();
            model.Export();
            model.LoadUi();
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
        private void attachpointListBox_MouseEnter(object sender, EventArgs e)
        {
            attachpointListBox.Focus();
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
