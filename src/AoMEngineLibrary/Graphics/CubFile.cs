using System;
using System.IO;

namespace AoMEngineLibrary.Graphics
{
    public class CubFile
    {
        public string[] FileNames { get; }

        public string UpFileName
        {
            get => FileNames[0];
            set { FileNames[0] = value; }
        }

        public string DownFileName
        {
            get => FileNames[1];
            set { FileNames[1] = value; }
        }

        public string FrontFileName
        {
            get => FileNames[2];
            set { FileNames[2] = value; }
        }

        public string BackFileName
        {
            get => FileNames[3];
            set { FileNames[3] = value; }
        }

        public string LeftFileName
        {
            get => FileNames[4];
            set { FileNames[4] = value; }
        }

        public string RightFileName
        {
            get => FileNames[5];
            set { FileNames[5] = value; }
        }

        public CubFile()
        {
            FileNames = new string[6];
        }

        public void Read(Stream stream)
        {
            using (var r = new StreamReader(stream))
            {
                int index = 0;
                while (r.Peek() >= 0)
                {
                    var line = r.ReadLine();

                    line = line.TrimStart();

                    if (line[0] == '/' && line[1] == '/')
                        continue;

                    line = line.TrimEnd();

                    if (line.Length <= 0)
                        continue;

                    FileNames[index++] = line;
                    if (index >= FileNames.Length)
                        break;
                }

                if (index < FileNames.Length)
                    throw new InvalidDataException("Expected 6 texture names in cub file.");
            }
        }

        public void Read(string filePath)
        {
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                Read(fs);
        }

        public void Write(Stream stream)
        {
            using (var w = new StreamWriter(stream))
            {
                w.WriteLine("// Up");
                w.WriteLine(UpFileName);

                w.WriteLine("// Down");
                w.WriteLine(DownFileName);

                w.WriteLine("// Front");
                w.WriteLine(FrontFileName);

                w.WriteLine("// Back");
                w.WriteLine(BackFileName);

                w.WriteLine("// Left");
                w.WriteLine(LeftFileName);

                w.WriteLine("// Right");
                w.Write(RightFileName);
            }
        }

        public void Write(string filePath)
        {
            using (var fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                Write(fs);
        }
    }
}
