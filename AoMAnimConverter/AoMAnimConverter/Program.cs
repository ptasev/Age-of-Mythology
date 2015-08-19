using AoMEngineLibrary.Anim;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoMAnimConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("--- AAC 1.0 ---");
                if (args.Length == 0)
                {
                    Console.WriteLine("No input arguments were found!");
                    Console.WriteLine("Drag and drop an anim or xml file on the EXE to convert.");
                }

                foreach (string f in args)
                {
                    try
                    {
                        Console.WriteLine("Processing " + Path.GetFileName(f) + "...");

                        if (Path.GetExtension(f) == ".txt")
                        {
                            AnimFile.ConvertToXml(File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read), File.Open(f + ".xml", FileMode.Create, FileAccess.Write, FileShare.Read));
                            Console.WriteLine("Success! Xml created.");
                        }
                        else if (Path.GetExtension(f) == ".xml")
                        {
                            AnimFile.ConvertToAnim(File.Open(f, FileMode.Open, FileAccess.Read, FileShare.Read), File.Open(f + ".txt", FileMode.Create, FileAccess.Write, FileShare.Read));
                            Console.WriteLine("Success! Anim created.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid file extension!");
                            Console.WriteLine("Drag and drop an anim or xml file on the EXE to convert.");
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
