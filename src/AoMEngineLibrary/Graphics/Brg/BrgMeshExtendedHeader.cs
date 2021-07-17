namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgMeshExtendedHeader
    {
        public ushort NumNameIndexes { get; set; }
        public ushort NumDummies { get; set; }
        public ushort NameLength { get; set; } // unknown091
        public short PointMaterial { get; set; }
        public float PointRadius { get; set; } // unknown09Unused
        public byte NumMaterials { get; set; } // lastMaterialIndex
        public byte ShadowNameLength0 { get; set; }
        public byte ShadowNameLength1 { get; set; }
        public byte ShadowNameLength2 { get; set; }
        public float AnimationLength { get; set; }
        public uint MaterialLibraryTimestamp { get; set; } // unknown09Const
        public float Reserved { get; set; }
        public float ExportedScaleFactor { get; set; } // animTimeMult
        public int NumNonUniformKeys { get; set; } //09c
        public int NumUniqueMaterials { get; set; } // numMaterialsUsed

        public BrgMeshExtendedHeader()
        {
            AnimationLength = 0f;
        }
        public BrgMeshExtendedHeader(BrgBinaryReader reader, int extendedHeaderSize)
        {
            NumNameIndexes = reader.ReadUInt16();
            NumDummies = reader.ReadUInt16();
            NameLength = reader.ReadUInt16();
            if (extendedHeaderSize > 6)
            {
                PointMaterial = reader.ReadInt16();
                PointRadius = reader.ReadSingle();
            }
            if (extendedHeaderSize > 12)
            {
                NumMaterials = reader.ReadByte();
                ShadowNameLength0 = reader.ReadByte();
                ShadowNameLength1 = reader.ReadByte();
                ShadowNameLength2 = reader.ReadByte();
            }
            if (extendedHeaderSize > 16)
            {
                AnimationLength = reader.ReadSingle();
            }
            if (extendedHeaderSize > 20)
            {
                MaterialLibraryTimestamp = reader.ReadUInt32();
            }
            if (extendedHeaderSize > 24)
            {
                Reserved = reader.ReadSingle();
            }
            if (extendedHeaderSize > 28)
            {
                ExportedScaleFactor = reader.ReadSingle();
            }
            if (extendedHeaderSize > 32)
            {
                NumNonUniformKeys = reader.ReadInt32(); //09c
            }
            if (extendedHeaderSize > 36)
            {
                NumUniqueMaterials = reader.ReadInt32();
            }
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(NumNameIndexes);
            writer.Write(NumDummies);
            writer.Write(NameLength);
            writer.Write(PointMaterial);
            writer.Write(PointRadius);
            writer.Write(NumMaterials);
            writer.Write(ShadowNameLength0);
            writer.Write(ShadowNameLength1);
            writer.Write(ShadowNameLength2);
            writer.Write(AnimationLength);
            writer.Write(MaterialLibraryTimestamp);
            writer.Write(Reserved);
            writer.Write(ExportedScaleFactor);
            writer.Write(NumNonUniformKeys);
            writer.Write(NumUniqueMaterials);
        }
    }
}
