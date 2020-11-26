using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Buffers.Binary;
using System.Numerics;

namespace AoMEngineLibrary.Tests.Graphics
{
    public static class BcBlockUtils
    {
        public static byte[] GenerateBc1Block(Rgb24 color0, Rgb24 color1, out Bgr565 col0, out Bgr565 col1, out uint indices, out Vector4[] vectors)
        {
            col0 = new Bgr565();
            col0.FromRgb24(color0);
            col1 = new Bgr565();
            col1.FromRgb24(color1);

            var dest = new byte[8];
            Span<byte> destSpan = dest.AsSpan();
            BinaryPrimitives.WriteUInt16LittleEndian(destSpan, col0.PackedValue);
            BinaryPrimitives.WriteUInt16LittleEndian(destSpan.Slice(2), col1.PackedValue);

            // First half of pixels use col0, second half col1
            const int pixels = 16;
            const int halfwayPoint = pixels / 2;

            // Set the indices, 2bpp
            vectors = new Vector4[pixels];
            int shift = 0;
            indices = 0;
            for (int i = 0; i < pixels; ++i, shift += 2)
            {
                if (i < halfwayPoint)
                {
                    // use col0 in the first half
                    indices |= (0u << shift);
                    vectors[i] = col0.ToVector4();
                }
                else
                {
                    // use col1 in the second half
                    indices |= (1u << shift);
                    vectors[i] = col1.ToVector4();
                }
            }

            BinaryPrimitives.WriteUInt32LittleEndian(destSpan.Slice(4), indices);
            return dest;
        }
    }
}
