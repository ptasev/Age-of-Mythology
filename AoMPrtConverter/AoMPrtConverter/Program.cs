namespace AoMPrtConverter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AoMEngineLibrary.Graphics.Prt;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0 || !File.Exists(args[0]))
                {
                    Console.WriteLine("No input arguments were found!");
                    Console.WriteLine("Drag and drop a prt or xml file on the EXE to convert.");
                    return;
                }

                if (Path.GetExtension(args[0]) == ".prt")
                {
                    PrtFile file = new PrtFile(File.Open(args[0], FileMode.Open, FileAccess.Read, FileShare.Read));
                    file.SerializeAsXml(File.Open(args[0] + ".xml", FileMode.Create, FileAccess.Write, FileShare.Read));
                    Console.WriteLine("Success! Xml created.");
                }
                else if (Path.GetExtension(args[0]) == ".xml")
                {
                    PrtFile file = PrtFile.DeserializeAsXml(File.Open(args[0], FileMode.Open, FileAccess.Read, FileShare.Read));
                    file.Write(File.Open(args[0] + ".prt", FileMode.Create, FileAccess.Write, FileShare.Read));
                    Console.WriteLine("Success! Prt created.");
                }
                else
                {
                    Console.WriteLine("Invalid file extension!");
                    Console.WriteLine("Drag and drop a prt or xml file on the EXE to convert.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to convert the file!");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
            }
        }
    }
}
