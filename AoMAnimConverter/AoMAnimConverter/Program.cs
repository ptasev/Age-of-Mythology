using AoMTextEditor;
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
                if (args.Length == 1)
                {
                    if (!File.Exists(args[0]))
                    {
                        throw new Exception("The file does not exist!");
                    }

                    if (Path.GetExtension(args[0]) == ".xml")
                    {
                        AnimFile.ConvertToAnim(File.Open(args[0], FileMode.Open, FileAccess.Read, FileShare.Read), File.Open(args[0] + ".txt", FileMode.Create, FileAccess.Write, FileShare.Read));
                    }
                    else
                    {
                        AnimFile.ConvertToXml(File.Open(args[0], FileMode.Open, FileAccess.Read, FileShare.Read), File.Open(args[0] + ".xml", FileMode.Create, FileAccess.Write, FileShare.Read));
                    }

                    Console.WriteLine("Conversion Successful!");
                }
                else
                {
                    Console.WriteLine("Please drag and drop an XML or TXT file onto the application executable!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured:");
                Console.WriteLine(ex.Message);
            }

            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
