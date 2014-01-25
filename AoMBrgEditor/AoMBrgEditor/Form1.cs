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
using AoMEngineLibrary;

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
            HashSet<int> ttt = new HashSet<int>();
            foreach (string s in Directory.GetFiles(@"F:\Mods\Tools\AoMed\bars\models2", "*.brg", SearchOption.AllDirectories))
            {
                try
                {
                    file = new BrgFile(File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read));//, new MaxPlugin());
                    //if ((file.Mesh[0].unknown09[6] != file.Mesh[0].unknown09d - 1) && (file.Material.Count - 1 != file.Mesh[0].unknown09[6]))
                    if (file.Mesh[0].unknown091 == 4)
                    {
                        //output += Path.GetFileName(s) + Environment.NewLine;
                        //output += "\tu09[0] = " + file.Mesh[0].unknown09[0];
                        //output += "\tu09[0] = " + file.Mesh[0].unknown09[0];
                        //output += Environment.NewLine;
                    }
                    if (file.Mesh[0].flags.HasFlag(BrgMeshFlag.VERTCOLOR) || file.Mesh[0].flags.HasFlag(BrgMeshFlag.TRANSPCOLOR) || file.Mesh[0].flags.HasFlag(BrgMeshFlag.CHANGINGCOL))
                    //if (file.Mesh[0].unknown09Unused != 0)
                    {
                        output += Path.GetFileName(s) + Environment.NewLine;
                        output += "\tu09[0] = " + file.Mesh[0].numIndex0;
                        output += "\tu09[1] = " + file.Mesh[0].numMatrix0;
                        output += "\tu09[2] = " + file.Mesh[0].unknown091;
                        output += "\tu09[3] = " + file.Mesh[0].unknown09Unused;
                        output += "\tu09[6] = " + file.Mesh[0].lastMaterialIndex;
                        output += "\tu09[7] = " + file.Mesh[0].animTime;
                        output += Environment.NewLine;
                    }
                    if ((file.Mesh[0].numMaterialsUsed - 1 != file.Mesh[0].lastMaterialIndex))
                    {
                        //output += Path.GetFileName(s) + Environment.NewLine;
                        //output += "\tu09[6] = " + file.Mesh[0].unknown09[6];
                    }
                    for (int j = 0; j < file.Mesh.Count; j++)
                    {
                        //ttt.Add(file.Mesh[j].unknown09[2]);
                    }
                    if (file.Mesh.Count > 1 && (file.Mesh[0].format != file.Mesh[1].format))
                    {
                        //output += Path.GetFileName(s) + Environment.NewLine;
                    }
                    for (int j = 0; j < file.Mesh.Count; j++)
                    {
                        if (!ttt.Contains((int)file.Mesh[j].format))
                        {
                            //ttt.Add(file.Mesh[j].meshFormat2);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    //if (file.Mesh[0].unknown091 == 550)
                    //if (file.Mesh[0].animTimeMult != 1)
                    //if (file.Mesh.Count > 1 && (file.Mesh[0].flags | BrgMeshFlag.NOTFIRSTMESH) != file.Mesh[1].flags)
                    if (false)
                    {
                        bool print = false;
                        for (int i = 1; i < file.Mesh.Count; i++)
                        {
                            if (true)
                            {
                                print = true;
                            }
                        }
                        if (print)
                        {
                            //MessageBox.Show("ttt");
                            output += Path.GetFileName(s);
                            output += "\tu09[3] = " + file.Mesh[1].unknown09Unused + Environment.NewLine;
                            //output += "\tu10 = " + Convert.ToInt32(file.Mesh[0].unknown10)  + Environment.NewLine;
                        }
                        continue;
                        //output += Path.GetFileName(s) + Environment.NewLine;
                        output += "animTime = " + file.AsetHeader.animTime;
                        output += "\tmeshCount = " + file.Mesh.Count;
                        //output += "\tmeshFormat = " + file.Mesh[0].meshFormat;
                        output += "\tunknown01b = " + file.Mesh[0].format;
                        output += "\tunknown02 = " + file.Mesh[0].properties;
                        //output += "\tu03[0] = " + file.Mesh[0].unknown03[0];
                        //output += "\tu03[1] = " + file.Mesh[0].unknown03[1];
                        //output += "\tu03[2] = " + file.Mesh[0].unknown03[2];
                        // output += "\tu03[3] = " + file.Mesh[0].unknown03[3];
                        //output += "\tu04[4] = " + file.Mesh[0].unknown04[4];
                        //output += "\tu04[5] = " + file.Mesh[0].unknown04[5];
                        output += "\tu09[0] = " + file.Mesh[0].numIndex0;
                        output += "\tu09[1] = " + file.Mesh[0].numMatrix0;
                        output += "\tu09[2] = " + file.Mesh[0].unknown091;
                        output += "\tu09[3] = " + file.Mesh[0].unknown09Unused;
                        output += "\taTime = " + Convert.ToString(file.Mesh[0].animTime);
                        //output += "\tunknown09e = " + Convert.ToInt32(file.Mesh[0].unknown09e);
                        //output += "\tunknown09b = " + Convert.ToInt32(file.Mesh[0].unknown09b);
                        //output += "\tlenSpace = " + Convert.ToInt32(file.Mesh[0].lenSpace);
                        output += "\tu09d = " + Convert.ToInt32(file.Mesh[0].numMaterialsUsed);
                        //output += "\tu10 = " + Convert.ToInt32(file.Mesh[0].unknown10);
                        output += "\tflags = " + file.Mesh[0].flags;
                        output += Environment.NewLine;
                    }
                    // MATERIALS _______________________________________________________
                    if (file.Material.Count > 0)
                    {
                        bool print = false;
                        for (int j = 0; j < file.Material.Count; j++)
                        {
                            ttt.Add((int)file.Material[j].flags);
                            //if (file.Material[j].flags.HasFlag(BrgMatFlag.MATTEX2))
                            //if (file.Material[j].flags.HasFlag(BrgMatFlag.REFLECTTEX) && file.Material[j].sfx.Count > 1)
                            if ((((int)file.Material[j].flags & 0x000000FF) != (int)BrgMatFlag.DIFFUSETEXTURE) && (((int)file.Material[j].flags & 0x000000FF) == 0))
                            //if ((int)file.Material[j].flags == 0x00800000)
                            //if ((int)file.Material[j].flags == 0x00800030)
                            //if ((int)file.Material[j].flags == 0x00800800)
                            //if ((int)file.Material[j].flags == 0x00800830)
                            //if ((int)file.Material[j].flags == 0x00801030)
                            //if ((int)file.Material[j].flags == 0x00808330) // -TT
                            //if ((int)file.Material[j].flags == 0x00820000)
                            //if ((int)file.Material[j].flags == 0x00820030)
                            //if ((int)file.Material[j].flags == 0x00840030)
                            //if ((int)file.Material[j].flags == 0x00860030)
                            //if ((int)file.Material[j].flags == 0x00880030)
                            //if ((int)file.Material[j].flags == 0x008a0030)
                            //if ((int)file.Material[j].flags == 0x00a00030)
                            //if ((int)file.Material[j].flags == 0x00A20030) // -TT
                            //if ((int)file.Material[j].flags == 0x00a40030)
                            //if ((int)file.Material[j].flags == 0x01800030)
                            //if ((int)file.Material[j].flags == 0x02800030)
                            //if ((int)file.Material[j].flags == 0x02820030)
                            //if ((int)file.Material[j].flags == 0x04820030) // -TT
                            //if ((int)file.Material[j].flags == 0x1c800030)
                            //if ((int)file.Material[j].flags == 0x1c840030)
                            //if ((int)file.Material[j].flags == 0x1e800030)
                            //if (file.Material[j].flags.HasFlag(BrgMatFlag.MATNONE8))
                            {
                                print = true;
                                output += "id = " + file.Material[j].id;
                                output += "\tflags = " + file.Material[j].flags;
                                output += "\tflags78 = " + ((int)file.Material[j].flags & 0x000000FF);
                                output += "\tu01b = " + file.Material[j].unknown01b;
                                output += "\tname = " + file.Material[j].name;
                                output += Environment.NewLine;
                            }
                        }
                        if (print)
                        {
                            output += Path.GetFileName(s);
                            output += "\t" + file.Mesh[0].flags + Environment.NewLine;
                        }
                    }
                }
                catch (Exception ex)
                {
                    output += Path.GetFileName(s) + Environment.NewLine;
                    output += ex.Message + "\t---------------------------------------------------------------------------" + Environment.NewLine;
                    continue;
                }
            }
            IEnumerable<int> tttSort = ttt.OrderBy(tVal => tVal);
            foreach (int t in tttSort)
            {
                output += String.Format("0x{0:X8} ", t);
            }
            richTextBox1.Text = output;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "brg files|*.brg";
            
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                file = new BrgFile(File.Open(dlg.FileName, FileMode.Open, FileAccess.Read, FileShare.Read));//, new MaxPlugin());
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
