namespace AoMEngineLibrary.Graphics.Brg
{
    using AoMEngineLibrary.Graphics.Model;
    using Extensions;
    using Newtonsoft.Json;
    using System;
    using System.Numerics;

    public class BrgAttachpoint
    {
        public static string[] AttachpointNames = new string[55] { 
            "targetpoint", "launchpoint", "corpse", "decal", "fire", "gatherpoint", "reserved9", "reserved8", "reserved7", "reserved6", "reserved5", "reserved4", "reserved3", "reserved2", "reserved1", "reserved0", 
            "smoke9", "smoke8", "smoke7", "smoke6", "smoke5", "smoke4", "smoke3", "smoke2", "smoke1", "smoke0", "garrisonflag", "hitpointbar", "rightforearm", "leftforearm", "rightfoot", "leftfoot", 
            "rightleg", "leftleg", "rightthigh", "leftthigh", "pelvis", "backabdomen", "frontabdomen", "backchest", "frontchest", "rightshoulder", "leftshoulder", "neck", "rightear", "leftear", "chin", "face", 
            "forehead", "topofhead", "righthand", "lefthand", "reserved", "smokepoint", "attachpoint"
            //"TARGETPOINT", "LAUNCHPOINT", "CORPSE", "DECAL", "FIRE", "GATHERPOINT", "RESERVED9", "RESERVED8", "RESERVED7", "RESERVED6", "RESERVED5", "RESERVED4", "RESERVED3", "RESERVED2", "RESERVED1", "RESERVED0", 
            //"SMOKE9", "SMOKE8", "SMOKE7", "SMOKE6", "SMOKE5", "SMOKE4", "SMOKE3", "SMOKE2", "SMOKE1", "SMOKE0", "GARRISONFLAG", "HITPOINTBAR", "RIGHTFOREARM", "LEFTFOREARM", "RIGHTFOOT", "LEFTFOOT", 
            //"RIGHTLEG", "LEFTLEG", "RIGHTTHIGH", "LEFTTHIGH", "PELVIS", "BACKABDOMEN", "FRONTABDOMEN", "BACKCHEST", "FRONTCHEST", "RIGHTSHOULDER", "LEFTSHOULDER", "NECK", "RIGHTEAR", "LEFTEAR", "CHIN", "FACE", 
            //"FOREHEAD", "TOPOFHEAD", "RIGHTHAND", "LEFTHAND", "RESERVED", "SMOKEPOINT", "ATTACHPOINT"
         };

        public int Index;
        public int NameId;
        public Vector3 XVector;
        public Vector3 YVector;
        public Vector3 ZVector;
        public Vector3 Position;
        public Vector3 BoundingBoxMin;
        public Vector3 BoundingBoxMax;

        public string Name
        {
            get
            {
                if (NameId >= 0 && NameId <= 54)
                {
                    return AttachpointNames[54 - NameId];
                }
                else
                {
                    //return string.Empty;
                    throw new Exception("Invalid Attachpoint Name Id " + NameId + "!");
                }
            }
        }

        public BrgAttachpoint()
        {
            Index = -1;
            NameId = -1;
            XVector = new Vector3(0, 1, 0);
            YVector = new Vector3(0, 0, -1);
            ZVector = new Vector3(-1, 0, 0);
            Position = new Vector3(0f);
            BoundingBoxMin = new Vector3(-0.25f);
            BoundingBoxMax = new Vector3(0.25f);
        }
        public BrgAttachpoint(BrgAttachpoint prev)
        {
            Index = prev.Index;
            NameId = prev.NameId;
            XVector = prev.XVector;
            YVector = prev.YVector;
            ZVector = prev.ZVector;
            Position = prev.Position;
            BoundingBoxMin = prev.BoundingBoxMin;
            BoundingBoxMax = prev.BoundingBoxMax;
        }

        public static int GetIdByName(string name)
        {
            int ret;
            if (!BrgAttachpoint.TryGetIdByName(name, out ret))
            {
                throw new Exception("Invalid Attachpoint Name " + name + "!");
            }
            return ret;
        }
        public static bool TryGetIdByName(string name, out int nId)
        {
            if (name.Equals("righthandtag", StringComparison.InvariantCultureIgnoreCase))
            {
                nId = 4;
                return true;
            }

            if (name.Equals("lefthandtag", StringComparison.InvariantCultureIgnoreCase))
            {
                nId = 3;
                return true;
            }

            for (int i = 0; i < AttachpointNames.Length; i++)
            {
                if (AttachpointNames[i].Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    nId = 54 - i;
                    return true;
                }
            }

            nId = -1;
            return false;
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
                        case nameof(Name):
                            NameId = GetIdByName(reader.ReadAsString());
                            break;
                        case nameof(XVector):
                            XVector = reader.ReadAsVector3();
                            break;
                        case nameof(YVector):
                            YVector = reader.ReadAsVector3();
                            break;
                        case nameof(ZVector):
                            ZVector = reader.ReadAsVector3();
                            break;
                        case nameof(Position):
                            Position = reader.ReadAsVector3();
                            break;
                        case nameof(BoundingBoxMin):
                            BoundingBoxMin = reader.ReadAsVector3();
                            break;
                        case nameof(BoundingBoxMax):
                            BoundingBoxMax = reader.ReadAsVector3();
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

            writer.WritePropertyName(nameof(Name));
            writer.WriteValue(Name);

            writer.WritePropertyName(nameof(XVector));
            XVector.WriteJson(writer);

            writer.WritePropertyName(nameof(YVector));
            YVector.WriteJson(writer);

            writer.WritePropertyName(nameof(ZVector));
            ZVector.WriteJson(writer);

            writer.WritePropertyName(nameof(Position));
            Position.WriteJson(writer);

            writer.WritePropertyName(nameof(BoundingBoxMin));
            BoundingBoxMin.WriteJson(writer);

            writer.WritePropertyName(nameof(BoundingBoxMax));
            BoundingBoxMax.WriteJson(writer);

            writer.WriteEndObject();
        }
    }
}
