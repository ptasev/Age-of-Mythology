namespace AoMEngineLibrary.Graphics.Brg
{
    using AoMEngineLibrary.Graphics.Model;
    using Extensions;
    using Newtonsoft.Json;
    using System;
    using System.Numerics;

    public class BrgMeshHeader
    {
        public Int16 Version { get; set; }
        public BrgMeshFormat Format { get; set; }
        public Int16 NumVertices { get; set; }
        public Int16 NumFaces { get; set; }
        public BrgMeshInterpolationType InterpolationType { get; set; }
        public BrgMeshAnimType AnimationType { get; set; }
        public Int16 UserDataEntryCount { get; set; }
        public Vector3 CenterPosition { get; set; }
        public Single CenterRadius { get; set; }
        public Vector3 MassPosition { get; set; }
        public Vector3 HotspotPosition { get; set; }
        public Int16 ExtendedHeaderSize { get; set; }
        public BrgMeshFlag Flags { get; set; }
        public Vector3 MinimumExtent { get; set; }
        public Vector3 MaximumExtent { get; set; }

        public BrgMeshHeader()
        {

        }
        public BrgMeshHeader(BrgBinaryReader reader)
        {
            this.Version = reader.ReadInt16();
            this.Format = (BrgMeshFormat)reader.ReadInt16();
            this.NumVertices = reader.ReadInt16();
            this.NumFaces = reader.ReadInt16();
            this.InterpolationType = (BrgMeshInterpolationType)reader.ReadByte();
            this.AnimationType = (BrgMeshAnimType)reader.ReadByte();
            this.UserDataEntryCount = reader.ReadInt16();
            this.CenterPosition = reader.ReadVector3D(true, false);
            this.CenterRadius = reader.ReadSingle();
            this.MassPosition = reader.ReadVector3D(true, false);
            this.HotspotPosition = reader.ReadVector3D(true, false);
            this.ExtendedHeaderSize = reader.ReadInt16();
            this.Flags = (BrgMeshFlag)reader.ReadInt16();
            this.MinimumExtent = reader.ReadVector3D(true, false);
            this.MaximumExtent = reader.ReadVector3D(true, false);
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(this.Version);
            writer.Write((UInt16)this.Format);
            writer.Write(this.NumVertices);
            writer.Write(this.NumFaces);
            writer.Write((Byte)this.InterpolationType);
            writer.Write((Byte)this.AnimationType);
            writer.Write(this.UserDataEntryCount);
            writer.WriteVector3D(this.CenterPosition, true);
            writer.Write(this.CenterRadius);//unknown03
            writer.WriteVector3D(this.MassPosition, true);
            writer.WriteVector3D(this.HotspotPosition, true);
            writer.Write(this.ExtendedHeaderSize);
            writer.Write((UInt16)this.Flags);
            writer.WriteVector3D(this.MinimumExtent, true);
            writer.WriteVector3D(this.MaximumExtent, true);
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
                        case nameof(Version):
                            Version = (Int16)reader.ReadAsInt32();
                            break;
                        case nameof(Format):
                            Format = (BrgMeshFormat)reader.ReadAsInt32();
                            break;
                        case nameof(NumVertices):
                            NumVertices = (Int16)reader.ReadAsInt32();
                            break;
                        case nameof(NumFaces):
                            NumFaces = (Int16)reader.ReadAsInt32();
                            break;
                        case nameof(InterpolationType):
                            InterpolationType = (BrgMeshInterpolationType)reader.ReadAsInt32();
                            break;
                        case nameof(AnimationType):
                            AnimationType = (BrgMeshAnimType)reader.ReadAsInt32();
                            break;
                        case nameof(UserDataEntryCount):
                            UserDataEntryCount = (Int16)reader.ReadAsInt32();
                            break;
                        case nameof(CenterPosition):
                            CenterPosition = reader.ReadAsVector3();
                            break;
                        case nameof(CenterRadius):
                            CenterRadius = (float)reader.ReadAsDouble();
                            break;
                        case nameof(MassPosition):
                            MassPosition = reader.ReadAsVector3();
                            break;
                        case nameof(HotspotPosition):
                            HotspotPosition = reader.ReadAsVector3();
                            break;
                        case nameof(ExtendedHeaderSize):
                            ExtendedHeaderSize = (Int16)reader.ReadAsInt32();
                            break;
                        case nameof(Flags):
                            Flags = (BrgMeshFlag)reader.ReadAsInt32();
                            break;
                        case nameof(MinimumExtent):
                            MinimumExtent = reader.ReadAsVector3();
                            break;
                        case nameof(MaximumExtent):
                            MaximumExtent = reader.ReadAsVector3();
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

            writer.WritePropertyName(nameof(Version));
            writer.WriteValue(Version);

            writer.WritePropertyName(nameof(Format));
            writer.WriteValue(Format);

            writer.WritePropertyName(nameof(NumVertices));
            writer.WriteValue(NumVertices);

            writer.WritePropertyName(nameof(NumFaces));
            writer.WriteValue(NumFaces);

            writer.WritePropertyName(nameof(InterpolationType));
            writer.WriteValue(InterpolationType);

            writer.WritePropertyName(nameof(AnimationType));
            writer.WriteValue(AnimationType);

            writer.WritePropertyName(nameof(UserDataEntryCount));
            writer.WriteValue(UserDataEntryCount);

            writer.WritePropertyName(nameof(CenterPosition));
            CenterPosition.WriteJson(writer);

            writer.WritePropertyName(nameof(CenterRadius));
            writer.WriteRawValue(CenterRadius.ToRoundTripString());

            writer.WritePropertyName(nameof(MassPosition));
            MassPosition.WriteJson(writer);

            writer.WritePropertyName(nameof(HotspotPosition));
            HotspotPosition.WriteJson(writer);

            writer.WritePropertyName(nameof(ExtendedHeaderSize));
            writer.WriteValue(ExtendedHeaderSize);

            writer.WritePropertyName(nameof(Flags));
            writer.WriteValue(Flags);

            writer.WritePropertyName(nameof(MinimumExtent));
            MinimumExtent.WriteJson(writer);

            writer.WritePropertyName(nameof(MaximumExtent));
            MaximumExtent.WriteJson(writer);

            writer.WriteEndObject();
        }
    }
}
