using AoMEngineLibrary.Anim;
using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace AoMTextEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            openFileDialog.Filter = "Text files|*.txt";
            saveFileDialog.Filter = "Text files|*.txt";
            fastColoredTextBox1.Text = string.Empty;
            fastColoredTextBox1.Language = FastColoredTextBoxNS.Language.CSharp;
            documentMap1.Target = fastColoredTextBox1;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //TextReader reader = new StreamReader(File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
                    //AnimFile file = new AnimFile(reader);
                    //fastColoredTextBox1.Text = file.XDoc.ToString();
                    using (StreamReader sr = new StreamReader(File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
                    {
                        fastColoredTextBox1.Text = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("The file could not be read:");
                //Console.WriteLine(ex.Message);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    saveFileDialog.FileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    saveFileDialog.InitialDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                }
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter outfile = new StreamWriter(saveFileDialog.FileName))
                    {
                        outfile.Write(fastColoredTextBox1.Text);
                    }
                }
            }
            catch
            {

            }
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.CollapseAllFoldingBlocks();
        }

        private void formatDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Range r = fastColoredTextBox1.Selection.Clone();
            fastColoredTextBox1.DoAutoIndent();
            for (int i = 0; i < fastColoredTextBox1.LinesCount; i++)
            {
                //fastColoredTextBox1.DoAutoIndent(i);
            }
            //fastColoredTextBox1.Selection = r;
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.ExpandAllFoldingBlocks();
        }

        private void exportAsXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    saveFileDialog.FileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    saveFileDialog.InitialDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                }
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var ms = new MemoryStream())
                    using (var sw = new StreamWriter(ms))
                    {
                        sw.Write(fastColoredTextBox1.Text);
                        sw.Flush();
                        ms.Seek(0, SeekOrigin.Begin);

                        AnimFile.ConvertToXml(ms, File.Open(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read));
                    }
                }
            }
            catch
            {
                MessageBox.Show("Failed to export the xml file!");
            }
        }

        private void importAsXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var ms = new MemoryStream())
                    using (var sr = new StreamReader(ms))
                    {
                        AnimFile.ConvertToAnim(File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read), ms);

                        ms.Seek(0, SeekOrigin.Begin);
                        fastColoredTextBox1.Text = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to import the xml file!");
                //MessageBox.Show(ex.Message);
            }
        }
    }
}
