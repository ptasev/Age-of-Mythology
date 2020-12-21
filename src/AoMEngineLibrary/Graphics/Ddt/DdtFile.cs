using System;
using System.IO;
using System.Linq;

namespace AoMEngineLibrary.Graphics.Ddt
{
    public class DdtFile
    {
        private const int Magic = 861099090; // ASCII RTS3

        public DdtProperty Properties { get; set; }

        // 8, 4, 1, 0
        public byte AlphaBits { get; set; }

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

                if (magic != Magic)
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

        public void Read(string filePath)
        {
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                Read(fs);
        }

        public void Write(Stream stream)
        {
            using (var w = new BinaryWriter(stream))
            {
                int numFaces = Properties.HasFlag(DdtProperty.CubeMap) ? 6 : 1;
                if (Data.Length != numFaces)
                    throw new InvalidDataException("The ddt must have either 1 or 6 faces (cube map).");

                if (Data.Any(f => f.Length != MipMapLevels))
                    throw new InvalidDataException($"Each face in the ddt must have the same number of mips {MipMapLevels}.");

                w.Write(Magic);
                w.Write((byte)Properties);
                w.Write(AlphaBits);
                w.Write((byte)Format);
                w.Write(MipMapLevels);

                w.Write(Width);
                w.Write(Height);

                if (Format == DdtFormat.BT8)
                {
                    w.Write(Bt8ImageInfo.NumColors);

                    Bt8ImageInfo.RGB8Offset = 0;
                    Bt8ImageInfo.R5G6B5Offset = 0;
                    Bt8ImageInfo.R5G5B5Offset = 0;
                    Bt8ImageInfo.A1R5B5G5Offset = 0;
                    Bt8ImageInfo.A4R4B4G4Offset = 0;

                    // Offset is current pos + 4 * 5 offsets + length of all entries
                    uint paletteOffset = (uint)(w.BaseStream.Position + 20 + 8 * numFaces * MipMapLevels);
                    if (AlphaBits == 4)
                    {
                        Bt8ImageInfo.A4R4B4G4Offset = paletteOffset;
                    }
                    else if (AlphaBits == 1)
                    {
                        Bt8ImageInfo.A1R5B5G5Offset = paletteOffset;
                    }
                    else if (AlphaBits == 0)
                    {
                        Bt8ImageInfo.R5G6B5Offset = paletteOffset;
                    }
                    else
                    {
                        throw new InvalidDataException("Ddt format BT8 only supports 0, 1, or 4 alpha bits.");
                    }

                    w.Write(Bt8ImageInfo.RGB8Offset);
                    w.Write(Bt8ImageInfo.R5G6B5Offset);
                    w.Write(Bt8ImageInfo.R5G5B5Offset);
                    w.Write(Bt8ImageInfo.A1R5B5G5Offset);
                    w.Write(Bt8ImageInfo.A4R4B4G4Offset);
                }

                // Set offset to current pos + length of all entries + 2 * palette colors
                uint offset = (uint)(w.BaseStream.Position + 8 * numFaces * MipMapLevels + 2 * Bt8ImageInfo.NumColors);
                for (int face = 0; face < numFaces; ++face)
                {
                    for (int mip = 0; mip < MipMapLevels; ++mip)
                    {
                        var data = Data[face][mip];
                        w.Write(offset);
                        w.Write(data.Length);

                        // Increment the offset
                        offset += (uint)data.Length;
                    }
                }

                // Write the palette for BT8 format
                if (Format == DdtFormat.BT8)
                {
                    // max of 256 16-bit colors in palette
                    if (Bt8ImageInfo.NumColors > 256)
                        throw new InvalidDataException("BT8 palette cannot have more than 256 colors.");

                    if (Bt8ImageInfo.NumColors != Bt8PaletteBuffer.Length)
                        throw new InvalidDataException("BT8 palette buffer length must match the number of colors in the BT8 image info.");

                    // Write bt8 palette
                    Bt8PaletteBuffer = new ushort[Bt8ImageInfo.NumColors];
                    for (uint i = 0; i < Bt8PaletteBuffer.Length; ++i)
                    {
                        w.Write(Bt8PaletteBuffer[i]);
                    }
                }

                // Write all the image data
                for (int face = 0; face < numFaces; ++face)
                {
                    for (int mip = 0; mip < MipMapLevels; ++mip)
                    {
                        var data = Data[face][mip];
                        w.Write(data);
                    }
                }
            }
        }

        public void Write(string filePath)
        {
            using (var fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                Write(fs);
        }

        public BtiFile GetTextureInfo()
        {
            BtiFile bti = new BtiFile();

            bti.AlphaBits = AlphaBits;
            bti.AlphaTest = !Properties.HasFlag(DdtProperty.NoAlphaTest);
            bti.UseMips = MipMapLevels > 1;
            bti.CanBeLowDetail = !Properties.HasFlag(DdtProperty.NoLowDetail);
            bti.Displacement = Properties.HasFlag(DdtProperty.DisplacementMap);
            bti.Format = GetBtiTextureFormat();

            return bti;
        }

        public void SetTextureInfo(BtiFile bti)
        {
            AlphaBits = bti.AlphaBits;
            if (!bti.AlphaTest) Properties |= DdtProperty.NoAlphaTest;
            if (!bti.CanBeLowDetail) Properties |= DdtProperty.NoLowDetail;
            if (bti.Displacement) Properties |= DdtProperty.DisplacementMap;

            Format = bti.Format switch
            {
                BtiTextureFormat.BC1 => DdtFormat.Dxt1,
                BtiTextureFormat.BC2 => DdtFormat.BC2,
                BtiTextureFormat.BC3 => DdtFormat.BC3,
                BtiTextureFormat.DeflatedR8 => DdtFormat.AlphaDeflated,
                BtiTextureFormat.DeflatedRG8 => DdtFormat.RgDeflated,
                BtiTextureFormat.DeflatedRGB8 => DdtFormat.RgbDeflated,
                BtiTextureFormat.DeflatedRGBA8 => DdtFormat.RgbaDeflated,
                _ => throw new InvalidOperationException($"Cannot convert bti texture format {bti.Format} to ddt format.")
            };
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
                DdtFormat.Dxt1Alpha => BtiTextureFormat.BC3, // use BC3 since BC1 with alpha has lower quality RGB (only 3 color palette)
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