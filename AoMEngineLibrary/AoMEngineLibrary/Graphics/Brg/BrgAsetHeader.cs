using AoMEngineLibrary.Extensions;
using Newtonsoft.Json;
using System;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgAsetHeader
    {
        public int numFrames;
        public float frameStep;
        public float animTime;
        public float frequency;
        public float spf;
        public float fps;
        public int space;

        public BrgAsetHeader()
        {
        }
        public BrgAsetHeader(BrgBinaryReader reader)
        {
            this.numFrames = reader.ReadInt32();
            this.frameStep = reader.ReadSingle();
            this.animTime = reader.ReadSingle();
            this.frequency = reader.ReadSingle();
            this.spf = reader.ReadSingle();
            this.fps = reader.ReadSingle();
            this.space = reader.ReadInt32();
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(this.numFrames);
            writer.Write(this.frameStep);
            writer.Write(this.animTime);
            writer.Write(this.frequency);
            writer.Write(this.spf);
            writer.Write(this.fps);
            writer.Write(this.space);
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
                        case nameof(numFrames):
                            numFrames = reader.ReadAsInt32().Value;
                            break;
                        case nameof(frameStep):
                            frameStep = (float)reader.ReadAsDouble().Value;
                            break;
                        case nameof(animTime):
                            animTime = (float)reader.ReadAsDouble().Value;
                            break;
                        case nameof(frequency):
                            frequency = (float)reader.ReadAsDouble().Value;
                            break;
                        case nameof(spf):
                            spf = (float)reader.ReadAsDouble().Value;
                            break;
                        case nameof(fps):
                            fps = (float)reader.ReadAsDouble().Value;
                            break;
                        case nameof(space):
                            space = reader.ReadAsInt32().Value;
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

            writer.WritePropertyName("numFrames");
            writer.WriteValue(numFrames);
            writer.WritePropertyName("frameStep");
            writer.WriteRawValue(frameStep.ToRoundTripString());
            writer.WritePropertyName("animTime");
            writer.WriteRawValue(animTime.ToRoundTripString());
            writer.WritePropertyName("frequency");
            writer.WriteRawValue(frequency.ToRoundTripString());
            writer.WritePropertyName("spf");
            writer.WriteRawValue(spf.ToRoundTripString());
            writer.WritePropertyName("fps");
            writer.WriteRawValue(fps.ToRoundTripString());
            writer.WritePropertyName("space");
            writer.WriteValue(space);

            writer.WriteEndObject();
        }
    }
}
