using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AoMEngineLibrary.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 ReadAsVector3(this JsonReader reader)
        {
            int count = 0;
            Vector3 v = new Vector3();

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

                switch(count)
                {
                    case 0:
                        v.X = (float)(double)reader.Value;
                        break;
                    case 1:
                        v.Y = (float)(double)reader.Value;
                        break;
                    case 2:
                        v.Z = (float)(double)reader.Value;
                        break;
                    default:
                        throw new JsonException("Vector3 can only have 3 values!");
                }
                ++count;
            }
            
            return v;
        }

        public static void WriteJson(this Vector3 v, JsonWriter writer)
        {
            writer.WriteStartArray();

            writer.WriteRawValue(v.X.ToRoundTripString());
            writer.WriteRawValue(v.Y.ToRoundTripString());
            writer.WriteRawValue(v.Z.ToRoundTripString());

            writer.WriteEndArray();
        }
    }
}
