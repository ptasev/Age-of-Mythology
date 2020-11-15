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

        /// <summary>
        /// Finds the texture path based on the name of the file.
        /// </summary>
        /// <param name="fileName">The file name of the texture to find.</param>
        /// <returns>The file path of the texture, or empty string if the texture does not exist.</returns>
        public string GetTexturePath(string fileName)
        {
            string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);

            // for now just check the main textures directory, and only for ddt files
            // in the future need to support tga, btx and cub files, and determine which is the latest
            var ddtPath = Path.Combine(_texturesPath, $"{fileNameNoExt}.ddt");
            if (File.Exists(ddtPath))
            {
                return ddtPath;
            }

            return string.Empty;
        }

        public Image[][] GetTexture(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The texture was not found.", filePath);

            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            Image[][] images;
            switch (extension)
            {
                case ".ddt":
                    images = LoadDdtTexture(filePath);
                    break;
                default:
                    throw new NotImplementedException($"Support for \"{extension}\" textures is not implemented.");
            }

            if (images.Length == 0 || images[0].Length == 0)
            {
                throw new InvalidDataException("Failed to load texture data from the file.");
            }

            return images;
        }

        private Image[][] LoadDdtTexture(string filePath)
        {
            var ddt = new DdtFile();
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                ddt.Read(fs);

            var images = _ddtImageConverter.Convert(ddt);

            return images;
        }
    }
}
