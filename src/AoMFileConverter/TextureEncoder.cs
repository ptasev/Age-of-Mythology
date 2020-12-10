using AoMEngineLibrary.Graphics;
using AoMEngineLibrary.Graphics.Ddt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var tgaFilePath = Path.ChangeExtension(filePath, ".tga");
            if (!File.Exists(tgaFilePath))
                throw new InvalidOperationException("The related tga file could not be found.");

            BtiFile bti = new BtiFile();
            bti.Read(filePath);

            var image = Image.Load<Rgba32>(tgaFilePath);
            var ddt = converter.Convert(new[] { image }, bti);

            var fileName = Path.GetFileName(filePath);
            var ddtFilePath = Path.Combine(outputDirectoryPath, fileName + ".ddt");
            ddt.Write(ddtFilePath);
        }

        private static void EncodeCub(string filePath, string outputDirectoryPath)
        {
            throw new NotImplementedException("Encoding cube maps is not yet implemented.");
        }
    }
}
