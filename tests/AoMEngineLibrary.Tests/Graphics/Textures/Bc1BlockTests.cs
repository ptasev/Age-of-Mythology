using AoMEngineLibrary.Graphics.Textures;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace AoMEngineLibrary.Tests.Graphics.Textures
{
    public class Bc1BlockTests
    {
        [Fact]
        public void Decode_Works()
        {
            // Arrange
            var endpoint0 = new Rgb24(32, 64, 128);
            var endpoint1 = new Rgb24(16, 32, 64);
            var data = BcBlockUtils.GenerateBc1Block(endpoint0, endpoint1,
                out var expCol0, out var expCol1, out var expIndices, out var expVectors);
            var decoder = MemoryMarshal.Cast<byte, Bc1Block>(data)[0];

            // Act
            var vectors = new Vector4[Bc1Block.PixelsPerBlock];
            decoder.Decode(vectors);

            // Assert
            Assert.Equal(expCol0, decoder.Color0);
            Assert.Equal(expCol1, decoder.Color1);
            Assert.Equal(expIndices, decoder.Bitmap);
            Assert.Equal(expVectors, vectors);
        }
    }
}
