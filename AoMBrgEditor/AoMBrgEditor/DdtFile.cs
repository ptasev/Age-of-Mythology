using MiscUtil.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
namespace AoMBrgEditor
{
    public enum DdtType : byte
    {
        Texture = 0, 
        Texture2 = 1, 
        Palette = 3, 
        BumpMap = 6, 
        CubeMap = 8
    };
    public enum DdtTexelFormat : byte
    { 
        Uncompressed32 = 1,
        Palette = 3,
        DXT1 = 4,
        DXT3 = 5,
        DXT5 = 6,
        Grayscale8 = 7 
    };

    public class DdtFile
    {
        DdtType type;
        byte alphaBits;
        public DdtTexelFormat texelFormat;
        public byte mipMap;
        public int Height;
        public int Width;
        public byte[] imageData;

        public DdtFile(System.IO.Stream fileStream)
        {
            using (BrgBinaryReader reader = new BrgBinaryReader(new LittleEndianBitConverter(), fileStream))
            {
                reader.ReadInt32();
                type = (DdtType)reader.ReadByte();
                alphaBits = reader.ReadByte();
                texelFormat = (DdtTexelFormat)reader.ReadByte();
                mipMap = reader.ReadByte();
                Height = reader.ReadInt32();
                Width = reader.ReadInt32();

                int length = 0;
                for (int i = 0; i < mipMap; i++)
                {
                    reader.ReadInt32();
                    length += reader.ReadInt32();
                }

                imageData = reader.ReadBytes(length);
            }
        }

        public string GetTexelFormat()
        {
            switch (texelFormat) 
            {
                case DdtTexelFormat.DXT3:
                    return "DXT1";
                default:
                    return "";
            }
        }
    }
}
*/