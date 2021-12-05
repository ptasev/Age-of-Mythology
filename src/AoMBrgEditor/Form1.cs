using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AoMEngineLibrary.Graphics.Brg;
using AoMEngineLibrary.Graphics.Grn;
using AoMEngineLibrary.Graphics.Prt;

namespace AoMBrgEditor
{
    // This project started as the first app I used to reverse-engineer the brg format
    // the initial intent was to make a desktop app to help with modding brg files.
    // I ended up going in a different direction, and this turned into a sandbox for experimenting
    public partial class Form1 : Form
    {
        BrgFile? file;

        public Form1()
        {
            InitializeComponent();

            string output = "";

            richTextBox1.Text = output;

            PrtTesting();
        }

        private string BrgDevelopmentCode()
        {
            // Code I used during brg development to understand the format
            string output = ""; // should use StringBuilder instead
            HashSet<int> ttt = new HashSet<int>();

            //@"C:\Games\Steam\SteamApps\common\Age of Mythology\models"
            //@"C:\Users\Petar\Desktop\modelsAlpha"
            foreach (string s in Directory.GetFiles(@"C:\Games\Steam\SteamApps\common\Age of Mythology\models", "*.brg", SearchOption.AllDirectories))
            //foreach (string s in Directory.GetFiles(@"C:\Users\Petar\Desktop\modelsBeta", "*.brg", SearchOption.AllDirectories))
            //foreach (string s in Directory.GetFiles(@"C:\Users\Petar\Desktop\modelsAlpha", "*.brg", SearchOption.AllDirectories))
            {
                break;
                try
                {
                    file = new BrgFile(File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read));
                    //if (file.Meshes[0].Header.Flags.HasFlag(BrgMeshFlag.COLORCHANNEL) || file.Meshes[0].Header.Flags.HasFlag(BrgMeshFlag.COLORALPHACHANNEL))
                    //{
                    //    output += Path.GetFileName(s) + " " + file.Meshes[0].Header.Flags.ToString() + Environment.NewLine; ;
                    //}
                    //if (file.Meshes[0].Header.Flags.HasFlag(BrgMeshFlag.PARTICLESYSTEM) || file.Meshes[0].Header.Flags.HasFlag(BrgMeshFlag.PARTICLEPOINTS))
                    //{
                    //    output += Path.GetFileName(s) + " " + file.Meshes[0].Header.Flags.ToString() + Environment.NewLine; ;
                    //}
                    //if (file.Meshes[0].Header.InterpolationType == BrgMeshInterpolationType.ConformToTerrain)
                    //{
                    //    output += Path.GetFileName(s) + " " + file.Meshes[0].Header.Flags.ToString() + Environment.NewLine; ;
                    //}
                    foreach (var mat in file.Materials)
                    {
                        if (!string.IsNullOrEmpty(mat.CubeMapInfo.CubeMapName))
                        {
                            output += Path.GetFileName(s) + " " + mat.CubeMapInfo.CubeMapName + Environment.NewLine; ;
                        }
                    }
                }
                catch (Exception ex)
                {
                    output += Path.GetFileName(s) + Environment.NewLine;
                    output += ex.Message + "\t---------------------------------------------------------------------------" + Environment.NewLine;
                }
                continue;
                try
                {
                    file = new BrgFile(File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read));
                    if (file.Meshes.Count > 0 && (file.Meshes[0].Header.Flags.HasFlag(BrgMeshFlag.COLORCHANNEL)))
                    {
                        output += Path.GetFileName(s) + " " + file.Meshes[0].Header.Flags.ToString() + Environment.NewLine;
                    }
                    if (file.Meshes.Count > 0 && (file.Meshes[0].Header.Format.HasFlag(BrgMeshFormat.ANIMTEXCOORDSNAP)))
                    {
                        //output += Path.GetFileName(s) + " " + file.Meshes[0].Header.Format.ToString() + Environment.NewLine;
                    }
                    if (file.Meshes.Count > 0 && (file.Meshes[0].Header.AnimationType.HasFlag(BrgMeshAnimType.NonUniform)))
                    {
                        //output += Path.GetFileName(s) + " " + file.Meshes[0].Header.AnimationType.ToString() + Environment.NewLine;
                    }
                    //if (file.Materials.Count > 0)
                    //{
                    //    foreach (BrgMaterial mat in file.Materials)
                    //    {
                    //        //if(mat.sfx.Count > 1)
                    //        if (mat.Flags.HasFlag(BrgMatFlag.ILLUMREFLECTION))
                    //        {
                    //            output += Path.GetFileName(s) + "," + mat.DiffuseMap + "," + mat.BumpMap + "," + mat.sfx[0].Name;
                    //            //output += Path.GetFileName(s) + " " + mat.Flags.ToString() + Environment.NewLine;
                    //        }
                    //        else
                    //        {
                    //            output += Path.GetFileName(s) + "," + mat.DiffuseMap + "," + mat.BumpMap + "," + string.Empty;
                    //        }
                    //        foreach (BrgMatFlag eval in Enum.GetValues(typeof(BrgMatFlag)).Cast<BrgMatFlag>())
                    //        {
                    //            output += ",";
                    //            if (mat.Flags.HasFlag(eval))
                    //            {
                    //                output += eval.ToString();
                    //            }
                    //        }
                    //        output += Environment.NewLine;
                    //    }
                    //}
                    //file.Write(File.Open(@"C:\Users\Petar\Desktop\modelsAlphaConv\" + Path.GetFileName(s), FileMode.Create, FileAccess.Write, FileShare.Read));
                }
                catch (Exception ex)
                {
                    output += Path.GetFileName(s) + Environment.NewLine;
                    output += ex.Message + "\t---------------------------------------------------------------------------" + Environment.NewLine;
                    //break;
                }
                continue;
                try
                {
                    file = new BrgFile(File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read));//, new MaxPlugin());
                    //if ((file.Mesh[0].unknown09[6] != file.Mesh[0].unknown09d - 1) && (file.Material.Count - 1 != file.Mesh[0].unknown09[6]))
                    if (file.Meshes[0].ExtendedHeader.NameLength == 4)
                    {
                        //output += Path.GetFileName(s) + Environment.NewLine;
                        //output += "\tu09[0] = " + file.Mesh[0].unknown09[0];
                        //output += "\tu09[0] = " + file.Mesh[0].unknown09[0];
                        //output += Environment.NewLine;
                    }
                    /*if (file.Mesh[0].flags.HasFlag(BrgMeshFlag.VERTCOLOR) || file.Mesh[0].flags.HasFlag(BrgMeshFlag.TRANSPCOLOR) || file.Mesh[0].flags.HasFlag(BrgMeshFlag.CHANGINGCOL))
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
                    }*/
                    if ((file.Meshes[0].ExtendedHeader.NumUniqueMaterials - 1 != file.Meshes[0].ExtendedHeader.NumMaterials))
                    {
                        //output += Path.GetFileName(s) + Environment.NewLine;
                        //output += "\tu09[6] = " + file.Mesh[0].unknown09[6];
                    }
                    for (int j = 0; j < file.Meshes.Count; j++)
                    {
                        //ttt.Add(file.Mesh[j].unknown09[2]);
                    }
                    if (file.Meshes.Count > 1 && (file.Meshes[0].Header.Format != file.Meshes[1].Header.Format))
                    {
                        //output += Path.GetFileName(s) + Environment.NewLine;
                    }
                    for (int j = 0; j < file.Meshes.Count; j++)
                    {
                        if (!ttt.Contains((int)file.Meshes[j].Header.Format))
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
                    if (true)
                    {
                        bool print = false;
                        for (int i = 0; i < file.Meshes.Count; i++)
                        {
                            if ((uint)file.Meshes[0].Header.AnimationType != 256 && (uint)file.Meshes[0].Header.AnimationType != 1 && (uint)file.Meshes[0].Header.AnimationType != 0)
                            {
                                print = true;
                            }
                        }
                        if (print)
                        {
                            //MessageBox.Show("ttt");
                            output += Path.GetFileName(s);
                            output += file.Meshes[0].Header.AnimationType + " ";
                            //output += "\tu09[3] = " + file.Mesh[1].unknown09Unused + Environment.NewLine;
                            //output += "\tu10 = " + Convert.ToInt32(file.Mesh[0].unknown10)  + Environment.NewLine;
                        }
                        continue;
                        //output += Path.GetFileName(s) + Environment.NewLine;
                        output += "animTime = " + file.AsetHeader.AnimTime;
                        output += "\tmeshCount = " + file.Meshes.Count;
                        //output += "\tmeshFormat = " + file.Mesh[0].meshFormat;
                        output += "\tunknown01b = " + file.Meshes[0].Header.Format;
                        output += "\tunknown02 = " + file.Meshes[0].Header.AnimationType;
                        //output += "\tu03[0] = " + file.Mesh[0].unknown03[0];
                        //output += "\tu03[1] = " + file.Mesh[0].unknown03[1];
                        //output += "\tu03[2] = " + file.Mesh[0].unknown03[2];
                        // output += "\tu03[3] = " + file.Mesh[0].unknown03[3];
                        //output += "\tu04[4] = " + file.Mesh[0].unknown04[4];
                        //output += "\tu04[5] = " + file.Mesh[0].unknown04[5];
                        //output += "\tu09[0] = " + file.Mesh[0].numIndex0;
                        //output += "\tu09[1] = " + file.Mesh[0].numMatrix0;
                        output += "\tu09[2] = " + file.Meshes[0].ExtendedHeader.NameLength;
                        output += "\tu09[3] = " + file.Meshes[0].ExtendedHeader.PointRadius;
                        output += "\taTime = " + Convert.ToString(file.Meshes[0].ExtendedHeader.AnimationLength);
                        //output += "\tunknown09e = " + Convert.ToInt32(file.Mesh[0].unknown09e);
                        //output += "\tunknown09b = " + Convert.ToInt32(file.Mesh[0].unknown09b);
                        //output += "\tlenSpace = " + Convert.ToInt32(file.Mesh[0].lenSpace);
                        output += "\tu09d = " + Convert.ToInt32(file.Meshes[0].ExtendedHeader.NumUniqueMaterials);
                        //output += "\tu10 = " + Convert.ToInt32(file.Mesh[0].unknown10);
                        output += "\tflags = " + file.Meshes[0].Header.Flags;
                        output += Environment.NewLine;
                    }
                    // MATERIALS _______________________________________________________
                    if (file.Materials.Count > 50)
                    {
                        bool print = false;
                        for (int j = 0; j < file.Materials.Count; j++)
                        {
                            ttt.Add((int)file.Materials[j].Flags);
                            //if (file.Material[j].flags.HasFlag(BrgMatFlag.MATTEX2))
                            //if (file.Material[j].flags.HasFlag(BrgMatFlag.REFLECTTEX) && file.Material[j].sfx.Count > 1)
                            if ((((int)file.Materials[j].Flags & 0x000000FF) != (int)BrgMatFlag.Alpha) && (((int)file.Materials[j].Flags & 0x000000FF) == 0))
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
                                output += "id = " + file.Materials[j].Id;
                                output += "\tflags = " + file.Materials[j].Flags;
                                output += "\tflags78 = " + ((int)file.Materials[j].Flags & 0x000000FF);
                                output += "\tu01b = " + file.Materials[j].Reserved;
                                output += "\tname = " + file.Materials[j].DiffuseMapName;
                                output += Environment.NewLine;
                            }
                        }
                        if (print)
                        {
                            output += Path.GetFileName(s);
                            output += "\t" + file.Meshes[0].Header.Flags + Environment.NewLine;
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

            return output;
        }

        private void AdjustAoSWIds()
        {
            // some sort of AoSW stuff
            //List<string> newData = new List<string>();
            //Dictionary<int, int> strIds = new Dictionary<int, int>();
            //using (StreamReader reader = new StreamReader(File.Open(@"C:\Users\Petar\Desktop\SWConv\SW.txt", FileMode.Open, FileAccess.Read, FileShare.Read)))
            //{
            //    int i = 0;
            //    while (!reader.EndOfStream)
            //    {
            //        string line = reader.ReadLine();
            //        int spIndex = line.IndexOf(' ');
            //        int id = Convert.ToInt32(line.Substring(0, spIndex));
            //        int newId = i + 137000;
            //        strIds.Add(id, newId);
            //        newData.Add(newId + " " + line.Substring(spIndex + 1));
            //        ++i;
            //    }
            //}

            //using (StreamWriter writer = new StreamWriter(File.Open(@"C:\Users\Petar\Desktop\SWConv\SW2.txt", FileMode.Create, FileAccess.Write, FileShare.Read)))
            //{
            //    for (int i = 0; i < newData.Count; ++i)
            //    {
            //        writer.WriteLine(newData[i]);
            //    }
            //}

            //int unitId = 37000;
            //XDocument doc = XDocument.Load(File.Open(@"C:\Users\Petar\Desktop\SWConv\SW.xml", FileMode.Open, FileAccess.Read, FileShare.Read));
            //foreach (XElement unit in doc.Root.Elements("unit"))
            //{
            //    XAttribute xid = unit.Attribute("id");
            //    xid.Value = unitId.ToString();
            //    unit.Element("dbid").Value = unitId.ToString();
            //    ++unitId;

            //    XElement xDispNameId = unit.Element("displaynameid");
            //    if (xDispNameId.Value.Contains("x") || xDispNameId.Value.Contains("X"))
            //    {
            //        continue;
            //    }

            //    int oldDispId = Convert.ToInt32(xDispNameId.Value);
            //    xDispNameId.Value = strIds[oldDispId].ToString();
            //}
            //doc.Save(File.Open(@"C:\Users\Petar\Desktop\SWConv\SW2.xml", FileMode.Create, FileAccess.Write, FileShare.Read));
        }

        private void GrnTesting()
        {
            GrnFile grnFile = new GrnFile();

            //@"C:\Users\Petar\Desktop\Nieuwe map (3)\Output"
            //bool areEq = this.AreEqual(AoMEngineLibrary.Graphics.Model.Matrix3x3.Identity, new AoMEngineLibrary.Graphics.Model.Matrix3x3(0.9999996f, 0f, 0f, 0f, 0.9999996f, 0f, 0f, 0f, 0.9999996f));
            //grnFile.DumpData(File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\ajax_17youmayfeel.grn", FileMode.Open, FileAccess.Read, FileShare.Read), @"C:\Users\Petar\Desktop\Nieuwe map (3)\Output");
            //grnFile.Read(File.Open(@"C:\Games\Steam\steamapps\common\Age of Mythology\models\ajax_17youmayfeel.grn", FileMode.Open, FileAccess.Read, FileShare.Read));
            //grnFile.Read(File.Open(@"C:\Users\Petar\Desktop\Nieuwe map (3)\AoM Grn\ajax_17youmayfeel.grn", FileMode.Open, FileAccess.Read, FileShare.Read));
            //grnFile.Read(File.Open(@"C:\Users\Petar\Desktop\Nieuwe map (3)\AoM Grn\ajaxC.grn", FileMode.Open, FileAccess.Read, FileShare.Read));
            //grnFile.Read(File.Open(@"C:\Users\Petar\Desktop\Nieuwe map (3)\AoM Grn\agamem_idlea.grn", FileMode.Open, FileAccess.Read, FileShare.Read));
            //grnFile.Write(File.Open(@"C:\Users\Petar\Desktop\Nieuwe map (3)\AoM Grn\ajaxC2.grn", FileMode.Create, FileAccess.Write, FileShare.Read));
            //grnFile.Meshes[0].CalculateUniqueMap();
        }

        private void OldDdtTesting()
        {
            //string texDir = @"C:\Games\Age of Mythology\textures\t1bar\textures";
            //string texDir = @"C:\Games\Age of Mythology\textures\t2bar\textures";
            string texDir = @"C:\Games\Steam\steamapps\common\Age of Mythology\textures";
            var sb = new StringBuilder();
            foreach (string s in Directory.GetFiles(texDir, "*.ddt", SearchOption.AllDirectories))
            {
                var ddt = new DdtFile(File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read));
                if (ddt.Format == DdtFormat.RgbDeflated)
                {
                    sb.AppendLine($"{s}\t{ddt.AlphaBits}");
                }
            }
            richTextBox1.Text = sb.ToString();
        }

