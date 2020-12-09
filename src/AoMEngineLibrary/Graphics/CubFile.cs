using System;
using System.IO;

namespace AoMEngineLibrary.Graphics
{
    public class CubFile
    {
        public readonly string[] FileNames;

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
