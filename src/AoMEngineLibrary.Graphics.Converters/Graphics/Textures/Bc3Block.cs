using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace AoMEngineLibrary.Graphics.Textures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Bc3Block : IBcBlock
    {
        private const int PixelsPerBlock = 16;

        public byte AlphaBits0;
        public byte AlphaBits1;
        public uint AlphaIndices0;
        public ushort AlphaIndices1;
        public Bgr565 Color0;
        public Bgr565 Color1;
        public uint Bitmap;

        public void Decode(Span<Vector4> colors)
        {
            var color0 = Color0.ToVector4();
            var color1 = Color1.ToVector4();

            Vector4 color2 = Vector4.Lerp(color0, color1, 1.0f / 3.0f);
            Vector4 color3 = Vector4.Lerp(color0, color1, 2.0f / 3.0f);

            // Adaptive 3-bit alpha part
            Span<float> alphas = stackalloc float[8];
            alphas[0] = AlphaBits0 * (1.0f / 255.0f);
            alphas[1] = AlphaBits1 * (1.0f / 255.0f);
            if (AlphaBits0 > AlphaBits1)
            {
                for (int i = 1; i < 7; ++i)
                    alphas[i + 1] = (alphas[0] * (7 - i) + alphas[1] * i) * (1.0f / 7.0f);
            }
            else
            {
                for (int i = 1; i < 5; ++i)
                    alphas[i + 1] = (alphas[0] * (5 - i) + alphas[1] * i) * (1.0f / 5.0f);

                alphas[6] = 0.0f;
                alphas[7] = 1.0f;
            }

            Vector4 color;
            ulong alphaIndices = AlphaIndices0 | ((ulong)AlphaIndices1 << 32);
            uint indices = Bitmap;
            for (int i = 0; i < PixelsPerBlock; ++i, indices >>= 2, alphaIndices >>= 3)
            {
                switch (indices & 3u)
                {
                    case 0: color = color0; break;
                    case 1: color = color1; break;
                    case 2: color = color2; break;
                    case 3:
                    default: color = color3; break;
                }

                color.W = alphas[(int)alphaIndices & 0x7];
                colors[i] = color;
            }
        }
    }
}
