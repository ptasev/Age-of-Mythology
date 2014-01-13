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

namespace AoMBrgEditor
{
    public partial class Form1 : Form
    {
        BrgFile file;

        public Form1()
        {
            InitializeComponent();
            //DdtFile ddt = new DdtFile(File.Open(@"D:\Petar\LatestFiles\Mods\Tools\AoMed\bars\textures\archer n throwing axeman standard.ddt", FileMode.Open, FileAccess.Read, FileShare.Read));
            //Dds dds = new Dds(ddt);
            //dds.Write(File.Open(@"D:\Petar\LatestFiles\Mods\Tools\AoMed\archer n throwing axeman standard.dds", FileMode.Create, FileAccess.Write, FileShare.Read), -1);
            string output = "";
            int i = 0;
            foreach (string s in Directory.GetFiles(@"D:\Petar\LatestFiles\Mods\Tools\AoMed\bars\models", "*.brg", SearchOption.TopDirectoryOnly))
            {
                if (i < 1000)
                {
                    i++;
                    continue;
                }
                if (i == 1000)
                    break;
                try
                {
                    output += Path.GetFileName(s) + Environment.NewLine;
                    file = new BrgFile(File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read));
                }
                catch { continue; }
                output += "animTime = " + file.AsetHeader.animTime;
                output += "\tmeshCount = " + file.Mesh.Count;
                if (file.Mesh.Count > 0)
                {
                    output += "\tmeshFormat = " + file.Mesh[0].meshFormat;
                    output += "\tunknown01b = " + file.Mesh[0].meshFormat2;
                    output += "\tunknown02 = " + file.Mesh[0].unknown02;
                    //output += "\tu03[0] = " + file.Mesh[0].unknown03[0];
                    //output += "\tu03[1] = " + file.Mesh[0].unknown03[1];
                    //output += "\tu03[2] = " + file.Mesh[0].unknown03[2];
                    // output += "\tu03[3] = " + file.Mesh[0].unknown03[3];
                    output += "\tu03[7] = " + file.Mesh[0].unknown03[7];
                    output += "\tu03[8] = " + file.Mesh[0].groundZ;
                    output += "\tu04[0] = " + file.Mesh[0].unknown04[0];
                    output += "\tu04[1] = " + file.Mesh[0].unknown04[1];
                    output += "\tu04[2] = " + file.Mesh[0].unknown04[2];
                    output += "\tu04[3] = " + file.Mesh[0].unknown04[3];
                    //output += "\tu04[4] = " + file.Mesh[0].unknown04[4];
                    //output += "\tu04[5] = " + file.Mesh[0].unknown04[5];
                    output += "\tu09[0] = " + file.Mesh[0].unknown09[0];
                    output += "\tu09[1] = " + file.Mesh[0].unknown09[1];
                    output += "\tu09[2] = " + file.Mesh[0].unknown09[2];
                    output += "\tu09[3] = " + file.Mesh[0].unknown09[3];
                    //output += "\taTime = " + Convert.ToString(file.Mesh[0].animTime);
                    //output += "\tunknown09e = " + Convert.ToInt32(file.Mesh[0].unknown09e);
                    //output += "\tunknown09b = " + Convert.ToInt32(file.Mesh[0].unknown09b);
                    output += "\tlenSpace = " + Convert.ToInt32(file.Mesh[0].lenSpace);
                    output += "\tu09d = " + Convert.ToInt32(file.Mesh[0].unknown09d);
                    //output += "\tu10 = " + Convert.ToInt32(file.Mesh[0].unknown10);
                    output += "\tflags = " + file.Mesh[0].flags;
                }
                output += Environment.NewLine;
                i++;
            }
            richTextBox1.Text = output;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "brg files|*.brg";
            
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                file = new BrgFile(File.Open(dlg.FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "br3 files|*.br3";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                file.ReadBr3(File.Open(dlg.FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "brg files|*.brg";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                file.Write(File.Open(dlg.FileName, FileMode.Create, FileAccess.Write, FileShare.Read));
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg2 = new SaveFileDialog();
            dlg2.Filter = "br3 files|*.br3";
            if (dlg2.ShowDialog() == DialogResult.OK)
            {
                file.WriteBr3(File.Open(dlg2.FileName, FileMode.Create, FileAccess.Write, FileShare.Read));
            }
        }
    }
}
