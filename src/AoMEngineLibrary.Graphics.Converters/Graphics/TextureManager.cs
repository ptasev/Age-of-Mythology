using AoMEngineLibrary.Graphics.Ddt;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AoMEngineLibrary.Graphics
{
    public class TextureManager
    {
        private readonly string _texturesPath;
        private readonly DdtImageConverter _ddtImageConverter;

        public TextureManager(string texturesPath)
        {
            _texturesPath = texturesPath;
            _ddtImageConverter = new DdtImageConverter();
        }

        public Image[][]? GetTexture(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (extension == string.Empty)
            {
                extension = ".ddt";
                fileName += ".ddt";
            }

            Image[][]? images;
            switch (extension)
            {
                case ".ddt":
                    images = GetDdtTexture(fileName);
                    break;
                default:
                    throw new NotImplementedException($"Support for {extension} textures is not implemented.");
            }

            if (images?.Length == 0 || images?[0]?.Length == 0)
            {
                return null;
            }

            return images;
        }

        private Image[][]? GetDdtTexture(string fileName)
        {
            string filePath = Path.Combine(_texturesPath, fileName);

            if (!File.Exists(filePath))
                return null;

            var ddt = new DdtFile();
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                ddt.Read(fs);

            var images = _ddtImageConverter.Convert(ddt);

            return images;
        }
    }
}
