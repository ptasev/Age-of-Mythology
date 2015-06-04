namespace AoMEngineLibrary.Graphics.Brg
{
    using System;

    public class BrgMeshExtendedHeader
    {
        public Int16 NumNameIndexes { get; set; }
        public Int16 NumDummies { get; set; }
        public Int16 NameLength { get; set; } // unknown091
        public Int16 PointMaterial { get; set; }
        public float PointRadius { get; set; } // unknown09Unused
        public byte NumMaterials { get; set; } // lastMaterialIndex
        public byte ShadowNameLength0 { get; set; }
        public byte ShadowNameLength1 { get; set; }
        public byte ShadowNameLength2 { get; set; }
        public float AnimationLength { get; set; }
        public int MaterialLibraryTimestamp { get; set; } // unknown09Const
        public float Reserved { get; set; }
        public float ExportedScaleFactor { get; set; } // animTimeMult
        public int NumNonUniformKeys { get; set; } //09c
        public int NumUniqueMaterials { get; set; } // numMaterialsUsed

        public BrgMeshExtendedHeader()
        {
            this.AnimationLength = 0f;
        }
        public BrgMeshExtendedHeader(BrgBinaryReader reader, int extendedHeaderSize)
        {
            this.NumNameIndexes = reader.ReadInt16();
            this.NumDummies = reader.ReadInt16();
            this.NameLength = reader.ReadInt16();
            if (extendedHeaderSize > 6)
            {
                this.PointMaterial = reader.ReadInt16();
                this.PointRadius = reader.ReadSingle();
            }
            if (extendedHeaderSize > 12)
            {
                this.NumMaterials = reader.ReadByte();
                this.ShadowNameLength0 = reader.ReadByte();
                this.ShadowNameLength1 = reader.ReadByte();
                this.ShadowNameLength2 = reader.ReadByte();
            }
            if (extendedHeaderSize > 16)
            {
                this.AnimationLength = reader.ReadSingle();
            }
            if (extendedHeaderSize > 20)
            {
                this.MaterialLibraryTimestamp = reader.ReadInt32();
            }
            if (extendedHeaderSize > 24)
            {
                this.Reserved = reader.ReadSingle();
            }
            if (extendedHeaderSize > 28)
            {
                this.ExportedScaleFactor = reader.ReadSingle();
            }
            if (extendedHeaderSize > 32)
            {
                this.NumNonUniformKeys = reader.ReadInt32(); //09c
            }
            if (extendedHeaderSize > 36)
            {
                this.NumUniqueMaterials = reader.ReadInt32();
            }
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(this.NumNameIndexes);
            writer.Write(this.NumDummies);
            writer.Write(this.NameLength);
            writer.Write(this.PointMaterial);
            writer.Write(this.PointRadius);
            writer.Write(this.NumMaterials);
            writer.Write(this.ShadowNameLength0);
            writer.Write(this.ShadowNameLength1);
            writer.Write(this.ShadowNameLength2);
            writer.Write(this.AnimationLength);
            writer.Write(this.MaterialLibraryTimestamp);
            writer.Write(this.Reserved);
            writer.Write(this.ExportedScaleFactor);
            writer.Write(this.NumNonUniformKeys);
            writer.Write(this.NumUniqueMaterials);
        }
    }
}
