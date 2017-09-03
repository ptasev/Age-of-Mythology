namespace AoMEngineLibrary.Graphics.Brg
{
    using Newtonsoft.Json;
    using System;

    public class BrgHeader
    {
        public string Magic { get; set; }
        public int Unknown01 { get; set; }
        public int NumMaterials { get; set; }
        public int Unknown02 { get; set; }
        public int NumMeshes { get; set; }
        public int Reserved { get; set; }
        public int Unknown03 { get; set; }

        public BrgHeader()
        {
            this.Magic = "BANG";
            this.Unknown03 = 1999922179;
        }
        public BrgHeader(BrgBinaryReader reader)
        {
            this.Magic = reader.ReadString(4);
            this.Unknown01 = reader.ReadInt32();
            this.NumMaterials = reader.ReadInt32();
            this.Unknown02 = reader.ReadInt32();
            this.NumMeshes = reader.ReadInt32();
            this.Reserved = reader.ReadInt32();
            this.Unknown03 = reader.ReadInt32();
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(1196310850); // magic "BANG"
            writer.Write(this.Unknown01);
            writer.Write(this.NumMaterials);
            writer.Write(this.Unknown02);
            writer.Write(this.NumMeshes);
            writer.Write(this.Reserved);
            writer.Write(1999922179);
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
                        case nameof(Magic):
                            Magic = reader.ReadAsString();
                            break;
                        case nameof(Unknown01):
                            Unknown01 = reader.ReadAsInt32().Value;
                            break;
                        case nameof(NumMaterials):
                            NumMaterials = reader.ReadAsInt32().Value;
                            break;
                        case nameof(Unknown02):
                            Unknown02 = reader.ReadAsInt32().Value;
                            break;
                        case nameof(NumMeshes):
                            NumMeshes = reader.ReadAsInt32().Value;
                            break;
                        case nameof(Reserved):
                            Reserved = reader.ReadAsInt32().Value;
                            break;
                        case nameof(Unknown03):
                            Unknown03 = reader.ReadAsInt32().Value;
                            break;
                        default:
                            throw new Exception("Unexpected token type! " + reader.TokenType);
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

            writer.WritePropertyName("Magic");
            writer.WriteValue(Magic);
            writer.WritePropertyName("Unknown01");
            writer.WriteValue(Unknown01);
            writer.WritePropertyName("NumMaterials");
            writer.WriteValue(NumMaterials);
            writer.WritePropertyName("Unknown02");
            writer.WriteValue(Unknown02);
            writer.WritePropertyName("NumMeshes");
            writer.WriteValue(NumMeshes);
            writer.WritePropertyName("Reserved");
            writer.WriteValue(Reserved);
            writer.WritePropertyName("Unknown03");
            writer.WriteValue(Unknown03);

            writer.WriteEndObject();
        }
    }
}