        private void PrtTesting()
        {
            string texDir = @"C:\Games\Steam\steamapps\common\Age of Mythology\modelsfx\ttt";
            var sb = new StringBuilder();
            foreach (string s in Directory.GetFiles(texDir, "*.prt", SearchOption.AllDirectories))
            {
                try
                {
                    using (var fs = File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var prt = new PrtFile(fs);
                        if (prt.Version < 12)
                        {
                            sb.AppendLine($"{s}\t{prt.Version}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"{s}\t{ex}");
                    sb.AppendLine("==================================================================");
                }
            }
            richTextBox1.Text = sb.ToString();
        }

        private string GetAllDclsWithLight()
        {
            // dcl with light
            string output = ""; // should use StringBuilder instead
            string dclDir = @"C:\c\Argos\ptasev\Argos\current source\\modelsfx";
            foreach (string s in Directory.GetFiles(dclDir, "*.dcl", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    string fileName = Path.GetFileName(s);
                    string data = File.ReadAllText(s);
                    if (data.Contains("light"))
                    {
                        output += $"{fileName} {Environment.NewLine}";
                    }
                }
                catch (Exception ex)
                {
                    output += Path.GetFileName(s) + Environment.NewLine;
                    output += ex.Message + "\t---------------------------------------------------------------------------" + Environment.NewLine;
                }
            }

            return output;
        }

        private string OutputMtrlFilesThatDoNotMatchBrgData()
        {
            // Materials
            string output = ""; // should use StringBuilder instead
            bool outputFiles = false;
            string modelsDir = @"C:\c\Argos\ptasev\Argos\current source\models\version2.7";
            string mtrlDir = @"C:\c\Argos\ptasev\Argos\current source\materials";
            string mtrlOutDir = @"C:\Users\tasev\Desktop\mtrl27";
            foreach (string s in Directory.GetFiles(modelsDir, "*.brg", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    string fileName = Path.GetFileName(s);
                    BrgFile file = new BrgFile(File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read));

                    for (int i = 0; i < file.Materials.Count; ++i)
                    {
                        string mtrlFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + i + ".mtrl";
                        string mtrlFilePath = Path.Combine(mtrlDir, mtrlFileName);
                        string mtrlOutFilePath = Path.Combine(mtrlOutDir, mtrlFileName);

                        MtrlFile mtrl = new MtrlFile(file.Materials[i]);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            mtrl.Write(ms);

                            var newData = ms.ToArray();

                            byte[]? origData = File.Exists(mtrlFilePath) ? File.ReadAllBytes(mtrlFilePath) : null;
                            if (origData == null || !origData.SequenceEqual(newData))
                            {
                                output += $"{fileName} {Environment.NewLine}";

                                if (outputFiles)
                                {
                                    using (var fs = File.Open(mtrlOutFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                                        mtrl.Write(fs);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    output += Path.GetFileName(s) + Environment.NewLine;
                    output += ex.Message + "\t---------------------------------------------------------------------------" + Environment.NewLine;
                }
            }

            return output;
        }

        private string CompareBrgsInTwoDirsAndOutputInThird()
        {
            // Models brg compare
            string output = ""; // should use StringBuilder instead
            bool outputFiles = false;
            string brgDir = @"C:\Users\tasev\Desktop\brgCompare\orig";
            string brgDir2 = @"C:\c\Argos\ptasev\Argos\current source\models\version2.0";
            string outDir3 = @"C:\Users\tasev\Desktop\brgCompare\new";
            foreach (string s in Directory.GetFiles(brgDir, "*.brg", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    string fileName = Path.GetFileName(s);

                    string eeFile = Path.Combine(brgDir2, fileName);
                    if (File.Exists(eeFile))
                    {
                        byte[] origData = File.ReadAllBytes(s);
                        byte[] eeData = File.ReadAllBytes(eeFile);

                        if (!origData.SequenceEqual(eeData))
                        {
                            output += $"{fileName} {Environment.NewLine}";

                            // Copy to output
                            if (outputFiles)
                            {
                                string outputTgaPath = Path.Combine(outDir3, fileName);
                                File.Copy(s, outputTgaPath, true);

                                BrgFile file = new BrgFile(File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read));
                                for (int i = 0; i < file.Materials.Count; ++i)
                                {
                                    string mtrlFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + i + ".mtrl";
                                    string mtrlOutFilePath = Path.Combine(outDir3, mtrlFileName);

                                    MtrlFile mtrl = new MtrlFile(file.Materials[i]);
                                    using (var fs = File.Open(mtrlOutFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                                    {
                                        mtrl.Write(fs);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        output += $"{eeFile} File not found! {Environment.NewLine}";
                    }
                }
                catch (Exception ex)
                {
                    output += Path.GetFileName(s) + Environment.NewLine;
                    output += ex.Message + "\t---------------------------------------------------------------------------" + Environment.NewLine;
                }
            }

            return output;
        }

        private string CompareDdtsInTwoDirsAndOutputInThird()
        {
            // Textures tga compare
            string output = ""; // should use StringBuilder instead
            bool outputFiles = false;
            string texdir = @"C:\Users\tasev\Desktop\tgaCompare\orig";
            string tgaDir2 = @"C:\c\Argos\ptasev\Argos\current source\textures_source";
            string outDir2 = @"C:\Users\tasev\Desktop\tgaCompare\new";
            foreach (string s in Directory.GetFiles(texdir, "*.tga", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    string fileName = Path.GetFileName(s);
                    string btiFileName = $"{Path.GetFileNameWithoutExtension(fileName)}.bti";

                    string eeFile = Path.Combine(tgaDir2, fileName);
                    if (File.Exists(eeFile))
                    {
                        byte[] origData = File.ReadAllBytes(s);
                        byte[] eeData = File.ReadAllBytes(eeFile);

                        if (!origData.SequenceEqual(eeData))
                        {
                            output += $"{fileName} {Environment.NewLine}";

                            // Copy to output
                            if (outputFiles)
                            {
                                string outputTgaPath = Path.Combine(outDir2, fileName);
                                string inputBtiPath = Path.Combine(tgaDir2, btiFileName);
                                string outputBtiPath = Path.Combine(outDir2, btiFileName);
                                File.Copy(s, outputTgaPath, true);
                                File.Copy(inputBtiPath, outputBtiPath, true);
                            }
                        }
                    }
                    else
                    {
                        output += $"{eeFile} File not found! {Environment.NewLine}";
                    }
                }
                catch (Exception ex)
                {
                    output += Path.GetFileName(s) + Environment.NewLine;
                    output += ex.Message + "\t---------------------------------------------------------------------------" + Environment.NewLine;
                }
            }

            return output;
        }

        private string CompareTexturesOrigToEEAndTheirBtis()
        {
            // Can't remember exactly what I was doing here
            // Something to do with compare TT to EE textures and their texture info (bti) files

            // Textures
            string output = ""; // should use StringBuilder instead
            bool outputFiles = false;
            string eedir = @"C:\c\Argos\ptasev\Argos\current source\textures";
            string tgaDir = @"C:\c\Argos\ptasev\Argos\current source\textures_source";
            string outDir = @"C:\Users\tasev\Desktop\tex2out";
            foreach (string s in Directory.GetFiles(@"C:\Games\Age of Mythology\textures\t2extracted", "*.ddt", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    string fileName = Path.GetFileName(s);
                    DdtFile ddt = new DdtFile(File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read));

                    if ((ddt.Properties & DdtProperty.CubeMap) == DdtProperty.CubeMap)
                    {
                        continue;
                    }

                    DdtFormat convFormat;
                    switch (ddt.Format)
                    {
                        case DdtFormat.Dxt1:
                            if (ddt.AlphaBits > 0)
                            {
                                convFormat = DdtFormat.BC3;
                            }
                            else
                            {
                                convFormat = DdtFormat.BC3;
                            }
                            break;
                        case DdtFormat.Dxt1Alpha:
                            if (ddt.AlphaBits > 0)
                            {
                                convFormat = DdtFormat.BC3;
                            }
                            else
                            {
                                convFormat = DdtFormat.BC3;
                            }
                            break;
                        case DdtFormat.Dxt3Swizzled:
                            if (ddt.AlphaBits > 0)
                            {
                                convFormat = DdtFormat.BC3;
                            }
                            else
                            {
                                convFormat = DdtFormat.BC3;
                            }
                            break;
                        case DdtFormat.BT8:
                            if (ddt.AlphaBits > 0)
                            {
                                convFormat = DdtFormat.BC3;
                            }
                            else
                            {
                                convFormat = DdtFormat.BC3;
                            }
                            break;
                        default:
                            convFormat = ddt.Format;
                            break;
                    }

                    DdtProperty convProperties = ddt.Properties & ~DdtProperty.NoLowDetail;

                    // Create manifest and save tga to new folder
                    string formatString;
                    switch (convFormat)
                    {
                        case DdtFormat.Dxt1:
                            formatString = "BC1";
                            break;
                        case DdtFormat.BC2:
                            formatString = "BC2";
                            break;
                        case DdtFormat.BC3:
                            formatString = "BC3";
                            break;
                        case DdtFormat.RgbaDeflated:
                            formatString = "DeflatedRGBA8";
                            break;
                        case DdtFormat.RgbDeflated:
                            formatString = "DeflatedRGB8";
                            break;
                        case DdtFormat.RgDeflated:
                            formatString = "DeflatedRG8";
                            break;
                        case DdtFormat.AlphaDeflated:
                            formatString = "DeflatedR8";
                            break;
                        default:
                            throw new NotImplementedException($"DDT format {convFormat} is not implemented!");
                    }
                    string[] texInfoData = new[]
                    {
                        $"alpha={ddt.AlphaBits}",
                        $"{(ddt.Properties.HasFlag(DdtProperty.NoAlphaTest) ? "noalphatest" : string.Empty)}",
                        $"{(ddt.NumMipLevels == 0 ? "nomips" : string.Empty)}",
                        $"fmt={formatString}"
                    };
                    string manifest = string.Join(" ", texInfoData.Where(ti => !string.IsNullOrWhiteSpace(ti)));

                    string ddteeFile = Path.Combine(eedir, fileName);
                    if (File.Exists(ddteeFile))
                    {
                        DdtFile ddt2 = new DdtFile(File.Open(ddteeFile, FileMode.Open, FileAccess.Read, FileShare.Read));
                        if (convFormat != ddt2.Format ||
                            ddt.AlphaBits != ddt2.AlphaBits ||
                            convProperties != ddt2.Properties)
                        {
                            output += $"{fileName} {ddt.Properties} {ddt.Format} {ddt.AlphaBits} {Environment.NewLine}";
                            output += $"{fileName} {ddt2.Properties} {ddt2.Format} {ddt2.AlphaBits} {Environment.NewLine}";

                            // Copy to output
                            if (outputFiles)
                            {
                                string tgaFilename = Path.GetFileNameWithoutExtension(fileName) + ".tga";
                                string btiFilename = Path.GetFileNameWithoutExtension(fileName) + ".bti";
                                string tgaPath = Path.Combine(tgaDir, tgaFilename);
                                string outputTgaPath = Path.Combine(outDir, tgaFilename);
                                string outputBtiPath = Path.Combine(outDir, btiFilename);
                                File.WriteAllText(outputBtiPath, manifest);
                                File.Copy(tgaPath, outputTgaPath, true);
                            }
                        }
                    }
                    else
                    {
                        output += $"{fileName} {ddt.Properties} {ddt.Format} {ddt.AlphaBits} {Environment.NewLine}";
                        output += $"{ddteeFile} File not found! {Environment.NewLine}";

                        // Copy to output
                        if (outputFiles)
                        {
                            string tgaFilename = Path.GetFileNameWithoutExtension(fileName) + ".tga";
                            string btiFilename = Path.GetFileNameWithoutExtension(fileName) + ".bti";
                            string tgaPath = Path.Combine(tgaDir, tgaFilename);
                            string outputTgaPath = Path.Combine(outDir, tgaFilename);
                            string outputBtiPath = Path.Combine(outDir, btiFilename);
                            File.Copy(tgaPath, outputTgaPath, true);
                            File.WriteAllText(outputBtiPath, manifest);
                        }
                    }
                }
                catch (Exception ex)
                {
                    output += Path.GetFileName(s) + Environment.NewLine;
                    output += ex.Message + "\t---------------------------------------------------------------------------" + Environment.NewLine;
                }
            }

            return output;
        }

        private string FindAllMtrlFilesUsingMeteorBlastTexture()
        {
            // sfx e meteor blast mat check
            string output = ""; // should use StringBuilder instead
            string mtrlDir1 = @"C:\c\Argos\ptasev\Argos\current source\materials";
            foreach (string s in Directory.GetFiles(mtrlDir1, "*.mtrl", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    string fileName = Path.GetFileName(s);
                    MtrlFile mtrl = new MtrlFile();
                    using (FileStream fs = File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        mtrl.Read(fs);
                        if (mtrl.Texture.ToLower() == "SFX E Meteor Blast map".ToLower())
                        {
                            output += $"{fileName} {Environment.NewLine}";
                        }
                    }
                }
                catch (Exception ex)
                {
                    output += Path.GetFileName(s) + Environment.NewLine;
                    output += ex.Message + "\t---------------------------------------------------------------------------" + Environment.NewLine;
                }
            }

            return output;
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
                //file.ReadBr3(File.Open(dlg.FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "brg files|*.brg";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                file?.Write(File.Open(dlg.FileName, FileMode.Create, FileAccess.Write, FileShare.Read));
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg2 = new SaveFileDialog();
            dlg2.Filter = "br3 files|*.br3";

            if (dlg2.ShowDialog() == DialogResult.OK)
            {
                //file.WriteBr3(File.Open(dlg2.FileName, FileMode.Create, FileAccess.Write, FileShare.Read));
            }
        }
    }
}
