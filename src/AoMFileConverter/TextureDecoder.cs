using AoMEngineLibrary.Graphics;
using AoMEngineLibrary.Graphics.Ddt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Tga;
using System;
using System.IO;

namespace AoMFileConverter
{
    public static class TextureDecoder
    {
        private static readonly DdtImageConverter converter = new DdtImageConverter();
        private static readonly TgaEncoder tgaEncoder;

        static TextureDecoder()
        {
            tgaEncoder = new TgaEncoder();
            tgaEncoder.BitsPerPixel = TgaBitsPerPixel.Pixel32;
        }

        public static void Decode(string filePath, string outputDirectoryPath)
        {
            CubFile cub = new CubFile();
            DdtFile ddt = new DdtFile();
            ddt.Read(filePath);

            Texture tex = converter.Convert(ddt);

            // write main image from each face
            string fileName = Path.GetFileName(filePath);
            string baseOutputFilePath = Path.Combine(outputDirectoryPath, $"{fileName}");
            for (int i = 0; i < tex.Images.Length; ++i)
            {
                string outputFilePath;
                if (tex.Images.Length > 1)
                {
                    string fn = $"{fileName}_{i}";
                    outputFilePath = Path.Combine(outputDirectoryPath, fn);
                    cub.FileNames[i] = fn;
                }
                else
                    outputFilePath = baseOutputFilePath;

                var img = tex.Images[i][0];
                img.SaveAsTga(outputFilePath + ".tga", tgaEncoder);
            }

            if (tex.Images.Length > 1)
            {
                cub.Write(baseOutputFilePath + ".cub");
            }

            BtiFile bti = ddt.GetTextureInfo();
            bti.Write(baseOutputFilePath + ".bti");
        }
    }
}
