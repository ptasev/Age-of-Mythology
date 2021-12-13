using AoMEngineLibrary.Anim;
using AoMEngineLibrary.Data.Bar;
using AoMEngineLibrary.Data.XmbFile;
using AoMEngineLibrary.Extensions;
using AoMEngineLibrary.Graphics.Brg;
using AoMEngineLibrary.Graphics.Prt;
using System;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace AoMFileConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("--- " + Properties.Resources.AppTitleLong + " ---");
                if (args.Length == 0)
                {
                    Console.WriteLine("No input arguments were found!");
                    Console.WriteLine($"Drag and drop one or more files on the EXE to convert.{Environment.NewLine}Supported: anim.txt, ddt, bti, cub, prt, mtrl, brg");
                }

                foreach (var f in args)
                {
                    try
                    {
                        Console.WriteLine("Processing " + Path.GetFileName(f) + "...");

                        Convert(f);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to convert the file!");
                        Console.WriteLine(ex.ToString());
                    }
                    finally
                    {
                        Console.WriteLine();
                        Console.WriteLine();
                    }
                }
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
            }
        }


        private static void Convert(string f)
        {
            var magic = string.Empty;
            var isDirectory = Directory.Exists(f);
            if (!isDirectory)
            {
                using (var fs = File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using var reader = new BinaryReader(fs);
                    magic = reader.ReadStringOfLength(4);
                }
            }

            if (f.EndsWith("anim.txt"))
            {
                AnimFile.ConvertToXml(File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read), File.Open(f + ".xml", FileMode.Create, FileAccess.Write, FileShare.Read));
                Console.WriteLine("Success! Anim converted.");
            }
            else if (f.EndsWith(".ddt"))
            {
                TextureDecoder.Decode(f, Path.GetDirectoryName(f) ?? string.Empty);
                Console.WriteLine("Success! Ddt converted.");
            }
            else if (f.EndsWith(".bti"))
            {
                TextureEncoder.Encode(f, Path.GetDirectoryName(f) ?? string.Empty);
                Console.WriteLine("Success! Bti converted.");
            }
            else if (f.EndsWith(".cub"))
            {
                TextureEncoder.Encode(f, Path.GetDirectoryName(f) ?? string.Empty);
                Console.WriteLine("Success! Cub converted.");
            }
            else if (f.EndsWith(".prt"))
            {
                using (var fs = File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var fso = File.Open(f + ".xml", FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    var file = new PrtFile(fs);
                    file.SerializeAsXml(fso);
                }
                Console.WriteLine("Success! Prt converted.");
            }
            else if (f.EndsWith(".xmb"))
            {
                using (var fs = File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var fso = File.Open(f + ".xml", FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var xw = XmlWriter.Create(fso, XmbFile.XmlWriterSettings))
                {
                    var xml = XmbFile.Load(fs);
                    xml.Save(xw);
                }
                Console.WriteLine("Success! Xmb converted.");
            }
            else if (f.EndsWith(".bar"))
            {
                using var fs = File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var bar = BarFile.Open(fs, false);
                var outDir = f;
                var dotIndex = outDir.LastIndexOf('.');
                if (dotIndex != -1)
                {
                    outDir = outDir.Remove(dotIndex) + "bar";
                    bar.ExtractToDirectory(outDir, true);
                    Console.WriteLine("Success! Bar file extracted.");
                }
                else
                {
                    Console.WriteLine("Failed to compute output file name.");
                }
            }
            else if (isDirectory)
            {
                using (var bar = new BarFile())
                using (var fso = File.Open(f + ".bar", FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    bar.AddEntriesFromDirectory(f);
                    bar.Write(fso, false);
                }
                Console.WriteLine("Success! Bar file created.");
            }
            else if (magic == "MTRL")
            {
                using (var fs = File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var fso = File.Open(f + ".xml", FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    var file = new MtrlFile();
                    file.Read(fs);
                    file.SerializeAsXml(fso);
                }
                Console.WriteLine("Success! Mtrl converted.");
            }
            else if (magic == "BANG")
            {
                var brgMtrlOutputPath = Path.Combine(Path.GetDirectoryName(f) ?? string.Empty, "materials");
                if (!Directory.Exists(brgMtrlOutputPath))
                {
                    Directory.CreateDirectory(brgMtrlOutputPath);
                }

                var file = new BrgFile(File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read));
                for (var i = 0; i < file.Materials.Count; ++i)
                {
                    var mtrl = new MtrlFile(file.Materials[i]);
                    using (var fs = File.Open(Path.Combine(brgMtrlOutputPath, Path.GetFileNameWithoutExtension(f) + "_" + i + ".mtrl"),
                        FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        mtrl.Write(fs);
                    }
                }
                Console.WriteLine("Success! Mtrl files created.");
            }
            else
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(f);

                if (xmlDoc?.DocumentElement?.Name == "AnimFile")
                {
                    AnimFile.ConvertToAnim(File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read), File.Open(f + ".txt", FileMode.Create, FileAccess.Write, FileShare.Read));
                    Console.WriteLine("Success! Anim converted.");
                }
                else if (xmlDoc?.DocumentElement?.Name == "ParticleFile")
                {
                    using (var fs = File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (var fso = File.Open(f + ".prt", FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        var file = PrtFile.DeserializeAsXml(fs);
                        file.Write(fso);
                    }
                    Console.WriteLine("Success! Prt converted.");
                }
                else if (xmlDoc?.DocumentElement?.Name == "Material")
                {
                    using (var fs = File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (var fso = File.Open(f + ".mtrl", FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        var file = MtrlFile.DeserializeAsXml(fs);
                        file.Write(fso);
                    }
                    Console.WriteLine("Success! Mtrl converted.");
                }
                else
                {
                    using (var fs = File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (var fso = File.Open(f + ".xmb", FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        XmbFile.Save(fs, fso, CompressionLevel.SmallestSize);
                    }
                    Console.WriteLine("Success! Xmb converted.");
                }
            }
        }
    }
}
