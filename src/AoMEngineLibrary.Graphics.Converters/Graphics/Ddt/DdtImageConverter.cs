using AoMEngineLibrary.Graphics.Textures;
using Microsoft.Toolkit.HighPerformance.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;

namespace AoMEngineLibrary.Graphics.Ddt
{
    public class DdtImageConverter
    {
        public DdtImageConverter()
        {
        }

        public Texture Convert(DdtFile ddt)
        {
            Image[][] images;

            if (ddt.Format == DdtFormat.Dxt1 && ddt.AlphaBits == 0)
            {
                images = DecodeBlockCompression<Bc1Block, Rgb24>(ddt);
            }
            else if (ddt.Format == DdtFormat.Dxt1 && ddt.AlphaBits == 1)
            {
                // AoMTT does not support DXT1 textures with 1-bit alpha
                // AoMEE supports DXT1 textures with 1-bit alpha
                images = DecodeBlockCompression<Bc1Block, Rgba32>(ddt);
            }
            else if (ddt.Format == DdtFormat.Dxt1Alpha && ddt.AlphaBits == 1)
            {
                // AoMTT uses this custom DXT1 based format for 1-bit alpha
                images = DecodeBlockCompression<Bc1CustomAlphaBlock, Rgba32>(ddt);
            }
            else if (ddt.Format == DdtFormat.BC2) // aka DXT3
            {
                // AoMEE only, supports 4-bit alpha, but some ddt have 1 for ddt.AlphaBits by mistake
                images = DecodeBlockCompression<Bc2Block, Rgba32>(ddt);
            }
            else if (ddt.Format == DdtFormat.Dxt3Swizzled && ddt.AlphaBits == 4) // DXT3 using 444 colors
            {
                images = DecodeBlockCompression<Bc2SwizzledBlock, Rgba32>(ddt);
            }
            else if (ddt.Format == DdtFormat.BC3) // aka DXT5
            {
                // AoMEE only, supports 8-bit alpha, but some ddt have 0 for ddt.AlphaBits by mistake
                images = DecodeBlockCompression<Bc3Block, Rgba32>(ddt);
            }
            else if (ddt.Format == DdtFormat.BT8 && ddt.AlphaBits == 0)
            {
                images = DecodeBt8<Rgb24>(ddt, DecodeBt8As565);
            }
            else if (ddt.Format == DdtFormat.BT8 && ddt.AlphaBits == 1)
            {
                images = DecodeBt8<Rgba32>(ddt, DecodeBt8As5551);
            }
            else if (ddt.Format == DdtFormat.BT8 && ddt.AlphaBits == 4)
            {
                images = DecodeBt8<Rgba32>(ddt, DecodeBt8As4444);
            }
            else if (ddt.Format == DdtFormat.RgbaDeflated)
            {
                // AoMEE only. 8-bit alpha although some ddt have 0/4
                images = DecodeRawDeflated<Bgra32>(ddt);
            }
            else if (ddt.Format == DdtFormat.RgbDeflated)
            {
                // AoMEE only. no alpha although some ddt have 1/4
                images = DecodeRawDeflated<Bgr24>(ddt);
            }
            else
            {
                throw new NotImplementedException($"Converting ddt format {ddt.Format} with {ddt.AlphaBits} alpha bits to image not implemented.");
            }

            return new Texture(images, !ddt.Properties.HasFlag(DdtProperty.NoAlphaTest));
        }

        private static Image[][] DecodeBlockCompression<TBlock, TPixel>(DdtFile ddt)
            where TBlock : unmanaged, IBcBlock
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var decoder = BcBlockDecoder<TBlock>.Instance;
            var numFaces = ddt.Properties.HasFlag(DdtProperty.CubeMap) ? 6 : 1;
            var images = new Image[numFaces][];

