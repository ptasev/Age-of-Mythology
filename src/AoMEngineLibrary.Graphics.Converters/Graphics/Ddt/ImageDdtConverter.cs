using BCnEncoder.Encoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

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
            encoder.OutputOptions.GenerateMipMaps = bti.UseMips;
            encoder.OutputOptions.Quality = CompressionQuality.BestQuality;
            encoder.OutputOptions.FileFormat = OutputFileFormat.Dds;
            encoder.OutputOptions.Format = GetCompressionFormat(bti.Format, bti.AlphaBits);
            encoder.OutputOptions.DdsBc1WriteAlphaFlag = false;
            encoder.OutputOptions.MaxMipMapLevel = GetMipLevels(width, height);

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

                // Swizzle R and B
                if (bti.Format == BtiTextureFormat.DeflatedRGB8)
                {
                    for (int m = 0; m < encodedData.Length; ++m)
                    {
                        var data = encodedData[m];
                        for (int i = 0; i < data.Length; i += 3)
                        {
                            var temp = data[i];
                            data[i] = data[i + 2];
                            data[i + 2] = temp;
                        }
                    }
                }
                else if (bti.Format == BtiTextureFormat.DeflatedRGBA8)
                {
                    for (int m = 0; m < encodedData.Length; ++m)
                    {
                        var data = encodedData[m];
                        for (int i = 0; i < data.Length; i += 4)
                        {
                            var temp = data[i];
                            data[i] = data[i + 2];
                            data[i + 2] = temp;
                        }
                    }
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
                BtiTextureFormat.BC1 when alphaBits == 0 => CompressionFormat.Bc1,
                BtiTextureFormat.BC1 when alphaBits == 1 => CompressionFormat.Bc1WithAlpha,
                BtiTextureFormat.BC2 when alphaBits == 4 => CompressionFormat.Bc2,
                BtiTextureFormat.BC3 when alphaBits == 1 => CompressionFormat.Bc3,
                BtiTextureFormat.BC3 when alphaBits == 8 => CompressionFormat.Bc3,
                // Commented these out since they're never used in the game, so better not to bother
                //BtiTextureFormat.DeflatedR8 when alphaBits == 0 => CompressionFormat.R,
                //BtiTextureFormat.DeflatedRG8 when alphaBits == 0 => CompressionFormat.RG,
                BtiTextureFormat.DeflatedRGB8 when alphaBits == 0 => CompressionFormat.Rgb,
                // Added this because I wasn't sure if 1 bit alpha meant alpha test
                BtiTextureFormat.DeflatedRGBA8 when alphaBits == 0 => CompressionFormat.Rgba,
                BtiTextureFormat.DeflatedRGBA8 when alphaBits == 1 => CompressionFormat.Rgba,
                BtiTextureFormat.DeflatedRGBA8 when alphaBits == 8 => CompressionFormat.Rgba,
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
            using (var mso = new MemoryStream())
            using (var dos = new ZLibStream(mso, CompressionLevel.Optimal))
            {
                dos.Write(data);
                dos.Flush();
                return mso.ToArray();
            }
        }
    }
}
