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
            Texture tex;
            BtiFile bti;
            CubFile cub = new CubFile();
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                DdtFile ddt = new DdtFile();
                ddt.Read(fs);
                tex = converter.Convert(ddt);
                bti = ddt.GetTextureInfo();
            }

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
                //tgaEncoder.BitsPerPixel = img.PixelType.BitsPerPixel == 24 ? TgaBitsPerPixel.Pixel24 : TgaBitsPerPixel.Pixel32;
                img.SaveAsTga(outputFilePath + ".tga", tgaEncoder);
                bti.Write(outputFilePath + ".bti");
            }

            if (tex.Images.Length > 1)
            {
                cub.Write(baseOutputFilePath + ".cub");
            }
        }
    }
}