            for (int face = 0; face < numFaces; ++face)
            {
                var faceImages = new Image[ddt.MipMapLevels];
                images[face] = faceImages;
                for (int mip = 0; mip < ddt.MipMapLevels; ++mip)
                {
                    var width = Math.Max(1, ddt.Width >> mip);
                    var height = Math.Max(1, ddt.Height >> mip);

                    var pixels = new TPixel[width * height];
                    decoder.Decode(ddt.Data[face][mip], pixels.AsSpan(), width, height);

                    Image image = Image.WrapMemory(pixels.AsMemory(), width, height);
                    image.Mutate(p => p.Flip(FlipMode.Vertical));
                    faceImages[mip] = image;
                }
            }

            return images;
        }

        private delegate byte[] Bt8Decoder(ReadOnlySpan<byte> source, ReadOnlySpan<ushort> palette, int width, int height);
        private static Image[][] DecodeBt8<TPixel>(DdtFile ddt, Bt8Decoder bt8Decoder)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var numFaces = ddt.Properties.HasFlag(DdtProperty.CubeMap) ? 6 : 1;
            var images = new Image[numFaces][];

            for (int face = 0; face < numFaces; ++face)
            {
                var faceImages = new Image[ddt.MipMapLevels];
                images[face] = faceImages;
                for (int mip = 0; mip < ddt.MipMapLevels; ++mip)
                {
                    var width = Math.Max(1, ddt.Width >> mip);
                    var height = Math.Max(1, ddt.Height >> mip);

                    byte[] decompressedData = bt8Decoder(ddt.Data[face][mip], ddt.Bt8PaletteBuffer, width, height);
                    var image = Image.LoadPixelData<TPixel>(decompressedData, width, height);

                    image.Mutate(p => p.Flip(FlipMode.Vertical));
                    faceImages[mip] = image;
                }
            }

