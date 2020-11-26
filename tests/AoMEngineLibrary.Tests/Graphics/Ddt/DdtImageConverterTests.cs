using AoMEngineLibrary.Graphics.Ddt;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace AoMEngineLibrary.Tests.Graphics.Ddt
{
    public class DdtImageConverterTests
    {
        [Fact]
        public void Convert_Dxt1()
        {
            // Arrange
            var endpoint0 = new Rgb24(32, 64, 128);
            var endpoint1 = new Rgb24(16, 32, 64);
            var data = BcBlockUtils.GenerateBc1Block(endpoint0, endpoint1,
                out var expCol0, out var expCol1, out var expIndices, out var expVectors);
            DdtFile ddt = new DdtFile();
            ddt.Properties = DdtProperty.Normal;
            ddt.Format = DdtFormat.Dxt1;
            ddt.AlphaBits = 0;
            ddt.MipMapLevels = 1;
            ddt.Width = 4;
            ddt.Height = 4;
            ddt.Data = new byte[1][][]
            {
                new byte[1][] { data }
            };
            var converter = new DdtImageConverter();

            // Act
            var tex = converter.Convert(ddt);
            var image = tex.Images[0][0] as Image<Rgb24>;

            // Assert
            Assert.NotNull(image);
            Assert.Equal(ddt.Width, image.Width);
            Assert.Equal(ddt.Height, image.Height);

            int index = 15; // img gets flipped so go backwards
            for (int row = 0; row < image.Height; ++row)
            {
                for (int col = 0; col < image.Width; ++col)
                {
                    var actual = image[col, row];
                    var exp = new Rgb24();
                    exp.FromVector4(expVectors[index]);
                    Assert.Equal(exp, actual);

                    --index;
                }
            }
        }
    }
}
