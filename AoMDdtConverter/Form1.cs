namespace AoMDdtConverter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        private List<string> args;
        private string exportSettings;
        private string importSettings;
        private bool convertRunning;

        public Form1()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Colossus;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.importSettingsTableLayoutPanel.SetColumnSpan(this.defaultCompressionGroupBox, 2);
            this.outputRichTextBox.HideSelection = false;
            
            Dictionary<string, string> compressionTypes = new Dictionary<string, string>(7);
            compressionTypes.Add("BC1 (DXT1)", "BC1");
            compressionTypes.Add("BC2 (DXT3)", "BC2");
            compressionTypes.Add("BC3 (DXT5)", "BC3");
            compressionTypes.Add("RGBA8", "DeflatedRGBA8");
            compressionTypes.Add("RGB8", "DeflatedRGB8");
            compressionTypes.Add("RG8", "DeflatedRG8");
            compressionTypes.Add("R8", "DeflatedR8");
            this.defaultCompressionComboBox.DataSource = compressionTypes.ToList();
            this.defaultCompressionComboBox.ValueMember = "Value";
            this.defaultCompressionComboBox.DisplayMember = "Key";
            this.defaultCompressionComboBox.SelectedIndex = 2;
        }
        public Form1(string[] args)
            : this()
        {
            this.args = args.ToList();
            foreach (string f in args)
            {
                FileInfo fi = new FileInfo(f);

                if (fi.Attributes.HasFlag(FileAttributes.Directory))
                {
                    this.args.Remove(f);
                    HashSet<string> filteredFiles = new HashSet<string>();
                    filteredFiles.UnionWith(Directory.GetFiles(f, "*.ddt", SearchOption.TopDirectoryOnly));
                    filteredFiles.UnionWith(Directory.GetFiles(f, "*.tga", SearchOption.TopDirectoryOnly));
                    this.args.AddRange(filteredFiles);
                }
            }
            //this.args = new string[1] { @"C:\Games\Steam\steamapps\common\Age of Mythology\tools\TextureCompiler_v2\converted\agamemnon map.ddt"};
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (this.args.Count == 0)
            {
                MessageBox.Show("Drag and drop ddt, or tga files to begin a conversion process!");
                this.Close();
            }
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            if (this.convertRunning)
            {
                return;
            }

            try
            {
                this.convertRunning = true;
                this.UpdateProgress(0);
                this.outputRichTextBox.Text = string.Empty;
                this.CreateSettings();
                Thread backgroundThread = new Thread(this.ConvertFiles);
                backgroundThread.Start();
            }
            finally { }
        }
        private void ConvertFiles()
        {
            int currentFileNum = 1;

            string outputDir;
            if (args.Count > 1)
            {
                outputDir = Path.Combine(Path.GetDirectoryName(args[0]), "converted");
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }
            }
            else
            {
                outputDir = Path.GetDirectoryName(args[0]);
            }

            foreach (string f in args)
            {
                try
                {
                    this.UpdateProcessingLabel("Processing " + Path.GetFileName(f) + "..." + Environment.NewLine);

                    if (Path.GetExtension(f) == ".ddt")
                    {
                        if (this.allMapRadioButton.Checked)
                        {
                            string outputFileNoExt = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(f));
                            this.ExportDdt(f, outputFileNoExt + ".tga", exportSettings);
                            this.ExportDdt(f, outputFileNoExt + "_nm.tga", "-nm " + exportSettings);
                            this.ExportDdt(f, outputFileNoExt + "_spec.tga", "-spec " + exportSettings);
                            this.ExportDdt(f, outputFileNoExt + "_gloss.tga", "-gloss " + exportSettings);
                            File.Copy(outputFileNoExt + ".bti", outputFileNoExt + "_nm.bti");
                            File.Copy(outputFileNoExt + ".bti", outputFileNoExt + "_spec.bti");
                            File.Copy(outputFileNoExt + ".bti", outputFileNoExt + "_gloss.bti");
                        }
                        else
                        {
                            this.ExportDdt(f, Path.Combine(outputDir, Path.ChangeExtension(Path.GetFileName(f), ".tga")), exportSettings);
                        }
                    }
                    else if (Path.GetExtension(f) == ".tga")
                    {
                        this.ImportDdt(f, Path.Combine(outputDir, Path.ChangeExtension(Path.GetFileName(f), ".ddt")), importSettings);
                    }

                    int progVal = (currentFileNum++ * 100) / args.Count;
                    this.UpdateProgress(progVal);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to convert " + Path.GetFileName(f) + "!" + Environment.NewLine + Environment.NewLine +
                        ex.ToString());
                }
            }

            this.convertRunning = false;
        }

        private void UpdateProcessingLabel(string processing)
        {
            if (this.processingLabel.InvokeRequired)
            {
                this.processingLabel.BeginInvoke(
                    new Action(() =>
                    {
                        this.processingLabel.Text = processing;
                    }
                ));
            }
            else
            {
                this.processingLabel.Text = processing;
            }
        }
        private void UpdateProgress(int progVal)
        {
            if (this.progressBar.InvokeRequired)
            {
                this.progressBar.BeginInvoke(
                    new Action(() =>
                    {
                        this.progressBar.Value = progVal;
                    }
                ));
            }
            else
            {
                this.progressBar.Value = progVal;
            }

            if (this.percentageLabel.InvokeRequired)
            {
                this.percentageLabel.BeginInvoke(
                    new Action(() =>
                    {
                        this.percentageLabel.Text = progVal.ToString() + "%";
                    }
                ));
            }
            else
            {
                this.percentageLabel.Text = progVal.ToString() + "%";
            }
        }
        private void UpdateOutput(string output)
        {
            if (this.outputRichTextBox.InvokeRequired)
            {
                this.outputRichTextBox.BeginInvoke(
                    new Action(() =>
                    {
                        this.outputRichTextBox.AppendText(output);
                    }
                ));
            }
            else
            {
                this.outputRichTextBox.AppendText(output);
            }
        }

        private void CreateSettings()
        {
            StringBuilder exps = new StringBuilder();
            StringBuilder imps = new StringBuilder();

            exps.Append(" -c " + this.defaultCompressionComboBox.SelectedValue);

            if (this.nmMapRadioButton.Checked)
            {
                imps.Append(" -nm");
            }
            else if (this.specMapRadioButton.Checked)
            {
                imps.Append(" -spec");
            }
            else if (this.glossMapRadioButton.Checked)
            {
                imps.Append(" -gloss");
            }

            this.importSettings = exps.ToString(1, exps.Length - 1);
            if (imps.Length > 0)
            {
                this.exportSettings = imps.ToString(1, imps.Length - 1);
            }
            else
            {
                this.exportSettings = string.Empty;
            }
        }

        private void ExportDdt(string input, string output, string exportSettings)
        {
            this.UpdateOutput(exportSettings + Environment.NewLine);
            string arguments = exportSettings + string.Format(" -i \"{0}\" -o \"{1}\"", input, output);

            Process process = new Process();

            process.StartInfo.FileName = Path.Combine(Application.StartupPath, "TextureExtractor.exe");
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            string stdoutput = process.StandardOutput.ReadToEnd();
            string stderror = process.StandardError.ReadToEnd();

            process.WaitForExit();
            Thread upOut = new Thread(() => this.UpdateProcessOutput(stdoutput, stderror));
            upOut.Start();
            upOut.Join();
        }

        private void ImportDdt(string input, string output, string importSettings)
        {
            this.UpdateOutput(importSettings + Environment.NewLine);
            string arguments = importSettings + string.Format(" -i \"{0}\" -o \"{1}\"", input, output);

            Process process = new Process();

            process.StartInfo.FileName = Path.Combine(Application.StartupPath, "TextureCompiler.exe");
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            string stdoutput = process.StandardOutput.ReadToEnd();
            string stderror = process.StandardError.ReadToEnd();

            process.WaitForExit();
            Thread upOut = new Thread( () => this.UpdateProcessOutput(stdoutput, stderror));
            upOut.Start();
            upOut.Join();
        }

        private void UpdateProcessOutput(string std, string err)
        {
            this.UpdateOutput(std);
            this.UpdateOutput(err);
        }
    }
}
