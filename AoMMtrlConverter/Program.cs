namespace AoMMtrlConverter
{
    using AoMEngineLibrary.Graphics.Brg;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("--- AMC 1.0 ---");
                if (args.Length == 0)
                {
                    Console.WriteLine("No input arguments were found!");
                    Console.WriteLine("Drag and drop a mtrl, xml, or brg file on the EXE to convert.");
                }

                foreach (string f in args)
                {
                    try
                    {
                        Console.WriteLine("Processing " + Path.GetFileName(f) + "...");

                        if (Path.GetExtension(f) == ".mtrl")
                        {
                            MtrlFile file = new MtrlFile();
                            file.Read(File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read));
                            file.SerializeAsXml(File.Open(f + ".xml", FileMode.Create, FileAccess.Write, FileShare.Read));
                            Console.WriteLine("Success! Xml created.");
                        }
                        else if (Path.GetExtension(f) == ".xml")
                        {
                            MtrlFile file = MtrlFile.DeserializeAsXml(File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read));
                            file.Write(File.Open(f + ".mtrl", FileMode.Create, FileAccess.Write, FileShare.Read));
                            Console.WriteLine("Success! Mtrl created.");
                        }
                        else if (Path.GetExtension(f) == ".brg")
                        {
                            string brgMtrlOutputPath = Path.Combine(Path.GetDirectoryName(f), "materials");
                            if (!Directory.Exists(brgMtrlOutputPath))
                            {
                                Directory.CreateDirectory(brgMtrlOutputPath);
                            }

                            BrgFile file = new BrgFile(File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read));
                            for (int i = 0; i < file.Materials.Count; ++i)
                            {
                                MtrlFile mtrl = new MtrlFile(file.Materials[i]);
                                mtrl.Write(File.Open(Path.Combine(brgMtrlOutputPath, Path.GetFileNameWithoutExtension(f) + "_" + i + ".mtrl"), FileMode.Create, FileAccess.Write, FileShare.Read));
                            }
                            Console.WriteLine("Success! Mtrl files created.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid file extension!");
                            Console.WriteLine("Drag and drop a mtrl, xml, or brg file on the EXE to convert.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to convert the file!");
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
            }
        }
    }
}

