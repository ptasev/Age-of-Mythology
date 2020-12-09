using System;
using System.IO;

namespace AoMEngineLibrary.Graphics.Ddt
{
    public class DdtFile
    {
        public DdtProperty Properties { get; set; }

        public byte AlphaBits { get; set; } //6, 4, 1, 0

        public DdtFormat Format { get; set; }

        public byte MipMapLevels { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public Bt8ImageInfo Bt8ImageInfo { get; set; }

        public ushort[] Bt8PaletteBuffer { get; set; }

        public byte[][][] Data { get; set; }

        public DdtFile()
        {
            Bt8ImageInfo = new Bt8ImageInfo();
            Bt8PaletteBuffer = Array.Empty<ushort>();
            Data = Array.Empty<byte[][]>();
        }

        public void Read(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                var magic = reader.ReadUInt32();

                if (magic != 861099090) // ASCII RTS3
                {
                    throw new FileFormatException("This is not a ddt file.");
                }

                Properties = (DdtProperty)reader.ReadByte();
                AlphaBits = reader.ReadByte();
                Format = (DdtFormat)reader.ReadByte();
                MipMapLevels = reader.ReadByte();
                Width = reader.ReadInt32();
                Height = reader.ReadInt32();

                Bt8ImageInfo = new Bt8ImageInfo();
                if (Format == DdtFormat.BT8)
                {
                    Bt8ImageInfo.NumColors = reader.ReadUInt32();

                    Bt8ImageInfo.RGB8Offset = reader.ReadUInt32();
                    Bt8ImageInfo.R5G6B5Offset = reader.ReadUInt32();
                    Bt8ImageInfo.R5G5B5Offset = reader.ReadUInt32();
                    Bt8ImageInfo.A1R5B5G5Offset = reader.ReadUInt32();
                    Bt8ImageInfo.A4R4B4G4Offset = reader.ReadUInt32();
                }

                int numFaces = Properties.HasFlag(DdtProperty.CubeMap) ? 6 : 1;
                const int minMipDimension = 1;
                //int minMipDimension = Format switch
                //{
                //    DdtFormat.Dxt1 => 4,
                //    DdtFormat.Dxt1Alpha => 4,
                //    DdtFormat.Dxt3Swizzled => 4,
                //    DdtFormat.BC2 => 4,
                //    DdtFormat.BC3 => 4,
                //    _ => 1
                //};

                uint totalDataSize = 0;
                var imageEntries = new DdtImageEntry[numFaces][];
                for (int face = 0; face < numFaces; ++face)
                {
                    var faceImageEntries = new DdtImageEntry[MipMapLevels];
                    imageEntries[face] = faceImageEntries;
                    for (int mip = 0; mip < MipMapLevels; ++mip)
                    {
                        var entry = new DdtImageEntry();
                        entry.Offset = reader.ReadUInt32();
                        entry.Size = reader.ReadUInt32();

                        entry.Width = Math.Max(minMipDimension, Width >> mip);
                        entry.Height = Math.Max(minMipDimension, Height >> mip);
                        faceImageEntries[mip] = entry;

                        // Increment total data size, we'll need this later
                        totalDataSize += entry.Size;
                    }
                }

                // Get the palette for BT8 format
                if (Format == DdtFormat.BT8)
                {
                    // max of 256 16-bit colors in palette
                    if (Bt8ImageInfo.NumColors > 256)
                        throw new InvalidDataException("BT8 palette cannot have more than 256 colors.");

                    uint paletteOffset;
                    if (AlphaBits == 4)
                    {
                        paletteOffset = Bt8ImageInfo.A4R4B4G4Offset;
                    }
                    else if (AlphaBits == 1)
                    {
                        paletteOffset = Bt8ImageInfo.A1R5B5G5Offset;
                    }
                    else
                    {
                        paletteOffset = Bt8ImageInfo.R5G6B5Offset;
                    }

                    if (paletteOffset == 0)
                    {
                        throw new InvalidDataException("Invalid BT8 palette offset.");
                    }

                    // Read bt8 palette
                    reader.BaseStream.Seek(paletteOffset, SeekOrigin.Begin);
                    Bt8PaletteBuffer = new ushort[Bt8ImageInfo.NumColors];
                    for (uint i = 0; i < Bt8PaletteBuffer.Length; ++i)
                    {
                        Bt8PaletteBuffer[i] = reader.ReadUInt16();
                    }
                }

                // Read all the byte data
                Data = new byte[numFaces][][];
                for (int face = 0; face < numFaces; ++face)
                {
                    var faceData = new byte[MipMapLevels][];
                    Data[face] = faceData;
                    for (int mip = 0; mip < MipMapLevels; ++mip)
                    {
                        var imageEntry = imageEntries[face][mip];

                        reader.BaseStream.Seek(imageEntry.Offset, SeekOrigin.Begin);
                        faceData[mip] = reader.ReadBytes((int)imageEntry.Size);
                    }
                }
            }
        }

        public BtiFile GetTextureInfo()
        {
            BtiFile bti = new BtiFile();

            bti.AlphaBits = AlphaBits;
            bti.AlphaTest = !Properties.HasFlag(DdtProperty.NoAlphaTest);
            bti.UseMips = MipMapLevels > 0;
            bti.CanBeLowDetail = !Properties.HasFlag(DdtProperty.NoLowDetail);
            bti.Displacement = Properties.HasFlag(DdtProperty.DisplacementMap);
            bti.Format = GetBtiTextureFormat();

            return bti;
        }

        private BtiTextureFormat GetBtiTextureFormat()
        {
            return Format switch
            {
                DdtFormat.Raw32 => BtiTextureFormat.DeflatedRGBA8,
                DdtFormat.Raw24 => BtiTextureFormat.DeflatedRGB8,
                DdtFormat.BT8 when AlphaBits == 0 => BtiTextureFormat.BC1,
                DdtFormat.BT8 when AlphaBits == 1 => BtiTextureFormat.BC1,
                DdtFormat.BT8 when AlphaBits == 4 => BtiTextureFormat.BC2,
                DdtFormat.Dxt1 => BtiTextureFormat.BC1,
                DdtFormat.Dxt1Alpha => BtiTextureFormat.BC1,
                DdtFormat.Dxt3Swizzled => BtiTextureFormat.BC2,
                DdtFormat.AlphaData => BtiTextureFormat.DeflatedR8,
                DdtFormat.BC2 => BtiTextureFormat.BC2,
                DdtFormat.BC3 => BtiTextureFormat.BC3,
                DdtFormat.RgbaDeflated => BtiTextureFormat.DeflatedRGBA8,
                DdtFormat.RgbDeflated => BtiTextureFormat.DeflatedRGB8,
                DdtFormat.AlphaDeflated => BtiTextureFormat.DeflatedR8,
                DdtFormat.RgDeflated => BtiTextureFormat.DeflatedRG8,
                _ => throw new InvalidOperationException($"Cannot convert ddt format {Format} to bti texture format.")
            };
        }
    }
}