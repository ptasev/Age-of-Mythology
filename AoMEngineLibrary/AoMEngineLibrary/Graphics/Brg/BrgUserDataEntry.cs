namespace AoMEngineLibrary.Graphics.Brg
{
    using Extensions;
    using Newtonsoft.Json;
    using System;

    public struct BrgUserDataEntry
    {
        public int dataNameLength;
        public int dataType;
        public object data;
        public string dataName;

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
                    switch (reader.ReadAsString())
                    {
                        case nameof(dataNameLength):
                            dataNameLength = reader.ReadAsInt32().Value;
                            break;
                        case nameof(dataType):
                            dataType = reader.ReadAsInt32().Value;
                            break;
                        case nameof(data):
                            reader.Read();
                            data = reader.Value;
                            break;
                        case nameof(dataName):
                            dataName = reader.ReadAsString();
                            break;
                        default:
                            throw new Exception("Unexpected property name!");
                    }
                }
                else
                {
                    throw new Exception("Unexpected token type! " + reader.TokenType);
                }
            }
        }

        public void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(dataNameLength));
            writer.WriteValue(dataNameLength);

            writer.WritePropertyName(nameof(dataType));
            writer.WriteValue(dataType);

            writer.WritePropertyName(nameof(data));
            if (data is float)
            {
                writer.WriteRawValue(((float)data).ToRoundTripString());
            }
            else
            {
                writer.WriteValue(data);
            }

            writer.WritePropertyName(nameof(dataName));
            writer.WriteValue(dataName);

            writer.WriteEndObject();
        }
    }
}
