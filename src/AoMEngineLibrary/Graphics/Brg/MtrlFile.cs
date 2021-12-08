using AoMEngineLibrary.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace AoMEngineLibrary.Graphics.Brg
{
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
        private const uint MaxPath = 260; // Used in game code for Win max path

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
                (_texture, TextureNameLength) = UpdateTextureFile(value);
            }
        }

        private string _secondaryTexture;
        public string SecondaryTexture
        {
            get => _secondaryTexture;
            set
            {
                (_secondaryTexture, SecondaryTextureNameLength) = UpdateTextureFile(value);
            }
        }

        private string _bumpMap;
        public string BumpMap
        {
            get => _bumpMap;
            set
            {
                (_bumpMap, BumpMapNameLength) = UpdateTextureFile(value);
            }
        }

        private string _specMap;
        public string SpecMap
        {
            get => _specMap;
            set
            {
                (_specMap, SpecMapNameLength) = UpdateTextureFile(value);
            }
        }

        private string _glossMap;
        public string GlossMap
        {
            get => _glossMap;
            set
            {
                (_glossMap, GlossMapNameLength) = UpdateTextureFile(value);
            }
        }

        private string _emissiveMap;
        public string EmissiveMap
        {
            get => _emissiveMap;
            set
            {
                (_emissiveMap, EmissiveMapNameLength) = UpdateTextureFile(value);
            }
        }

        public MtrlFile()
        {
            Alpha = 1f;

            Id = -1;

            SelfIlluminating = 0;
            ClampU = 0;
            ClampV = 0;
            LightSpecular = 1;
            AffectsAmbient = 1;
            AffectsDiffuse = 1;
            AffectsSpecular = 1;

            AlphaMode = MtrlAlphaMode.Texture;

            TextureIndex = -1;
            SecondaryTextureIndex = -1;
            BumpMapIndex = -1;

            SpecularMapIndex = -1;
            GlossMapIndex = -1;
            EmissiveMapIndex = -1;

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
            Diffuse = mat.DiffuseColor;
            Ambient = mat.AmbientColor;
            Specular = mat.SpecularColor;
            Emissive = mat.EmissiveColor;
            SpecularPower = mat.SpecularExponent;
            Alpha =  mat.Opacity > 1 ? 1 : (mat.Opacity < 0 ? 0 : mat.Opacity);

            // EE doesn't seem to care about setting these.
            //this.DiffuseIntensity = 0.299f * Diffuse.X + 0.587f * Diffuse.Y + 0.114f * Diffuse.Z;
            //this.AmbientIntensity = 0.299f * Ambient.X + 0.587f * Ambient.Y + 0.114f * Ambient.Z;
            //this.SpecularIntensity = 0.299f * Specular.X + 0.587f * Specular.Y + 0.114f * Specular.Z;
            //this.EmissiveIntensity = 0.299f * Emissive.X + 0.587f * Emissive.Y + 0.114f * Emissive.Z;

            if ((Math.Abs(Diffuse.X - 1) >= Epsilon) || (Math.Abs(Diffuse.Y - 1) >= Epsilon) || (Math.Abs(Diffuse.Z - 1) >= Epsilon))
                AffectsDiffuse = 1;
            if ((Math.Abs(Ambient.X - 1) >= Epsilon) || (Math.Abs(Ambient.Y - 1) >= Epsilon) || (Math.Abs(Ambient.Z - 1) >= Epsilon))
                AffectsAmbient = 1;
            if ((Math.Abs(Specular.X - 1) >= Epsilon) || (Math.Abs(Specular.Y - 1) >= Epsilon) || (Math.Abs(Specular.Z - 1) >= Epsilon))
                AffectsSpecular = 1;
            if ((Emissive.X > Epsilon) || (Emissive.Y > Epsilon) || (Emissive.Z > Epsilon))
                SelfIlluminating = 1;

            if (mat.SpecularColor.X >= Epsilon || mat.SpecularColor.Y >= Epsilon || mat.SpecularColor.Z >= Epsilon)
                LightSpecular = 1;

            if (!mat.Flags.HasFlag(BrgMatFlag.WrapUTx1))
            {
                ClampU = 1;
            }

            if (!mat.Flags.HasFlag(BrgMatFlag.WrapVTx1))
            {
                ClampV = 1;
            }

            if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormColor1))
            {
                ColorTransform = MtrlColorTransformMode.Transform1;
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormColor2))
            {
                ColorTransform = MtrlColorTransformMode.Transform2;
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormColor3))
            {
                ColorTransform = MtrlColorTransformMode.Transform3;
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PixelXForm1))
            {
                ColorTransform = MtrlColorTransformMode.TransformPixel1;
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PixelXForm2))
            {
                ColorTransform = MtrlColorTransformMode.TransformPixel2;
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PixelXForm3))
            {
                ColorTransform = MtrlColorTransformMode.TransformPixel3;
            }

            if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormTx1))
            {
                TextureTransform = MtrlTextureTransformMode.Transform1;
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.PlayerXFormTx2))
            {
                TextureTransform = MtrlTextureTransformMode.Transform2;
            }

            Texture = mat.DiffuseMapName;
            BumpMap = mat.BumpMapName;

            if (mat.Flags.HasFlag(BrgMatFlag.CubeMapInfo))
            {
                SecondaryTexture = mat.CubeMapInfo.CubeMapName;

                switch (mat.CubeMapInfo.Mode)
                {
                    case 0:
                        TexGenMode1 = MtrlTexGenMode.CubicEnvironment;

                        if (mat.Flags.HasFlag(BrgMatFlag.AdditiveCubeBlend))
                        {
                            if (mat.Flags.HasFlag(BrgMatFlag.InverseAlpha))
                                MultiTextureMode = MtrlMultitextureMode.AddInverseAlpha;
                            else
                                MultiTextureMode = MtrlMultitextureMode.AddAlpha;
                        }
                        else
                        {
                            if (mat.Flags.HasFlag(BrgMatFlag.InverseAlpha))
                                MultiTextureMode = MtrlMultitextureMode.LinearBlendInverseAlpha;
                            else
                                MultiTextureMode = MtrlMultitextureMode.LinearBlendAlpha;
                        }
                        break;
                    case 1:
                        TexGenMode1 = MtrlTexGenMode.CubicEnvironment;

                        if (mat.Flags.HasFlag(BrgMatFlag.AdditiveCubeBlend))
                            MultiTextureMode = MtrlMultitextureMode.AddFactor;
                        else
                            MultiTextureMode = MtrlMultitextureMode.LinearBlend;

                        uint txFactor = (byte)(255.0f * (mat.CubeMapInfo.TextureFactor / 100.0f));
                        TextureFactor = txFactor << 24;
                        break;
                    default:
                        throw new NotImplementedException($"Unsupported cube map mode {mat.CubeMapInfo.Mode}.");
                }
            }

            if (mat.Flags.HasFlag(BrgMatFlag.Alpha))
            {
                if ((1 - Alpha) > Epsilon)
                    AlphaMode = MtrlAlphaMode.On;
                else
                    AlphaMode = MtrlAlphaMode.Texture;
            }

            if ((ColorTransform == MtrlColorTransformMode.TransformPixel1 ||
                ColorTransform == MtrlColorTransformMode.TransformPixel2 ||
                ColorTransform == MtrlColorTransformMode.TransformPixel3) &&
                (AlphaMode == MtrlAlphaMode.On))
                AlphaMode = MtrlAlphaMode.Off;

            if (mat.Flags.HasFlag(BrgMatFlag.AdditiveBlend))
            {
                AlphaMode = MtrlAlphaMode.Additive;
            }
            else if (mat.Flags.HasFlag(BrgMatFlag.SubtractiveBlend))
            {
                AlphaMode = MtrlAlphaMode.Subtractive;
            }
        }

        public void Read(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var magic = reader.ReadUInt32();
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

                Diffuse = reader.ReadVector3();
                Ambient = reader.ReadVector3();
                Specular = reader.ReadVector3();
                Emissive = reader.ReadVector3();
                SpecularPower = reader.ReadSingle();
                Alpha = reader.ReadSingle();

                Id = reader.ReadInt32();

                SelfIlluminating = reader.ReadByte();
                ClampU = reader.ReadByte();
                ClampV = reader.ReadByte();
                LightSpecular = reader.ReadByte();
                AffectsAmbient = reader.ReadByte();
                AffectsDiffuse = reader.ReadByte();
                AffectsSpecular = reader.ReadByte();
                Updateable = reader.ReadByte();

                AlphaMode = (MtrlAlphaMode)reader.ReadInt32(); // wave has a 2 here, phoenix has 6
                AmbientIntensity = reader.ReadSingle();
                DiffuseIntensity = reader.ReadSingle();
                SpecularIntensity = reader.ReadSingle();
                EmissiveIntensity = reader.ReadSingle();
                ColorTransform = (MtrlColorTransformMode)reader.ReadInt32();
                TextureTransform = (MtrlTextureTransformMode)reader.ReadInt32();
                TextureFactor = reader.ReadUInt32();
                MultiTextureMode = (MtrlMultitextureMode)reader.ReadInt32();
                TexGenMode0 = (MtrlTexGenMode)reader.ReadInt32();
                TexGenMode1 = (MtrlTexGenMode)reader.ReadInt32();
                TexCoordSet0 = reader.ReadInt32();
                TexCoordSet1 = reader.ReadInt32();
                TexCoordSet2 = reader.ReadInt32();
                TexCoordSet3 = reader.ReadInt32();
                TexCoordSet4 = reader.ReadInt32();
                TexCoordSet5 = reader.ReadInt32();
                TexCoordSet6 = reader.ReadInt32();
                TexCoordSet7 = reader.ReadInt32();

                TextureIndex = reader.ReadInt32();
                SecondaryTextureIndex = reader.ReadInt32();
                BumpMapIndex = reader.ReadInt32();

                SpecularMapIndex = reader.ReadInt32();
                GlossMapIndex = reader.ReadInt32();
                EmissiveMapIndex = reader.ReadInt32();

                for (var i = 0; i < 4; ++i)
                {
                    Reserved[i] = reader.ReadInt32();
                }

                if (TextureNameLength > 0)
                {
                    Texture = reader.ReadNullTerminatedString();
                }

                if (SecondaryTextureNameLength > 0)
                {
                    SecondaryTexture = reader.ReadNullTerminatedString();
                }

                if (BumpMapNameLength > 0)
                {
                    BumpMap = reader.ReadNullTerminatedString();
                }

                if (SpecMapNameLength > 0)
                {
                    SpecMap = reader.ReadNullTerminatedString();
                }

                if (GlossMapNameLength > 0)
                {
                    GlossMap = reader.ReadNullTerminatedString();
                }

                if (EmissiveMapNameLength > 0)
                {
                    EmissiveMap = reader.ReadNullTerminatedString();
                }
            }
        }

        public void Write(Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Write(1280463949); // MTRL

                writer.Write(TextureNameLength);
                writer.Write(SecondaryTextureNameLength);
                writer.Write(BumpMapNameLength);
                writer.Write(SpecMapNameLength);
                writer.Write(GlossMapNameLength);
                writer.Write(EmissiveMapNameLength);

                writer.WriteVector3(Diffuse);
                writer.WriteVector3(Ambient);
                writer.WriteVector3(Specular);
                writer.WriteVector3(Emissive);
                writer.Write(SpecularPower);
                writer.Write(Alpha);

                writer.Write(Id);

                writer.Write(SelfIlluminating);
                writer.Write(ClampU);
                writer.Write(ClampV);
                writer.Write(LightSpecular);
                writer.Write(AffectsAmbient);
                writer.Write(AffectsDiffuse);
                writer.Write(AffectsSpecular);
                writer.Write(Updateable);

                writer.Write((int)AlphaMode); // Seems to be very often 10, wave has a 2 here, phoenix has 6
                writer.Write(AmbientIntensity);
                writer.Write(DiffuseIntensity);
                writer.Write(SpecularIntensity);
                writer.Write(EmissiveIntensity);
                writer.Write((int)ColorTransform); // Val of 4 seems to be PC
                writer.Write((int)TextureTransform);
                writer.Write(TextureFactor); // Has something to do with Cube Map
                writer.Write((int)MultiTextureMode); // Has something to do with Cube Map
                writer.Write((int)TexGenMode0);
                writer.Write((int)TexGenMode1); // Has something to do with Cube Map
                writer.Write(TexCoordSet0);
                writer.Write(TexCoordSet1);
                writer.Write(TexCoordSet2);
                writer.Write(TexCoordSet3);
                writer.Write(TexCoordSet4);
                writer.Write(TexCoordSet5);
                writer.Write(TexCoordSet6);
                writer.Write(TexCoordSet7);

                writer.Write(TextureIndex);
                writer.Write(SecondaryTextureIndex);
                writer.Write(BumpMapIndex);

                writer.Write(SpecularMapIndex);
                writer.Write(GlossMapIndex);
                writer.Write(EmissiveMapIndex);

                for (var i = 0; i < 4; ++i)
                {
                    writer.Write(Reserved[i]);
                }

                if (TextureNameLength > 0)
                {
                    writer.WriteNullTerminatedString(Texture);
                }

                if (SecondaryTextureNameLength > 0)
                {
                    writer.WriteNullTerminatedString(SecondaryTexture);
                }

                if (BumpMapNameLength > 0)
                {
                    writer.WriteNullTerminatedString(BumpMap);
                }

                if (SpecMapNameLength > 0)
                {
                    writer.WriteNullTerminatedString(SpecMap);
                }

                if (GlossMapNameLength > 0)
                {
                    writer.WriteNullTerminatedString(GlossMap);
                }

                if (EmissiveMapNameLength > 0)
                {
                    writer.WriteNullTerminatedString(EmissiveMap);
                }
            }
        }

        public void SerializeAsXml(Stream stream)
        {
            var xdoc = new XDocument(new XElement("Material"));
            var elem = (XElement)xdoc.FirstNode!;
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
            elem.Add(new XElement("alpha_mode", (int)AlphaMode));

            elem.Add(new XElement("ambient_intensity", AmbientIntensity));
            elem.Add(new XElement("diffuse_intensity", DiffuseIntensity));
            elem.Add(new XElement("specular_intensity", SpecularIntensity));
            elem.Add(new XElement("emissive_intensity", EmissiveIntensity));

            elem.Add(new XElement("color_transform", (int)ColorTransform));
            elem.Add(new XElement("texture_transform", (int)TextureTransform));
            elem.Add(new XElement("texture_factor", TextureFactor));
            elem.Add(new XElement("multitexture_mode", (int)MultiTextureMode));

            elem.Add(new XElement("texgen_mode_0", (int)TexGenMode0));
            elem.Add(new XElement("texgen_mode_1", (int)TexGenMode1));
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

            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = IndentString,
                CloseOutput = false
            };
            using (var writer = XmlWriter.Create(stream, settings))
            {
                xdoc.Save(writer);
            }
        }

        public static MtrlFile DeserializeAsXml(Stream stream)
        {
            var file = new MtrlFile();
            // default xml reader settings do not close input
            var xdoc = XDocument.Load(stream);
            var elem = xdoc.Root;
            var elements = elem?.Elements() ?? Enumerable.Empty<XElement>();

            foreach (var e in elements)
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

        private (string Texture, uint Length) UpdateTextureFile(string texture)
        {
            ArgumentNullException.ThrowIfNull(texture);

            var strLen = Encoding.UTF8.GetByteCount(texture);
            if (strLen > MaxPath)
                throw new ArgumentOutOfRangeException($"Texture name exceeded max length of {MaxPath}");

            return (texture, (uint)strLen);
        }
    }
}
