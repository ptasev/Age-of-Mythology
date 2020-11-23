using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Numerics;

namespace AoMEngineLibrary.Graphics.Textures
{
    public struct Bc2Block : IBcBlock
    {
        private const int PixelsPerBlock = 16;

        public ulong AlphaBits;
        public Bgr565 Color0;
        public Bgr565 Color1;
        public uint Bitmap;

        public void Decode(Span<Vector4> colors)
        {
            var color0 = Color0.ToVector4();
            var color1 = Color1.ToVector4();

            Vector4 color2 = Vector4.Lerp(color0, color1, 1.0f / 3.0f);
            Vector4 color3 = Vector4.Lerp(color0, color1, 2.0f / 3.0f);

            Vector4 color;
            ulong alphas = AlphaBits;
            uint indices = Bitmap;
            for (int i = 0; i < PixelsPerBlock; ++i, indices >>= 2, alphas >>= 4)
            {
                switch (indices & 3u)
                {
                    case 0: color = color0; break;
                    case 1: color = color1; break;
                    case 2: color = color2; break;
                    case 3:
                    default: color = color3; break;
                }

                color.W = (alphas & 0xf) * (1.0f / 15.0f);
                colors[i] = color;
            }
        }
    }
}
