using AoMEngineLibrary.Graphics.Brg;
using System;

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
                imageData = Array.Empty<byte>();

                // TODO: implement the rest
            }
        }
    }
}