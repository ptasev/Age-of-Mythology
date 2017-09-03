namespace AoMEngineLibrary.Graphics.Brg
{
    using Extensions;
    using Newtonsoft.Json;
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

        public void ReadJson(JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    switch ((string)reader.Value)
                    {
                        case nameof(NumNameIndexes):
                            NumNameIndexes = (Int16)reader.ReadAsInt32();
                            break;
                        case nameof(NumDummies):
                            NumDummies = (Int16)reader.ReadAsInt32();
                            break;
                        case nameof(NameLength):
                            NameLength = (Int16)reader.ReadAsInt32();
                            break;
                        case nameof(PointMaterial):
                            PointMaterial = (Int16)reader.ReadAsInt32();
                            break;
                        case nameof(PointRadius):
                            PointRadius = (float)reader.ReadAsDouble();
                            break;
                        case nameof(NumMaterials):
                            NumMaterials = (byte)reader.ReadAsInt32();
                            break;
                        case nameof(ShadowNameLength0):
                            ShadowNameLength0 = (byte)reader.ReadAsInt32();
                            break;
                        case nameof(ShadowNameLength1):
                            ShadowNameLength1 = (byte)reader.ReadAsInt32();
                            break;
                        case nameof(ShadowNameLength2):
                            ShadowNameLength2 = (byte)reader.ReadAsInt32();
                            break;
                        case nameof(AnimationLength):
                            AnimationLength = (float)reader.ReadAsDouble();
                            break;
                        case nameof(MaterialLibraryTimestamp):
                            MaterialLibraryTimestamp = reader.ReadAsInt32().Value;
                            break;
                        case nameof(Reserved):
                            Reserved = (float)reader.ReadAsDouble();
                            break;
                        case nameof(ExportedScaleFactor):
                            ExportedScaleFactor = (float)reader.ReadAsDouble();
                            break;
                        case nameof(NumNonUniformKeys):
                            NumNonUniformKeys = reader.ReadAsInt32().Value;
                            break;
                        case nameof(NumUniqueMaterials):
                            NumUniqueMaterials = reader.ReadAsInt32().Value;
                            break;
                        default:
                            throw new Exception("Unexpected property name!");
                    }
                }
                else if (reader.TokenType != JsonToken.StartObject)
                {
                    throw new Exception("Unexpected token type! " + reader.TokenType);
                }
            }
        }

        public void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(NumNameIndexes));
            writer.WriteValue(NumNameIndexes);

            writer.WritePropertyName(nameof(NumDummies));
            writer.WriteValue(NumDummies);

            writer.WritePropertyName(nameof(NameLength));
            writer.WriteValue(NameLength);

            writer.WritePropertyName(nameof(PointMaterial));
            writer.WriteValue(PointMaterial);

            writer.WritePropertyName(nameof(PointRadius));
            writer.WriteRawValue(PointRadius.ToRoundTripString());

            writer.WritePropertyName(nameof(NumMaterials));
            writer.WriteValue(NumMaterials);

            writer.WritePropertyName(nameof(ShadowNameLength0));
            writer.WriteValue(ShadowNameLength0);

            writer.WritePropertyName(nameof(ShadowNameLength1));
            writer.WriteValue(ShadowNameLength1);

            writer.WritePropertyName(nameof(ShadowNameLength2));
            writer.WriteValue(ShadowNameLength2);

            writer.WritePropertyName(nameof(AnimationLength));
            writer.WriteRawValue(AnimationLength.ToRoundTripString());

            writer.WritePropertyName(nameof(MaterialLibraryTimestamp));
            writer.WriteValue(MaterialLibraryTimestamp);

            writer.WritePropertyName(nameof(Reserved));
            writer.WriteRawValue(Reserved.ToRoundTripString());

            writer.WritePropertyName(nameof(ExportedScaleFactor));
            writer.WriteRawValue(ExportedScaleFactor.ToRoundTripString());

            writer.WritePropertyName(nameof(NumNonUniformKeys));
            writer.WriteValue(NumNonUniformKeys);

            writer.WritePropertyName(nameof(NumUniqueMaterials));
            writer.WriteValue(NumUniqueMaterials);

            writer.WriteEndObject();
        }
    }
}
