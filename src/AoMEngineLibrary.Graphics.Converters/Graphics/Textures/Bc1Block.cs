using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Numerics;

namespace AoMEngineLibrary.Graphics.Textures
{
    public struct Bc1Block : IBcBlock
    {
        public const int PixelsPerBlock = 16;

        public Bgr565 Color0;
        public Bgr565 Color1;
        public uint Bitmap;

        public void Decode(Span<Vector4> colors)
        {
            var color0 = Color0.ToVector4();
            var color1 = Color1.ToVector4();

            Vector4 color2, color3;
            if (Color0.PackedValue > Color1.PackedValue)
            {
                color2 = Vector4.Lerp(color0, color1, 1.0f / 3.0f);
                color3 = Vector4.Lerp(color0, color1, 2.0f / 3.0f);
            }
            else
            {
                color2 = Vector4.Lerp(color0, color1, 1.0f / 2.0f);
                color3 = Vector4.Zero;
            }

            uint indices = Bitmap;
            for (int i = 0; i < PixelsPerBlock; ++i, indices >>= 2)
            {
                switch (indices & 3u)
                {
                    case 0: colors[i] = color0; break;
                    case 1: colors[i] = color1; break;
                    case 2: colors[i] = color2; break;

                    case 3:
                    default: colors[i] = color3; break;
                }
            }
        }
    }
}
