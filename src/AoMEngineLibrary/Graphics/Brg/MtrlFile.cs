namespace AoMEngineLibrary.Graphics.Brg
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public enum MtrlAlphaMode
    {
        Off,
        OneBit,
        On,
        Lightmap,
        LightmapInAlpha,
        InvertedLightmapInAlpha,
        Additive,
        AdditiveSquared,
        AdditiveModulateSrcAlpha,
        Subtractive,
        Texture
    }

    public enum MtrlColorTransformMode
    {
        TransformNone,
        Transform1,
        Transform2,
        Transform3,
        TransformPixel1,
        TransformPixel2,
        TransformPixel3,
    }

    public enum MtrlTextureTransformMode
    {
        TransformNone,
        Transform1,
        Transform2
    }

    public enum MtrlMultitextureMode
    {
        Off,
        OffNoVtxAlpha,
        OffNoTx,
        OffOnlyAlpha,
        LinearBlend,
        EmissiveSpecularFactor,
        EmissiveSpecularFactorNoTx,
        SpecularBump,
        LinearBlendAlpha,
        LinearBlendInverseAlpha,
        LinearBlendAlphaEmissiveSpecularFactor,
        LinearBlendEmissiveSpecularFactor,
        AddFactor,
        AddAlpha,
        AddInverseAlpha,
        SeparateAlpha,
        Lightmap,
        FogMask,
        FadeLightmapByAlpha
    }
    public enum MtrlTexGenMode
    {
        Disable,
        CubicEnvironment,
        FakeReflection,
        XZPosition
    }

    public class MtrlFile
    {
        private const string Zero = "0";
        private const string IndentString = "    ";
        private const float Epsilon = 0.000001f;

        public uint TextureNameLength { get; private set; }
        public uint SecondaryTextureNameLength { get; private set; }
        public uint BumpMapNameLength { get; private set; }
        public uint SpecMapNameLength { get; private set; }
        public uint GlossMapNameLength { get; private set; }
        public uint EmissiveMapNameLength { get; private set; }

        public Vector3 Diffuse { get; set; }
        public Vector3 Ambient { get; set; }
        public Vector3 Specular { get; set; }
        public Vector3 Emissive { get; set; }
        public float SpecularPower { get; set; }
        public float Alpha { get; set; }

        public int Id { get; set; }

        public byte SelfIlluminating { get; set; }
        public byte ClampU { get; set; }
        public byte ClampV { get; set; }
        public byte LightSpecular { get; set; }
        public byte AffectsAmbient { get; set; }
        public byte AffectsDiffuse { get; set; }
        public byte AffectsSpecular { get; set; }
        public byte Updateable { get; set; }

        public MtrlAlphaMode AlphaMode { get; set; } // Seems to be very often 10, wave has a 2 here, phoenix has 6
        public float AmbientIntensity { get; set; }
        public float DiffuseIntensity { get; set; }
        public float SpecularIntensity { get; set; }
        public float EmissiveIntensity { get; set; }
        public MtrlColorTransformMode ColorTransform { get; set; } // Val of 4 seems to be PC
        public MtrlTextureTransformMode TextureTransform { get; set; }
        public uint TextureFactor { get; set; } // Has something to do with Cube Map
        public MtrlMultitextureMode MultiTextureMode { get; set; } // Has something to do with Cube Map
        public MtrlTexGenMode TexGenMode0 { get; set; }
        public MtrlTexGenMode TexGenMode1 { get; set; } // Has something to do with Cube Map
        public int TexCoordSet0 { get; set; }
        public int TexCoordSet1 { get; set; }
        public int TexCoordSet2 { get; set; }
        public int TexCoordSet3 { get; set; }
        public int TexCoordSet4 { get; set; }
        public int TexCoordSet5 { get; set; }
        public int TexCoordSet6 { get; set; }
        public int TexCoordSet7 { get; set; }
        
        public int TextureIndex { get; set; }
        public int SecondaryTextureIndex { get; set; }
        public int BumpMapIndex { get; set; }

        public int SpecularMapIndex { get; set; }
        public int GlossMapIndex { get; set; }
        public int EmissiveMapIndex { get; set; }

        public int[] Reserved { get; set; }

        private string _texture;
        public string Texture
        {
            get => _texture;
            set
            {
                _texture = value;
                TextureNameLength = (byte)Encoding.UTF8.GetByteCount(value);
            }
        }

        private string _secondaryTexture;
        public string SecondaryTexture
        {
            get => _secondaryTexture;
            set
            {
                _secondaryTexture = value;
                SecondaryTextureNameLength = (byte)Encoding.UTF8.GetByteCount(value);
            }
        }

        private string _bumpMap;
        public string BumpMap
        {
            get => _bumpMap;
            set
            {
                _bumpMap = value;
                BumpMapNameLength = (byte)Encoding.UTF8.GetByteCount(value);
            }
        }

        private string _specMap;
        public string SpecMap
        {
            get => _specMap;
            set
            {
                _specMap = value;
                SpecMapNameLength = (byte)Encoding.UTF8.GetByteCount(value);
            }
        }

        private string _glossMap;
        public string GlossMap
        {
            get => _glossMap;
            set
            {
                _glossMap = value;
                GlossMapNameLength = (byte)Encoding.UTF8.GetByteCount(value);
            }
        }

        private string _emissiveMap;
        public string EmissiveMap
        {
            get => _emissiveMap;
            set
            {
                _emissiveMap = value;
                EmissiveMapNameLength = (byte)Encoding.UTF8.GetByteCount(value);
            }
        }

        public MtrlFile()
        {
            this.Alpha = 1f;

            this.Id = -1;

            this.SelfIlluminating = 0;
            this.ClampU = 0;
            this.ClampV = 0;
            this.LightSpecular = 1;
            this.AffectsAmbient = 1;
            this.AffectsDiffuse = 1;
            this.AffectsSpecular = 1;

            this.AlphaMode = MtrlAlphaMode.Texture;

            this.TextureIndex = -1;
            this.SecondaryTextureIndex = -1;
            this.BumpMapIndex = -1;

            this.SpecularMapIndex = -1;
            this.GlossMapIndex = -1;
            this.EmissiveMapIndex = -1;

            Reserved = new int[4];

            _texture = string.Empty;
            _secondaryTexture = string.Empty;
            _bumpMap = string.Empty;
            _specMap = string.Empty;
            _glossMap = string.Empty;
            _emissiveMap = string.Empty;
        }
        public MtrlFile(BrgMaterial mat)
            : this()
        {
            this.Diffuse = mat.DiffuseColor;
            this.Ambient = mat.AmbientColor;
            this.Specular = mat.SpecularColor;
            this.Emissive = mat.EmissiveColor;
            this.SpecularPower = mat.SpecularExponent;
            this.Alpha =  mat.Opacity > 1 ? 1 : (mat.Opacity < 0 ? 0 : mat.Opacity);

            this.DiffuseIntensity = 0.299f * Diffuse.X + 0.587f * Diffuse.Y + 0.114f * Diffuse.Z;
            this.AmbientIntensity = 0.299f * Diffuse.X + 0.587f * Diffuse.Y + 0.114f * Diffuse.Z;
            this.SpecularIntensity = 0.299f * Diffuse.X + 0.587f * Diffuse.Y + 0.114f * Diffuse.Z;
            this.EmissiveIntensity = 0.299f * Diffuse.X + 0.587f * Diffuse.Y + 0.114f * Diffuse.Z;

            if ((Math.Abs(Diffuse.X - 1) >= Epsilon) || (Math.Abs(Diffuse.Y - 1) >= Epsilon) || (Math.Abs(Diffuse.Z - 1) >= Epsilon))
                this.AffectsDiffuse = 1;
            if ((Math.Abs(Ambient.X - 1) >= Epsilon) || (Math.Abs(Ambient.Y - 1) >= Epsilon) || (Math.Abs(Ambient.Z - 1) >= Epsilon))
                this.AffectsAmbient = 1;
            if ((Math.Abs(Specular.X - 1) >= Epsilon) || (Math.Abs(Specular.Y - 1) >= Epsilon) || (Math.Abs(Specular.Z - 1) >= Epsilon))
                this.AffectsSpecular = 1;
            if ((Emissive.X > Epsilon) || (Emissive.Y > Epsilon) || (Emissive.Z > Epsilon))
                this.SelfIlluminating = 1;

            if (mat.SpecularColor.X >= Epsilon || mat.SpecularColor.Y >= Epsilon || mat.SpecularColor.Z >= Epsilon)
                this.LightSpecular = 1;

            if (!mat.Flags.HasFlag(BrgMatFlag.WrapUTx1))
            {
                this.ClampU = 1;
            }

            if (!mat.Flags.HasFlag(BrgMatFlag.WrapVTx1))
            {
                this.ClampV = 1;
            }

            if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormColor1))
            {
                this.ColorTransform = MtrlColorTransformMode.Transform1;
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormColor2))
            {
                this.ColorTransform = MtrlColorTransformMode.Transform2;
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormColor3))
            {
                this.ColorTransform = MtrlColorTransformMode.Transform3;
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PixelXForm1))
            {
                this.ColorTransform = MtrlColorTransformMode.TransformPixel1;
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PixelXForm2))
            {
                this.ColorTransform = MtrlColorTransformMode.TransformPixel2;
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PixelXForm3))
            {
                this.ColorTransform = MtrlColorTransformMode.TransformPixel3;
            }

            if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormTx1))
            {
                this.TextureTransform = MtrlTextureTransformMode.Transform1;
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormTx2))
            {
                this.TextureTransform = MtrlTextureTransformMode.Transform2;
            }

            this.Texture = mat.DiffuseMapName;
            this.BumpMap = mat.BumpMapName;

            if (mat.Flags.HasFlag(BrgMatFlag.CubeMapInfo))
            {
                this.SecondaryTexture = mat.CubeMapInfo.CubeMapName;

                switch (mat.CubeMapInfo.Mode)
                {
                    case 0:
                        this.TexGenMode1 = MtrlTexGenMode.CubicEnvironment;

                        if (mat.Flags.HasFlag(BrgMatFlag.AdditiveCubeBlend))
                        {
                            if (mat.Flags.HasFlag(BrgMatFlag.InverseAlpha))
                                this.MultiTextureMode = MtrlMultitextureMode.AddInverseAlpha;
                            else
                                this.MultiTextureMode = MtrlMultitextureMode.AddAlpha;
                        }
                        else
                        {
                            if (mat.Flags.HasFlag(BrgMatFlag.InverseAlpha))
                                this.MultiTextureMode = MtrlMultitextureMode.LinearBlendInverseAlpha;
                            else
                                this.MultiTextureMode = MtrlMultitextureMode.LinearBlendAlpha;
                        }
                        break;
                    case 1:
                        this.TexGenMode1 = MtrlTexGenMode.CubicEnvironment;

                        if (mat.Flags.HasFlag(BrgMatFlag.AdditiveCubeBlend))
                            this.MultiTextureMode = MtrlMultitextureMode.AddFactor;
                        else
                            this.MultiTextureMode = MtrlMultitextureMode.LinearBlend;

                        uint txFactor = (byte)(255.0f * (mat.CubeMapInfo.TextureFactor / 100.0f));
                        this.TextureFactor = txFactor << 24;
                        break;
                    default:
                        throw new NotImplementedException($"Unsupported cube map mode {mat.CubeMapInfo.Mode}.");
                }
            }

            if (mat.Flags.HasFlag(BrgMatFlag.Alpha))
            {
                if ((1 - this.Alpha) > Epsilon)
                    this.AlphaMode = MtrlAlphaMode.On;
                else
                    this.AlphaMode = MtrlAlphaMode.Texture;
            }

            if ((ColorTransform == MtrlColorTransformMode.TransformPixel1 ||
                ColorTransform == MtrlColorTransformMode.TransformPixel2 ||
                ColorTransform == MtrlColorTransformMode.TransformPixel3) &&
                (AlphaMode == MtrlAlphaMode.On))
                AlphaMode = MtrlAlphaMode.Off;

            if (mat.Flags.HasFlag(BrgMatFlag.AdditiveBlend))
            {
                this.AlphaMode = MtrlAlphaMode.Additive;
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.SubtractiveBlend))
            {
                this.AlphaMode = MtrlAlphaMode.Subtractive;
            }
        }

        public void Read(Stream stream)
        {
            using (BrgBinaryReader reader = new BrgBinaryReader(stream))
            {
                uint magic = reader.ReadUInt32();
                if (magic != 1280463949)
                {
                    throw new Exception("This is not a MTRL file!");
                }

                TextureNameLength = reader.ReadUInt32();
                SecondaryTextureNameLength = reader.ReadUInt32();
                BumpMapNameLength = reader.ReadUInt32();
                SpecMapNameLength = reader.ReadUInt32();
                GlossMapNameLength = reader.ReadUInt32();
                EmissiveMapNameLength = reader.ReadUInt32();

                this.Diffuse = reader.ReadVector3D(false);
                this.Ambient = reader.ReadVector3D(false);
                this.Specular = reader.ReadVector3D(false);
                this.Emissive = reader.ReadVector3D(false);
                this.SpecularPower = reader.ReadSingle();
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

                this.AlphaMode = (MtrlAlphaMode)reader.ReadInt32(); // wave has a 2 here, phoenix has 6
                this.AmbientIntensity = reader.ReadSingle();
                this.DiffuseIntensity = reader.ReadSingle();
                this.SpecularIntensity = reader.ReadSingle();
                this.EmissiveIntensity = reader.ReadSingle();
                this.ColorTransform = (MtrlColorTransformMode)reader.ReadInt32();
                this.TextureTransform = (MtrlTextureTransformMode)reader.ReadInt32();
                this.TextureFactor = reader.ReadUInt32();
                this.MultiTextureMode = (MtrlMultitextureMode)reader.ReadInt32();
                this.TexGenMode0 = (MtrlTexGenMode)reader.ReadInt32();
                this.TexGenMode1 = (MtrlTexGenMode)reader.ReadInt32();
                this.TexCoordSet0 = reader.ReadInt32();
                this.TexCoordSet1 = reader.ReadInt32();
                this.TexCoordSet2 = reader.ReadInt32();
                this.TexCoordSet3 = reader.ReadInt32();
                this.TexCoordSet4 = reader.ReadInt32();
                this.TexCoordSet5 = reader.ReadInt32();
                this.TexCoordSet6 = reader.ReadInt32();
                this.TexCoordSet7 = reader.ReadInt32();

                this.TextureIndex = reader.ReadInt32();
                this.SecondaryTextureIndex = reader.ReadInt32();
                this.BumpMapIndex = reader.ReadInt32();

                this.SpecularMapIndex = reader.ReadInt32();
                this.GlossMapIndex = reader.ReadInt32();
                this.EmissiveMapIndex = reader.ReadInt32();

                for (int i = 0; i < 4; ++i)
                {
                    this.Reserved[i] = reader.ReadInt32();
                }

                if (TextureNameLength > 0)
                {
                    this.Texture = reader.ReadString();
                }

                if (SecondaryTextureNameLength > 0)
                {
                    this.SecondaryTexture = reader.ReadString();
                }

                if (BumpMapNameLength > 0)
                {
                    this.BumpMap = reader.ReadString();
                }

                if (SpecMapNameLength > 0)
                {
                    this.SpecMap = reader.ReadString();
                }

                if (GlossMapNameLength > 0)
                {
                    this.GlossMap = reader.ReadString();
                }

                if (EmissiveMapNameLength > 0)
                {
                    this.EmissiveMap = reader.ReadString();
                }
            }
        }

        public void Write(Stream stream)
        {
            using (BrgBinaryWriter writer = new BrgBinaryWriter(stream, true))
            {
                writer.Write(1280463949); // MTRL

                writer.Write(TextureNameLength);
                writer.Write(SecondaryTextureNameLength);
                writer.Write(BumpMapNameLength);
                writer.Write(SpecMapNameLength);
                writer.Write(GlossMapNameLength);
                writer.Write(EmissiveMapNameLength);

                writer.WriteVector3D(this.Diffuse, false);
                writer.WriteVector3D(this.Ambient, false);
                writer.WriteVector3D(this.Specular, false);
                writer.WriteVector3D(this.Emissive, false);
                writer.Write(this.SpecularPower);
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

                writer.Write((int)this.AlphaMode); // Seems to be very often 10, wave has a 2 here, phoenix has 6
                writer.Write(this.AmbientIntensity);
                writer.Write(this.DiffuseIntensity);
                writer.Write(this.SpecularIntensity);
                writer.Write(this.EmissiveIntensity);
                writer.Write((int)this.ColorTransform); // Val of 4 seems to be PC
                writer.Write((int)this.TextureTransform);
                writer.Write(this.TextureFactor); // Has something to do with Cube Map
                writer.Write((int)this.MultiTextureMode); // Has something to do with Cube Map
                writer.Write((int)this.TexGenMode0);
                writer.Write((int)this.TexGenMode1); // Has something to do with Cube Map
                writer.Write(this.TexCoordSet0);
                writer.Write(this.TexCoordSet1);
                writer.Write(this.TexCoordSet2);
                writer.Write(this.TexCoordSet3);
                writer.Write(this.TexCoordSet4);
                writer.Write(this.TexCoordSet5);
                writer.Write(this.TexCoordSet6);
                writer.Write(this.TexCoordSet7);

                writer.Write(this.TextureIndex);
                writer.Write(this.SecondaryTextureIndex);
                writer.Write(this.BumpMapIndex);

                writer.Write(this.SpecularMapIndex);
                writer.Write(this.GlossMapIndex);
                writer.Write(this.EmissiveMapIndex);

                for (int i = 0; i < 4; ++i)
                {
                    writer.Write(this.Reserved[i]);
                }

                if (TextureNameLength > 0)
                {
                    writer.WriteString(this.Texture);
                }

                if (SecondaryTextureNameLength > 0)
                {
                    writer.WriteString(this.SecondaryTexture);
                }

                if (BumpMapNameLength > 0)
                {
                    writer.WriteString(this.BumpMap);
                }

                if (SpecMapNameLength > 0)
                {
                    writer.WriteString(this.SpecMap);
                }

                if (GlossMapNameLength > 0)
                {
                    writer.WriteString(this.GlossMap);
                }

                if (EmissiveMapNameLength > 0)
                {
                    writer.WriteString(this.EmissiveMap);
                }
            }
        }

        public void SerializeAsXml(Stream stream)
        {
            XDocument xdoc = new XDocument(new XElement("Material"));
            var elem = (XElement)xdoc.FirstNode;
            elem.Add(new XElement("diffuse", new XAttribute("R", Diffuse.X), new XAttribute("G", Diffuse.Y), new XAttribute("B", Diffuse.Z)));
            elem.Add(new XElement("ambient", new XAttribute("R", Ambient.X), new XAttribute("G", Ambient.Y), new XAttribute("B", Ambient.Z)));
            elem.Add(new XElement("specular", new XAttribute("R", Specular.X), new XAttribute("G", Specular.Y), new XAttribute("B", Specular.Z)));
            elem.Add(new XElement("emissive", new XAttribute("R", Emissive.X), new XAttribute("G", Emissive.Y), new XAttribute("B", Emissive.Z)));

            elem.Add(new XElement("specular_power", SpecularPower));
            elem.Add(new XElement("alpha", Alpha));
            elem.Add(new XElement("id", Id));
            elem.Add(new XElement("self_illuminating", SelfIlluminating));
            elem.Add(new XElement("clamp_u", ClampU));
            elem.Add(new XElement("clamp_v", ClampV));

            elem.Add(new XElement("light_specular", LightSpecular));
            elem.Add(new XElement("affects_ambient", AffectsAmbient));
            elem.Add(new XElement("affects_diffuse", AffectsDiffuse));
            elem.Add(new XElement("affects_specular", AffectsSpecular));
            elem.Add(new XElement("updateable", Updateable));
            elem.Add(new XElement("alpha_mode", AlphaMode));

            elem.Add(new XElement("ambient_intensity", AmbientIntensity));
            elem.Add(new XElement("diffuse_intensity", DiffuseIntensity));
            elem.Add(new XElement("specular_intensity", SpecularIntensity));
            elem.Add(new XElement("emissive_intensity", EmissiveIntensity));

            elem.Add(new XElement("color_transform", ColorTransform));
            elem.Add(new XElement("texture_transform", TextureTransform));
            elem.Add(new XElement("texture_factor", TextureFactor));
            elem.Add(new XElement("multitexture_mode", MultiTextureMode));

            elem.Add(new XElement("texgen_mode_0", TexGenMode0));
            elem.Add(new XElement("texgen_mode_1", TexGenMode1));
            elem.Add(new XElement("texcoord_set_0", TexCoordSet0));
            elem.Add(new XElement("texcoord_set_1", TexCoordSet1));
            elem.Add(new XElement("texcoord_set_2", TexCoordSet2));
            elem.Add(new XElement("texcoord_set_3", TexCoordSet3));
            elem.Add(new XElement("texcoord_set_4", TexCoordSet4));
            elem.Add(new XElement("texcoord_set_5", TexCoordSet5));
            elem.Add(new XElement("texcoord_set_6", TexCoordSet6));
            elem.Add(new XElement("texcoord_set_7", TexCoordSet7));

            elem.Add(new XElement("texture", Texture));
            elem.Add(new XElement("secondary_texture", SecondaryTexture));
            elem.Add(new XElement("bumpmap", BumpMap));
            elem.Add(new XElement("specmap", SpecMap));
            elem.Add(new XElement("glossmap", GlossMap));
            elem.Add(new XElement("emissivemap", EmissiveMap));

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = IndentString,
                CloseOutput = false
            };
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                xdoc.Save(writer);
            }
        }

        public static MtrlFile DeserializeAsXml(Stream stream)
        {
            MtrlFile file = new MtrlFile();
            XDocument xdoc = XDocument.Load(stream);
            XElement elem = xdoc.Root;

            foreach (var e in elem.Elements())
            {
                switch (e.Name.LocalName)
                {
                    case "diffuse":
                        file.Diffuse = new Vector3(float.Parse(e.Attribute("R")?.Value ?? Zero), float.Parse(e.Attribute("G")?.Value ?? Zero), float.Parse(e.Attribute("B")?.Value ?? Zero));
                        break;
                    case "ambient":
                        file.Ambient = new Vector3(float.Parse(e.Attribute("R")?.Value ?? Zero), float.Parse(e.Attribute("G")?.Value ?? Zero), float.Parse(e.Attribute("B")?.Value ?? Zero));
                        break;
                    case "specular":
                        file.Specular = new Vector3(float.Parse(e.Attribute("R")?.Value ?? Zero), float.Parse(e.Attribute("G")?.Value ?? Zero), float.Parse(e.Attribute("B")?.Value ?? Zero));
                        break;
                    case "emissive":
                        file.Emissive = new Vector3(float.Parse(e.Attribute("R")?.Value ?? Zero), float.Parse(e.Attribute("G")?.Value ?? Zero), float.Parse(e.Attribute("B")?.Value ?? Zero));
                        break;
                    case "specular_power":
                        file.SpecularPower = float.Parse(e.Value);
                        break;
                    case "alpha":
                        file.Alpha = float.Parse(e.Value);
                        break;
                    case "id":
                        file.Id = int.Parse(e.Value);
                        break;
                    case "self_illuminating":
                        file.SelfIlluminating = byte.Parse(e.Value);
                        break;
                    case "clamp_u":
                        file.ClampU = byte.Parse(e.Value);
                        break;
                    case "clamp_v":
                        file.ClampV = byte.Parse(e.Value);
                        break;
                    case "light_specular":
                        file.LightSpecular = byte.Parse(e.Value);
                        break;
                    case "affects_ambient":
                        file.AffectsAmbient = byte.Parse(e.Value);
                        break;
                    case "affects_diffuse":
                        file.AffectsDiffuse = byte.Parse(e.Value);
                        break;
                    case "affects_specular":
                        file.AffectsSpecular = byte.Parse(e.Value);
                        break;
                    case "updateable":
                        file.Updateable = byte.Parse(e.Value);
                        break;
                    case "alpha_mode":
                        file.AlphaMode = (MtrlAlphaMode)int.Parse(e.Value);
                        break;
                    case "ambient_intensity":
                        file.AmbientIntensity = float.Parse(e.Value);
                        break;
                    case "diffuse_intensity":
                        file.DiffuseIntensity = float.Parse(e.Value);
                        break;
                    case "specular_intensity":
                        file.SpecularIntensity = float.Parse(e.Value);
                        break;
                    case "emissive_intensity":
                        file.EmissiveIntensity = float.Parse(e.Value);
                        break;
                    case "color_transform":
                        file.ColorTransform = (MtrlColorTransformMode)int.Parse(e.Value);
                        break;
                    case "texture_transform":
                        file.TextureTransform = (MtrlTextureTransformMode)int.Parse(e.Value);
                        break;
                    case "texture_factor":
                        file.TextureFactor = uint.Parse(e.Value);
                        break;
                    case "multitexture_mode":
                        file.MultiTextureMode = (MtrlMultitextureMode)int.Parse(e.Value);
                        break;
                    case "texgen_mode_0":
                        file.TexGenMode0 = (MtrlTexGenMode)int.Parse(e.Value);
                        break;
                    case "texgen_mode_1":
                        file.TexGenMode1 = (MtrlTexGenMode)int.Parse(e.Value);
                        break;
                    case "texcoord_set_0":
                        file.TexCoordSet0 = int.Parse(e.Value);
                        break;
                    case "texcoord_set_1":
                        file.TexCoordSet1 = int.Parse(e.Value);
                        break;
                    case "texcoord_set_2":
                        file.TexCoordSet2 = int.Parse(e.Value);
                        break;
                    case "texcoord_set_3":
                        file.TexCoordSet3 = int.Parse(e.Value);
                        break;
                    case "texcoord_set_4":
                        file.TexCoordSet4 = int.Parse(e.Value);
                        break;
                    case "texcoord_set_5":
                        file.TexCoordSet5 = int.Parse(e.Value);
                        break;
                    case "texcoord_set_6":
                        file.TexCoordSet6 = int.Parse(e.Value);
                        break;
                    case "texcoord_set_7":
                        file.TexCoordSet7 = int.Parse(e.Value);
                        break;
                    case "texture":
                        file.Texture = e.Value;
                        break;
                    case "secondary_texture":
                        file.SecondaryTexture = e.Value;
                        break;
                    case "bumpmap":
                        file.BumpMap = e.Value;
                        break;
                    case "specmap":
                        file.SpecMap = e.Value;
                        break;
                    case "glossmap":
                        file.GlossMap = e.Value;
                        break;
                    case "emissivemap":
                        file.EmissiveMap = e.Value;
                        break;
                }
            }

            return file;
        }
    }
}
