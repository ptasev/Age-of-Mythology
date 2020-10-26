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

        public byte[][][] Data { get; set; }

        public DdtFile()
        {
            Bt8ImageInfo = new Bt8ImageInfo();
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
                Height = reader.ReadInt32();
                Width = reader.ReadInt32();

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
                const int minMipDimension = 4;
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
                        throw new InvalidDataException("Palette cannot have more than 256 colors.");

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

                    // TODO: read bt8 palette and store in field
                    //var palette = new byte[2 * Bt8ImageInfo.NumColors];

                    throw new NotImplementedException($"Support for format {Format} not implemented.");
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
    }
}