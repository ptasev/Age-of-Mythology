using AoMEngineLibrary.Graphics.Brg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoMBrgEditor
{
    [Flags]
    public enum DdtProperty : byte
    {
        Normal = 0,
        NoAlphaTest = 1,
        NoLowDetail = 2,
        DisplacementMap = 4,
        CubeMap = 8
    };
    public enum DdtFormat : byte
    {
        Raw32 = 1,
        Raw24 = 2,
        BT8 = 3,
        Dxt1 = 4,
        Dxt1Alpha = 5,
        Dxt3Swizzled = 6,
        AlphaData = 7,
        //BC1 = Dxt1,
        BC2 = 8,
        BC3 = 9,
        RgbaDeflated = 10,
        RgbDeflated = 11,
        AlphaDeflated = 12,
        RgDeflated = 13
    };

    public class DdtFile
    {
        public DdtProperty Properties { get; set; }
        public byte AlphaBits { get; set; } //6, 4, 1, 0
        public DdtFormat Format { get; set; }
        public byte NumMipLevels { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public byte[] imageData;

        public DdtFile(System.IO.Stream fileStream)
        {
            using (BrgBinaryReader reader = new BrgBinaryReader(fileStream))
            {
                reader.ReadInt32();
                Properties = (DdtProperty)reader.ReadByte();
                AlphaBits = reader.ReadByte();
                Format = (DdtFormat)reader.ReadByte();
                NumMipLevels = reader.ReadByte();
                Height = reader.ReadInt32();
                Width = reader.ReadInt32();

                // TODO: implement the rest
                return;
                int length = 0;
                for (int i = 0; i < NumMipLevels; i++)
                {
                    reader.ReadInt32();
                    length += reader.ReadInt32();
                }

                imageData = reader.ReadBytes(length);
            }
        }

        public string GetTexelFormat()
        {
            switch (Format)
            {
                case DdtFormat.Dxt1:
                    return "DXT1";
                case DdtFormat.Dxt1Alpha:
                    return "1555";
                case DdtFormat.Dxt3Swizzled:
                    return "4444";
                default:
                    return "";
            }
        }

        public struct Pixel
        {
            public byte R, G, B, A;
            public override string ToString()
            {
                return (((((((A << 8) + R) << 8) + G) << 8) + B)).ToString("x");
            }
            public Pixel Lerp(Pixel p2, int lerp, int denom)
            {
                Pixel p1 = this;
                return new Pixel()
                {
                    R = (byte)(p1.R + ((int)p2.R - p1.R) * lerp / denom),
                    G = (byte)(p1.G + ((int)p2.G - p1.G) * lerp / denom),
                    B = (byte)(p1.B + ((int)p2.B - p1.B) * lerp / denom),
                    A = (byte)(p1.A + ((int)p2.A - p1.A) * lerp / denom),
                };
            }
        }
        private static void Convert565ToRGB(byte[] data, int startData, Pixel[] palette)
        {
            ConvertToRGB(data, startData, palette, 0, 5, 5, 6, 11, 5, 16);
        }
        private static void Convert555ToRGB(byte[] data, int startData, Pixel[] palette)
        {
            ConvertToRGB(data, startData, palette, 1, 5, 6, 5, 11, 5, 16);
        }
        private static void Convert444ToRGB(byte[] data, int startData, Pixel[] palette)
        {
            ConvertToRGB(data, startData, palette, 4, 4, 8, 4, 12, 4, 16);
        }
        private static void ConvertToRGB(byte[] data, int startData, Pixel[] palette, int rOff, int rCnt, int gOff, int gCnt, int bOff, int bCnt, int stride)
        {
            int rMask = ((1 << rCnt) - 1);
            int gMask = ((1 << gCnt) - 1);
            int bMask = ((1 << bCnt) - 1);
            for (int c = 0; c < palette.Length; ++c)
            {
                int cBit = startData * 8 + c * stride;
                int dByte = cBit / 8;
                // Will be such that the color starts at 24th bit
                int b1 = data[dByte + 0];
                int b2 = (dByte + 1 < data.Length ? data[dByte + 1] : 0);
                int b3 = 0;// (dByte + 2 < data.Length ? data[dByte + 2] : 0);
                if (rCnt == 4)
                {
                    palette[c] = new Pixel()
                    {
                        R = (byte)((b1 << 4) & 0xf0),
                        G = (byte)((b1 << 0) & 0xf0),
                        B = (byte)((b2 << 4) & 0xf0),
                        A = 255,
                    };
                }
                else
                {
                    int dataV = ((((b3 << 8) + b2) << 8) + b1) << (cBit - dByte * 8);
                    int r = (((dataV << rOff) >> (16 - rCnt)) & rMask) * 255 / rMask;
                    int g = (((dataV << gOff) >> (16 - gCnt)) & gMask) * 255 / gMask;
                    int b = (((dataV << bOff) >> (16 - bCnt)) & bMask) * 255 / bMask;
                    palette[c] = new Pixel()
                    {
                        R = (byte)r,
                        G = (byte)g,
                        B = (byte)b,
                        A = 255,
                    };
                    if (rCnt == 5 && gCnt == 5 && bCnt == 5)
                    {
                        palette[c].A = ((dataV & 0x8000) == 0 ? (byte)0 : (byte)255);
                    }
                }
            }
        }
        public byte[] Get32BitUncompressed()
        {
            byte[] pixels = new byte[Width * Height * 4];
            switch (Format)
            {
                case DdtFormat.AlphaData:
                    {
                        for (int b = 0; b < imageData.Length; ++b)
                        {
                            var gray = imageData[b];
                            pixels[b * 4 + 0] = gray;
                            pixels[b * 4 + 1] = gray;
                            pixels[b * 4 + 2] = gray;
                            pixels[b * 4 + 3] = 255;
                        }
                    } break;
                case DdtFormat.Dxt1:
                case DdtFormat.Dxt1Alpha:
                case DdtFormat.Dxt3Swizzled:
                    {
                        Pixel[] blockPalette = new Pixel[2];
                        Pixel[] block4Palette = new Pixel[4];
                        int stride = 8;
                        int colOff = 4;
                        int colStride = 1;
                        int alpOff = 0;
                        int alpBits = 0;
                        int alpStrideBits = 0;
                        Action<byte[], int, Pixel[]> extractor = null;
                        switch (Format)
                        {
                            case DdtFormat.Dxt1: extractor = Convert565ToRGB; stride = 8; colOff = 4; colStride = 1; break;
                            case DdtFormat.Dxt1Alpha: extractor = Convert555ToRGB; stride = 10; colOff = 6; colStride = 1; alpOff = 4; alpBits = 1; alpStrideBits = 4 * 1; break;
                            case DdtFormat.Dxt3Swizzled: extractor = Convert444ToRGB; stride = 16; colOff = 6; colStride = 3; alpOff = 4; alpBits = 4; alpStrideBits = 6 * 4; break;
                        }
                        for (int x = 0; x < Width / 4; ++x)
                        {
                            for (int y = 0; y < Height / 4; ++y)
                            {
                                int pixStart = stride * (y * Width / 4 + x);
                                int palStart = pixStart;
                                int colStart = pixStart + colOff;
                                int alpStart = pixStart + alpOff;
                                extractor(imageData, palStart, blockPalette);
                                block4Palette[0] = blockPalette[0];
                                block4Palette[2] = blockPalette[0].Lerp(blockPalette[1], 1, 3);
                                block4Palette[3] = blockPalette[0].Lerp(blockPalette[1], 2, 3);
                                block4Palette[1] = blockPalette[1];
                                for (int sy = 0; sy < 4; ++sy)
                                {
                                    int py = y * 4 + sy;
                                    int imgOff = sy * colStride + colStart;
                                    int data = imageData[imgOff];
                                    for (int sx = 0; sx < 4; ++sx)
                                    {
                                        int v = (data >> (sx * 2)) & 0x03;
                                        Pixel c = block4Palette[v];
                                        int px = x * 4 + sx;
                                        int p = (px + py * Width) * 4;
                                        pixels[p + 0] = c.B;
                                        pixels[p + 1] = c.G;
                                        pixels[p + 2] = c.R;
                                        pixels[p + 3] = c.A;
                                    }
                                    if (alpBits > 0)
                                    {
                                        int alpMask = (1 << alpBits) - 1;
                                        int alphaStartBit = alpStart * 8 + sy * alpStrideBits;
                                        for (int sx = 0; sx < 4; ++sx)
                                        {
                                            int alphaBit = alphaStartBit + sx * alpBits;
                                            int alphaByte = alphaBit / 8;
                                            int alphaBitOff = alphaBit - alphaByte * 8;
                                            int alphaData = ((imageData[alphaByte + 1]) << 8) + imageData[alphaByte];
                                            int px = x * 4 + sx;
                                            int p = (px + py * Width) * 4;
                                            pixels[p + 3] = (byte)(((alphaData >> alphaBitOff) & alpMask) * 255 / alpMask);
                                        }
                                    }
                                }
                            }
                        }
                    } break;
                default: return null;
            }
            // Flip the image
            for (int y = 0; y < Height / 2; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    int p0 = x + y * Width;
                    int p1 = x + (Height - y - 1) * Width;
                    for (int c = 0; c < 4; ++c)
                    {
                        byte t = pixels[p0 * 4 + c];
                        pixels[p0 * 4 + c] = pixels[p1 * 4 + c];
                        pixels[p1 * 4 + c] = t;
                    }
                }
            }
            return pixels;
        }
    }
}