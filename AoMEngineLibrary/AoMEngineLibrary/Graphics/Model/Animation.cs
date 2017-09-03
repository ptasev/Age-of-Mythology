namespace AoMEngineLibrary.Graphics.Model
{
    using Extensions;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Animation
    {
        public float Duration { get; set; }
        public float TimeStep { get; set; }
        public List<float> MeshKeys { get; set; }

        public Animation()
        {
            this.Duration = 0f;
            this.TimeStep = 1f;
            this.MeshKeys = new List<float>();
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
                        case nameof(Duration):
                            Duration = (float)reader.ReadAsDouble();
                            break;
                        case nameof(TimeStep):
                            TimeStep = (float)reader.ReadAsDouble();
                            break;
                        case nameof(MeshKeys):
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.StartArray)
                                {
                                    continue;
                                }
                                if (reader.TokenType == JsonToken.EndArray)
                                {
                                    break;
                                }

                                MeshKeys.Add((float)(double)reader.Value);
                            }
                            break;
                        default:
                            throw new Exception("Unexpected property name! " + reader.ReadAsString());
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

            writer.WritePropertyName(nameof(Duration));
            writer.WriteRawValue(Duration.ToRoundTripString());
            writer.WritePropertyName(nameof(TimeStep));
            writer.WriteRawValue(TimeStep.ToRoundTripString());

            writer.WritePropertyName(nameof(MeshKeys));
            writer.WriteStartArray();
            for (int i = 0; i < MeshKeys.Count; ++i)
            {
                writer.WriteRawValue(MeshKeys[i].ToRoundTripString());
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
