namespace AoMEngineLibrary.Graphics.Model
{
    using Extensions;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Material
    {
        private Color3D diffuseColor;
        private Color3D ambientColor;
        private Color3D specularColor;
        private Color3D emissiveColor;

        #region Properties
        public virtual string Name { get; set; }
        public Color3D DiffuseColor
        {
            get
            {
                return diffuseColor;
            }
            set
            {
                diffuseColor = value;
            }
        }
        public Color3D AmbientColor
        {
            get
            {
                return ambientColor;
            }
            set
            {
                ambientColor = value;
            }
        }
        public Color3D SpecularColor
        {
            get
            {
                return specularColor;
            }
            set
            {
                specularColor = value;
            }
        }
        public Color3D EmissiveColor
        {
            get
            {
                return emissiveColor;
            }
            set
            {
                emissiveColor = value;
            }
        }

        public float Opacity { get; set; }
        public float SpecularExponent { get; set; }
        public bool FaceMap { get; set; }
        public bool TwoSided { get; set; }
        #endregion

        public Material()
        {
            this.Name = "Material";
            this.diffuseColor = new Color3D(1f);
            this.ambientColor = new Color3D(1f);
            this.specularColor = new Color3D(0f);
            this.emissiveColor = new Color3D(0f);

            this.Opacity = 1f;
            this.SpecularExponent = 0f;
            this.FaceMap = false;
            this.TwoSided = false;
        }

        public virtual void ReadJson(JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    ReadDataJson(reader, (string)reader.Value);
                }
                else
                {
                    throw new Exception("Unexpected token type! " + reader.TokenType);
                }
            }
        }

        protected virtual void ReadDataJson(JsonReader reader, string propName)
        {
            switch (propName)
            {
                case nameof(Name):
                    Name = reader.ReadAsString();
                    break;
                case nameof(DiffuseColor):
                    DiffuseColor.ReadJson(reader);
                    break;
                case nameof(AmbientColor):
                    AmbientColor.ReadJson(reader);
                    break;
                case nameof(SpecularColor):
                    SpecularColor.ReadJson(reader);
                    break;
                case nameof(EmissiveColor):
                    EmissiveColor.ReadJson(reader);
                    break;
                case nameof(Opacity):
                    Opacity = (float)reader.ReadAsDouble();
                    break;
                case nameof(SpecularExponent):
                    SpecularExponent = (float)reader.ReadAsDouble();
                    break;
                case nameof(FaceMap):
                    FaceMap = reader.ReadAsBoolean().Value;
                    break;
                case nameof(TwoSided):
                    TwoSided = reader.ReadAsBoolean().Value;
                    break;
                default:
                    break;
                    throw new Exception("Unexpected property name!");
            }
        }

        public virtual void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();

            WriteDataJson(writer);

            writer.WriteEndObject();
        }

        protected virtual void WriteDataJson(JsonWriter writer)
        {
            writer.WritePropertyName(nameof(Name));
            writer.WriteValue(Name);

            writer.WritePropertyName(nameof(DiffuseColor));
            DiffuseColor.WriteJson(writer);

            writer.WritePropertyName(nameof(AmbientColor));
            AmbientColor.WriteJson(writer);

            writer.WritePropertyName(nameof(SpecularColor));
            SpecularColor.WriteJson(writer);

            writer.WritePropertyName(nameof(EmissiveColor));
            EmissiveColor.WriteJson(writer);

            writer.WritePropertyName(nameof(Opacity));
            writer.WriteRawValue(Opacity.ToRoundTripString());

            writer.WritePropertyName(nameof(SpecularExponent));
            writer.WriteRawValue(SpecularExponent.ToRoundTripString());

            writer.WritePropertyName(nameof(FaceMap));
            writer.WriteValue(FaceMap);

            writer.WritePropertyName(nameof(TwoSided));
            writer.WriteValue(TwoSided);
        }
    }
}
