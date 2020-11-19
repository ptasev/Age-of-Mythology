using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace AoMEngineLibrary.Graphics
{
    public class Texture
    {
        public int Width { get; }

        public int Height { get; }

        public bool AlphaTest { get; }

        public Image[][] Images { get; }

        public Texture(Image[][] images, bool alphaTest)
        {
            Images = images;
            AlphaTest = alphaTest;

            if (images.Length > 0 && images[0].Length > 0)
            {
                var img = images[0][0];
                Width = img.Width;
                Height = img.Height;
            }
        }
    }
}
