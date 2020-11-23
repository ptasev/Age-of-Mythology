using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace AoMEngineLibrary.Graphics.Textures
{
    /// <summary>
    /// A custom BC1 format for the AoM game.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Bc1CustomAlphaBlock : IBcBlock
    {
        private const int PixelsPerBlock = 16;

        public Bgra5551 Color0;
        public Bgra5551 Color1;
        public ushort AlphaBits;
        public uint Bitmap;

        public void Decode(Span<Vector4> colors)
        {
            var color0 = Color0.ToVector4();
            var color1 = Color1.ToVector4();

            Vector4 color2, color3;
            color2 = Vector4.Lerp(color0, color1, 1.0f / 3.0f);
            color3 = Vector4.Lerp(color0, color1, 2.0f / 3.0f);

            Vector4 color;
            uint alphas = AlphaBits;
            uint indices = Bitmap;
            for (int i = 0; i < PixelsPerBlock; ++i, indices >>= 2, alphas >>= 1)
            {
                switch (indices & 3u)
                {
                    case 0: color = color0; break;
                    case 1: color = color1; break;
                    case 2: color = color2; break;
                    case 3:
                    default: color = color3; break;
                }

                // not sure if I should combine the color.W bit with alphas bit, don't for now
                color.W = (alphas) & 0b1u;
                colors[i] = color;
            }
        }
    }
}
