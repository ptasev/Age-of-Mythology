using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace AoMEngineLibrary.Graphics.Textures
{
    /// <summary>
    /// A custom BC2 format with the alpha data after the colors, and using 4444 colors for the AoM game.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Bc2SwizzledBlock : IBcBlock
    {
        private const int PixelsPerBlock = 16;

        public Bgra4444 Color0;
        public Bgra4444 Color1;
        public ushort AlphaBits0;
        public byte Bitmap0;
        public ushort AlphaBits1;
        public byte Bitmap1;
        public ushort AlphaBits2;
        public byte Bitmap2;
        public ushort AlphaBits3;
        public byte Bitmap3;

        public void Decode(Span<Vector4> colors)
        {
            var color0 = Color0.ToVector4();
            var color1 = Color1.ToVector4();

            Vector4 color2 = Vector4.Lerp(color0, color1, 1.0f / 3.0f);
            Vector4 color3 = Vector4.Lerp(color0, color1, 2.0f / 3.0f);

            Vector4 color;
            ulong alphas = AlphaBits0 | ((ulong)AlphaBits1 << 16) | ((ulong)AlphaBits2 << 32) | ((ulong)AlphaBits3 << 48);
            uint indices = Bitmap0 | ((uint)Bitmap1 << 8) | ((uint)Bitmap2 << 16) | ((uint)Bitmap3 << 24);
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
