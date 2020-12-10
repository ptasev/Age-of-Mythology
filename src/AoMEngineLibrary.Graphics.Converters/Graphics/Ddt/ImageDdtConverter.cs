using BCnEncoder.Encoder;
using BCnEncoder.Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoMEngineLibrary.Graphics.Ddt
{
    public class ImageDdtConverter
    {
        public DdtFile Convert(IReadOnlyList<Image> images, BtiFile bti)
        {
            if (images.Count != 1 && images.Count != 6)
            {
                throw new ArgumentException("There must be either 1 or 6 images to convert to ddt texture.", nameof(images));
            }

            int width = images.First().Width;
            int height = images.First().Height;
            if (images.Count > 1 && images.Skip(1).Any(i => i.Width != width || i.Height != height))
            {
                throw new ArgumentException("All images must be the same width and height", nameof(images));
            }
            if ((width & 0b11) != 0 || (height & 0b11) != 0)
            {
                throw new ArgumentException("The image width and height must be a multiple of 4", nameof(images));
            }

            DdtFile ddt = new DdtFile();
            ddt.Width = width;
            ddt.Height = height;
            ddt.Data = new byte[images.Count][][];
            bool shouldDeflate = ShouldDeflate(bti.Format);

            BcEncoder encoder = new BcEncoder();
            encoder.OutputOptions.generateMipMaps = bti.UseMips;
            encoder.OutputOptions.quality = CompressionQuality.BestQuality;
            encoder.OutputOptions.fileFormat = OutputFileFormat.Dds;
            encoder.OutputOptions.format = GetCompressionFormat(bti.Format, bti.AlphaBits);
            encoder.OutputOptions.ddsBc1WriteAlphaFlag = false;
            encoder.OutputOptions.maxMipMapLevel = GetMipLevels(width, height);

            for (int f = 0; f < images.Count; ++f)
            {
                var faceImage = images[f];
                faceImage.Mutate(p => p.Flip(FlipMode.Vertical));

                byte[][] encodedData;
                if (faceImage is Image<Rgba32>)
                {
                    encodedData = encoder.EncodeToRawBytes((Image<Rgba32>)faceImage).ToArray();
                }
                else
                {
                    var img = faceImage.CloneAs<Rgba32>();
                    encodedData = encoder.EncodeToRawBytes(img).ToArray();
                    img.Dispose();
                }

                if (shouldDeflate)
                {
                    for (int m = 0; m < encodedData.Length; ++m)
                    {
                        encodedData[m] = ZlibCompress(encodedData[m]);
                    }
                }

                ddt.Data[f] = encodedData;
            }

            ddt.MipMapLevels = (byte)ddt.Data[0].Length;
            if (ddt.Data.Length == 6) ddt.Properties |= DdtProperty.CubeMap;
            ddt.SetTextureInfo(bti);

            return ddt;
        }

        private static CompressionFormat GetCompressionFormat(BtiTextureFormat texFormat, byte alphaBits)
        {
            return texFormat switch
            {
                BtiTextureFormat.BC1 when alphaBits == 0 => CompressionFormat.BC1,
                BtiTextureFormat.BC1 when alphaBits == 1 => CompressionFormat.BC1WithAlpha,
                BtiTextureFormat.BC2 when alphaBits == 4 => CompressionFormat.BC2,
                BtiTextureFormat.BC3 when alphaBits == 1 => CompressionFormat.BC3,
                BtiTextureFormat.BC3 when alphaBits == 8 => CompressionFormat.BC3,
                BtiTextureFormat.DeflatedR8 when alphaBits == 0 => CompressionFormat.R,
                BtiTextureFormat.DeflatedRG8 when alphaBits == 0 => CompressionFormat.RG,
                BtiTextureFormat.DeflatedRGB8 when alphaBits == 0 => CompressionFormat.RGB,
                BtiTextureFormat.DeflatedRGBA8 when alphaBits == 1 => CompressionFormat.RGBA,
                BtiTextureFormat.DeflatedRGBA8 when alphaBits == 8 => CompressionFormat.RGBA,
                _ => throw new InvalidOperationException($"Cannot convert format {texFormat} with alpha bits {alphaBits} to ddt texture.")
            };
        }

        private static bool ShouldDeflate(BtiTextureFormat texFormat)
        {
            return texFormat switch
            {
                BtiTextureFormat.DeflatedR8 => true,
                BtiTextureFormat.DeflatedRG8 => true,
                BtiTextureFormat.DeflatedRGB8 => true,
                BtiTextureFormat.DeflatedRGBA8 => true,
                _ => false
            };
        }

        private static int GetMipLevels(int width, int height)
        {
            // smallest side should be no lower than 4
            uint side = (uint)Math.Min(width, height);

            int mipLevels = 1;
            while (side > 4)
            {
                side = side >> 1;
                ++mipLevels;
            }

            return mipLevels;
        }

        private static byte[] ZlibCompress(ReadOnlySpan<byte> data)
        {
            throw new NotImplementedException("Zlib compression is not implemented.");
        }
    }
}
