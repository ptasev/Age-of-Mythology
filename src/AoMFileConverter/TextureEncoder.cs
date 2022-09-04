using AoMEngineLibrary.Graphics;
using AoMEngineLibrary.Graphics.Ddt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace AoMFileConverter
{
    public static class TextureEncoder
    {
        private static readonly ImageDdtConverter converter = new ImageDdtConverter();

        public static void Encode(string filePath, string outputDirectoryPath)
        {
            var ext = Path.GetExtension(filePath);
            if (ext == ".cub")
            {
                EncodeCub(filePath, outputDirectoryPath);
            }
            else if (ext == ".bti")
            {
                EncodeBti(filePath, outputDirectoryPath);
            }
            else
            {
                throw new ArgumentException("Texture to encode must be either .bti or .cub.", nameof(filePath));
            }
        }

        private static void EncodeBti(string filePath, string outputDirectoryPath)
        {
            var imageFilePath = GetImageFilePath(filePath);
            if (imageFilePath is null)
                throw new InvalidOperationException("The related tga file could not be found.");

            BtiFile bti = new BtiFile();
            bti.Read(filePath);

            var image = Image.Load<Rgba32>(imageFilePath);
            var ddt = converter.Convert(new[] { image }, bti);

            var fileName = Path.GetFileName(filePath);
            var ddtFilePath = Path.Combine(outputDirectoryPath, fileName + ".ddt");
            ddt.Write(ddtFilePath);
        }

        private static void EncodeCub(string filePath, string outputDirectoryPath)
        {
            var btiFilePath = Path.ChangeExtension(filePath, ".bti");
            if (!File.Exists(btiFilePath))
                throw new InvalidOperationException($"The related bti file could not be found.");

            var bti = new BtiFile();
            bti.Read(btiFilePath);

            CubFile cub = new CubFile();
            cub.Read(filePath);

            var images = new Image[6];
            string inputDirectoryPath = Path.GetDirectoryName(filePath) ?? string.Empty;
            for (int i = 0; i < cub.FileNames.Length; ++i)
            {
                var fileName = cub.FileNames[i];
                var imageFilePath = GetImageFilePath(Path.Combine(inputDirectoryPath, fileName + ".tga"));
                if (imageFilePath is null)
                    throw new InvalidOperationException($"The cub referenced tga file could not be found ({fileName}).");

                var image = Image.Load<Rgba32>(imageFilePath);
                images[i] = image;
            }

            var ddtFileName = Path.GetFileName(filePath);
            var ddtFilePath = Path.Combine(outputDirectoryPath, ddtFileName + ".ddt");

            var ddt = converter.Convert(images, bti);
            ddt.Write(ddtFilePath);
        }

        private static string? GetImageFilePath(string filePath)
        {
            var tgaFilePath = Path.ChangeExtension(filePath, ".tga");
            var pngFilePath = Path.ChangeExtension(filePath, ".png");

            if (File.Exists(tgaFilePath))
            {
                return tgaFilePath;
            }
            else if (File.Exists(pngFilePath))
            {
                return pngFilePath;
            }
            else
            {
                return null;
            }
        }
    }
}