            return images;
        }
        private byte[] DecodeBt8As565(ReadOnlySpan<byte> source, ReadOnlySpan<ushort> palette, int width, int height)
        {
            // 3 bytes per pixel output
            byte[] dest = new byte[width * height * 3];

            int sourceIndex = 0, destIndex = 0;
            for (int row = 0; row < height; ++row)
            {
                for (int col = 0; col < width; ++col)
                {
                    var paletteIndex = source[sourceIndex++];
                    var color = palette[paletteIndex];

                    // Extract B5G6R5 (in that order)
                    byte b = (byte)((color & 0x1Fu));
                    byte g = (byte)((color & 0x7E0u) >> 5);
                    byte r = (byte)((color & 0xF800u) >> 11);
                    r = (byte)(r << 3 | r >> 2);
                    g = (byte)(g << 2 | g >> 4);
                    b = (byte)(b << 3 | b >> 2);

                    dest[destIndex++] = r;
                    dest[destIndex++] = g;
                    dest[destIndex++] = b;
                }
            }

            return dest;
        }
        private byte[] DecodeBt8As5551(ReadOnlySpan<byte> source, ReadOnlySpan<ushort> palette, int width, int height)
        {
            // 4 bytes per pixel output
            byte[] dest = new byte[width * height * 4];

            int sourceIndex = 0, destIndex = 0;
            for (int row = 0; row < height; ++row)
            {
                for (int col = 0; col < width; ++col)
                {
                    var paletteIndex = source[sourceIndex++];
                    var color = palette[paletteIndex];

                    // Extract B5G5R5A1 (in that order)
                    byte b = (byte)((color & 0x1Fu));
                    byte g = (byte)((color & 0x3E0u) >> 5);
                    byte r = (byte)((color & 0x7C00u) >> 10);
                    byte a = (byte)((color & 0x8000u) >> 15);
                    r = (byte)(r << 3 | r >> 2);
                    g = (byte)(g << 3 | g >> 2);
                    b = (byte)(b << 3 | b >> 2);
                    a *= byte.MaxValue;

                    dest[destIndex++] = r;
                    dest[destIndex++] = g;
                    dest[destIndex++] = b;
                    dest[destIndex++] = a;
                }
            }

            return dest;
        }
        private byte[] DecodeBt8As4444(ReadOnlySpan<byte> source, ReadOnlySpan<ushort> palette, int width, int height)
        {
            // 4 bytes per pixel output
            byte[] dest = new byte[width * height * 4];

            int sourceIndex = 0, destIndex = 0;
            for (int row = 0; row < height; ++row)
            {
                for (int col = 0; col < width; col += 4)
                {
                    ushort alpha = BinaryPrimitives.ReadUInt16LittleEndian(source.Slice(sourceIndex));
                    sourceIndex += 2;

                    Decode4444((ushort)(palette[source[sourceIndex++]] | ((alpha & 0x00F0u) << 8)), dest, ref destIndex);
                    Decode4444((ushort)(palette[source[sourceIndex++]] | ((alpha & 0x000Fu) << 12)), dest, ref destIndex);
                    Decode4444((ushort)(palette[source[sourceIndex++]] | ((alpha & 0xF000u))), dest, ref destIndex);
                    Decode4444((ushort)(palette[source[sourceIndex++]] | ((alpha & 0x0F00u) << 4)), dest, ref destIndex);
                }
            }

            return dest;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void Decode4444(ushort color, byte[] dest, ref int destIndex)
            {
                // Extract B4G4R4A4 (in that order)
                byte b = (byte)((color & 0x000Fu));
                byte g = (byte)((color & 0x00F0u) >> 4);
                byte r = (byte)((color & 0x0F00u) >> 8);
                byte a = (byte)((color & 0xF000u) >> 12);
                r = (byte)(r << 4 | r);
                g = (byte)(g << 4 | g);
                b = (byte)(b << 4 | b);
                a = (byte)(a << 4 | a);

                dest[destIndex++] = r;
                dest[destIndex++] = g;
                dest[destIndex++] = b;
                dest[destIndex++] = a;
            }
        }

        private static Image[][] DecodeRawDeflated<TPixel>(DdtFile ddt)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var numFaces = ddt.Properties.HasFlag(DdtProperty.CubeMap) ? 6 : 1;
            var images = new Image[numFaces][];

            for (int face = 0; face < numFaces; ++face)
            {
                var faceImages = new List<Image>(ddt.MipMapLevels);
                for (int mip = 0; mip < ddt.MipMapLevels; ++mip)
                {
                    var width = Math.Max(1, ddt.Width >> mip);
                    var height = Math.Max(1, ddt.Height >> mip);

                    var decompressedData = ZlibDecompress(ddt.Data[face][mip]);
                    var pixels = decompressedData.Span.Cast<byte, TPixel>();
                    // For some reason these textures seem to be broken in certain lower mips. Skip the remaining
                    if (pixels.Length < (width * height))
                        break;
                    //var image = Image.WrapMemory<TPixel>(decompressedData.Cast<byte, TPixel>(), width, height);
                    var image = Image.LoadPixelData<TPixel>(decompressedData.Span, width, height);

                    image.Mutate(p => p.Flip(FlipMode.Vertical));
                    faceImages.Add(image);
                }
                images[face] = faceImages.ToArray();
            }

            return images;
        }
        private static Memory<byte> ZlibDecompress(ReadOnlySpan<byte> data)
        {
            using (var sourceStream = new MemoryStream())
            using (var destStream = new MemoryStream())
            using (var decompressionStream = new DeflateStream(sourceStream, CompressionMode.Decompress))
            {
                // Skip zlib header/footer since deflatestream only works on raw deflate data
                int headerSize = (data[1] & 0b10000) == 0b100000 ? 6 : 2;
                if (data.Length <= headerSize + 4) return Memory<byte>.Empty;
                data = data.Slice(headerSize, data.Length - 4 - headerSize);

                sourceStream.Write(data);
                sourceStream.Seek(0, SeekOrigin.Begin);

                // Inflate
                decompressionStream.CopyTo(destStream);
                var buff = destStream.GetBuffer();
                return buff.AsMemory().Slice(0, (int)destStream.Length);
            }
        }
    }
}
