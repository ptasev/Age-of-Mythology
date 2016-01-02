namespace AoMEngineLibrary.Graphics.Brg
{
    using AoMEngineLibrary.Graphics.Model;
    using MiscUtil.Conversion;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    [XmlRootAttribute("Material", IsNullable = false)]
    public class MtrlFile
    {
        public uint[] unk; //5   

        [XmlElement(ElementName = "diffuse")]
        public Color3D Diffuse { get; set; }
        [XmlElement(ElementName = "ambient")]
        public Color3D Ambient { get; set; }
        [XmlElement(ElementName = "specular")]
        public Color3D Specular { get; set; }
        [XmlElement(ElementName = "emissive")]
        public Color3D Emissive { get; set; }
        [XmlElement(ElementName = "specular_power")]
        public float SpecularLevel { get; set; }
        [XmlElement(ElementName = "alpha")]
        public float Alpha { get; set; }

        [XmlElement(ElementName = "id")]
        public int Id { get; set; }

        [XmlElement(ElementName = "self_illuminating")]
        public byte SelfIlluminating { get; set; }
        [XmlElement(ElementName = "clamp_u")]
        public byte ClampU { get; set; }
        [XmlElement(ElementName = "clamp_v")]
        public byte ClampV { get; set; }
        [XmlElement(ElementName = "light_specular")]
        public byte LightSpecular { get; set; }
        [XmlElement(ElementName = "affects_ambient")]
        public byte AffectsAmbient { get; set; }
        [XmlElement(ElementName = "affects_diffuse")]
        public byte AffectsDiffuse { get; set; }
        [XmlElement(ElementName = "affects_specular")]
        public byte AffectsSpecular { get; set; }
        [XmlElement(ElementName = "updateable")]
        public byte Updateable { get; set; }

        [XmlElement(ElementName = "alpha_mode")]
        public int AlphaMode { get; set; } // Seems to be very often 10, wave has a 2 here, phoenix has 6
        [XmlElement(ElementName = "ambient_intensity")]
        public int AmbientIntensity { get; set; }
        [XmlElement(ElementName = "diffuse_intensity")]
        public int DiffuseIntensity { get; set; }
        [XmlElement(ElementName = "specular_intensity")]
        public int SpecularIntensity { get; set; }
        [XmlElement(ElementName = "emissive_intensity")]
        public int EmissiveIntensity { get; set; }
        [XmlElement(ElementName = "color_transform")]
        public int ColorTransform { get; set; } // Val of 4 seems to be PC
        [XmlElement(ElementName = "texture_transform")]
        public int TextureTransform { get; set; }
        [XmlElement(ElementName = "texture_factor")]
        public int TextureFactor { get; set; } // Has something to do with Cube Map
        [XmlElement(ElementName = "multitexture_mode")]
        public int MultiTextureMode { get; set; } // Has something to do with Cube Map
        [XmlElement(ElementName = "texgen_mode_0")]
        public int TexGenMode0 { get; set; }
        [XmlElement(ElementName = "texgen_mode_1")]
        public int TexGenMode1 { get; set; } // Has something to do with Cube Map
        [XmlElement(ElementName = "texcoord_set_0")]
        public int TexCoordSet0 { get; set; }
        [XmlElement(ElementName = "texcoord_set_1")]
        public int TexCoordSet1 { get; set; }
        [XmlElement(ElementName = "texcoord_set_2")]
        public int TexCoordSet2 { get; set; }
        [XmlElement(ElementName = "texcoord_set_3")]
        public int TexCoordSet3 { get; set; }
        [XmlElement(ElementName = "texcoord_set_4")]
        public int TexCoordSet4 { get; set; }
        [XmlElement(ElementName = "texcoord_set_5")]
        public int TexCoordSet5 { get; set; }
        [XmlElement(ElementName = "texcoord_set_6")]
        public int TexCoordSet6 { get; set; }
        [XmlElement(ElementName = "texcoord_set_7")]
        public int TexCoordSet7 { get; set; }

        public Texel[] unk2 { get; set; }
        public Texel[] unk3 { get; set; }
        public int[] unk4 { get; set; }

        [XmlElement(ElementName = "texture")]
        public string Texture { get; set; }

        public MtrlFile()
        {
            this.unk = new uint[5];
            this.Alpha = 1f;

            this.Id = -1;

            this.SelfIlluminating = 0;
            this.ClampU = 0;
            this.ClampV = 0;
            this.LightSpecular = 1;
            this.AffectsAmbient = 1;
            this.AffectsDiffuse = 1;
            this.AffectsSpecular = 1;

            this.AlphaMode = 10;

            unk2 = new Texel[3] { new Color4D(1f), new Color4D(1f), new Color4D(1f) };
            unk3 = new Texel[3] { new Color4D(1f), new Color4D(1f), new Color4D(1f) };
            unk4 = new int[4];
        }
        public MtrlFile(BrgMaterial mat)
            : this()
        {
            this.Diffuse = mat.DiffuseColor;
            this.Ambient = mat.AmbientColor;
            this.Specular = mat.SpecularColor;
            this.Emissive = mat.EmissiveColor;
            this.SpecularLevel = mat.SpecularExponent;
            this.Alpha = mat.Opacity;

            if (!mat.Flags.HasFlag(BrgMatFlag.WrapUTx1))
            {
                this.ClampU = 1;
            }

            if (!mat.Flags.HasFlag(BrgMatFlag.WrapVTx1))
            {
                this.ClampV = 1;
            }

            if (mat.Flags.HasFlag(BrgMatFlag.AdditiveBlend))
            {
                this.AlphaMode = 6;
            }

            if (mat.Flags.HasFlag(BrgMatFlag.PixelXForm1))
            {
                this.ColorTransform = 4;
            }

            if (mat.Flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
            {
                this.TextureFactor = 1275068416;
                this.MultiTextureMode = 12;
                this.TexGenMode1 = 1;
            }

            this.Texture = mat.DiffuseMap;
        }

        public void Read(Stream stream)
        {
            using (BrgBinaryReader reader = new BrgBinaryReader(new LittleEndianBitConverter(), stream))
            {
                uint magic = reader.ReadUInt32();
                if (magic != 1280463949)
                {
                    throw new Exception("This is not a MTRL file!");
                }

                uint nameLength = reader.ReadUInt32();
                for (int i = 0; i < 5; ++i)
                {
                    this.unk[i] = reader.ReadUInt32();
                }

                this.Diffuse = reader.ReadColor3D();
                this.Ambient = reader.ReadColor3D();
                this.Specular = reader.ReadColor3D();
                this.Emissive = reader.ReadColor3D();
                this.SpecularLevel = reader.ReadSingle();
                this.Alpha = reader.ReadSingle();

                this.Id = reader.ReadInt32();

                this.SelfIlluminating = reader.ReadByte();
                this.ClampU = reader.ReadByte();
                this.ClampV = reader.ReadByte();
                this.LightSpecular = reader.ReadByte();
                this.AffectsAmbient = reader.ReadByte();
                this.AffectsDiffuse = reader.ReadByte();
                this.AffectsSpecular = reader.ReadByte();
                this.Updateable = reader.ReadByte();

                this.AlphaMode = reader.ReadInt32(); // Seems to be very often 10, wave has a 2 here, phoenix has 6
                this.AmbientIntensity = reader.ReadInt32();
                this.DiffuseIntensity = reader.ReadInt32();
                this.SpecularIntensity = reader.ReadInt32();
                this.EmissiveIntensity = reader.ReadInt32();
                this.ColorTransform = reader.ReadInt32(); // Val of 4 seems to be PC
                this.TextureTransform = reader.ReadInt32();
                this.TextureFactor = reader.ReadInt32(); // Has something to do with Cube Map
                this.MultiTextureMode = reader.ReadInt32(); // Has something to do with Cube Map
                this.TexGenMode0 = reader.ReadInt32();
                this.TexGenMode1 = reader.ReadInt32(); // Has something to do with Cube Map
                this.TexCoordSet0 = reader.ReadInt32();
                this.TexCoordSet1 = reader.ReadInt32();
                this.TexCoordSet2 = reader.ReadInt32();
                this.TexCoordSet3 = reader.ReadInt32();
                this.TexCoordSet4 = reader.ReadInt32();
                this.TexCoordSet5 = reader.ReadInt32();
                this.TexCoordSet6 = reader.ReadInt32();
                this.TexCoordSet7 = reader.ReadInt32();

                for (int i = 0; i < 3; ++i)
                {
                    this.unk2[i] = reader.ReadTexel();
                }
                for (int i = 0; i < 3; ++i)
                {
                    this.unk3[i] = reader.ReadTexel();
                }
                for (int i = 0; i < 4; ++i)
                {
                    this.unk4[i] = reader.ReadInt32();
                }

                this.Texture = reader.ReadString();
            }
        }

        public void Write(Stream stream)
        {
            using (BrgBinaryWriter writer = new BrgBinaryWriter(new LittleEndianBitConverter(), stream))
            {
                writer.Write(1280463949); // MTRL
                writer.Write((uint)Encoding.UTF8.GetByteCount(this.Texture));
                for (int i = 0; i < 5; ++i)
                {
                    writer.Write(this.unk[i]);
                }

                writer.WriteColor3D(this.Diffuse);
                writer.WriteColor3D(this.Ambient);
                writer.WriteColor3D(this.Specular);
                writer.WriteColor3D(this.Emissive);
                writer.Write(this.SpecularLevel);
                writer.Write(this.Alpha);

                writer.Write(this.Id);

                writer.Write(this.SelfIlluminating);
                writer.Write(this.ClampU);
                writer.Write(this.ClampV);
                writer.Write(this.LightSpecular);
                writer.Write(this.AffectsAmbient);
                writer.Write(this.AffectsDiffuse);
                writer.Write(this.AffectsSpecular);
                writer.Write(this.Updateable);

                writer.Write(this.AlphaMode); // Seems to be very often 10, wave has a 2 here, phoenix has 6
                writer.Write(this.AmbientIntensity);
                writer.Write(this.DiffuseIntensity);
                writer.Write(this.SpecularIntensity);
                writer.Write(this.EmissiveIntensity);
                writer.Write(this.ColorTransform); // Val of 4 seems to be PC
                writer.Write(this.TextureTransform);
                writer.Write(this.TextureFactor); // Has something to do with Cube Map
                writer.Write(this.MultiTextureMode); // Has something to do with Cube Map
                writer.Write(this.TexGenMode0);
                writer.Write(this.TexGenMode1); // Has something to do with Cube Map
                writer.Write(this.TexCoordSet0);
                writer.Write(this.TexCoordSet1);
                writer.Write(this.TexCoordSet2);
                writer.Write(this.TexCoordSet3);
                writer.Write(this.TexCoordSet4);
                writer.Write(this.TexCoordSet5);
                writer.Write(this.TexCoordSet6);
                writer.Write(this.TexCoordSet7);

                for (int i = 0; i < 3; ++i)
                {
                    writer.WriteTexel(this.unk2[i]);
                }
                for (int i = 0; i < 3; ++i)
                {
                    writer.WriteTexel(this.unk3[i]);
                }
                for (int i = 0; i < 4; ++i)
                {
                    writer.Write(this.unk4[i]);
                }

                writer.WriteString(this.Texture);
            }
        }

        public void SerializeAsXml(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MtrlFile));
            using (TextWriter writer = new StreamWriter(stream))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static MtrlFile DeserializeAsXml(Stream stream)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(MtrlFile));
            using (TextReader reader = new StreamReader(stream))
            {
                return deserializer.Deserialize(reader) as MtrlFile;
            }
        }
    }
}
