namespace AoMEngineLibrary.Graphics.Brg
{
    using MiscUtil.Conversion;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;

    public class BrgMaterial
    {
        public BrgFile ParentFile;
        public string EditorName
        {
            get
            {
                return "Mat ID: " + id;
            }
        }
        public Color DiffuseColor
        {
            get
            {
                return Color.FromArgb((int)Math.Round(diffuse.X * 255f), (int)Math.Round(diffuse.Y * 255f), (int)Math.Round(diffuse.Z * 255f));
            }
        }
        public Color AmbientColor
        {
            get
            {
                return Color.FromArgb((int)Math.Round(ambient.X * 255f), (int)Math.Round(ambient.Y * 255f), (int)Math.Round(ambient.Z * 255f));
            }
        }
        public Color SpecularColor
        {
            get
            {
                return Color.FromArgb((int)Math.Round(specular.X * 255f), (int)Math.Round(specular.Y * 255f), (int)Math.Round(specular.Z * 255f));
            }
        }
        public Color SelfIllumColor
        {
            get
            {
                return Color.FromArgb((int)Math.Round(selfIllum.X * 255f), (int)Math.Round(selfIllum.Y * 255f), (int)Math.Round(selfIllum.Z * 255f));
            }
        }

        public int id;
        public BrgMatFlag Flags { get; set; }
        public int unknown01b;
        //int nameLength;
        Vector3<float> diffuse; //unknown02 [36 bytes]
        Vector3<float> ambient;
        Vector3<float> specular;
        Vector3<float> selfIllum; //unknown03 [12 bytes]
        public string DiffuseMap { get; set; }
        public string BumpMap { get; set; }
        public float SpecularExponent { get; set; }
        public float Alpha { get; set; }
        public List<BrgMatSFX> sfx;

        public BrgMaterial(BrgBinaryReader reader, BrgFile file)
        {
            ParentFile = file;

            id = reader.ReadInt32();
            Flags = (BrgMatFlag)reader.ReadInt32();
            unknown01b = reader.ReadInt32();
            int nameLength = reader.ReadInt32();
            this.diffuse = reader.ReadVector3Single();
            this.ambient = reader.ReadVector3Single();
            this.specular = reader.ReadVector3Single();
            this.selfIllum = reader.ReadVector3Single();

            DiffuseMap = reader.ReadString(nameLength);
            if (Flags.HasFlag(BrgMatFlag.SpecularExponent))
            {
                SpecularExponent = reader.ReadSingle();
            }
            if (Flags.HasFlag(BrgMatFlag.BumpMap))
            {
                BumpMap = reader.ReadString(reader.ReadInt32());
            }
            if (Flags.HasFlag(BrgMatFlag.Alpha))
            {
                Alpha = reader.ReadSingle();
            }

            if (Flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
            {
                byte numSFX = reader.ReadByte();
                sfx = new List<BrgMatSFX>(numSFX);
                for (int i = 0; i < numSFX; i++)
                {
                    sfx.Add(reader.ReadMaterialSFX());
                }
            }
            else
            {
                sfx = new List<BrgMatSFX>();
            }
        }
        public BrgMaterial(BrgFile file)
        {
            ParentFile = file;
            id = 0;
            Flags = 0;
            unknown01b = 0;
            diffuse = new Vector3<float>();
            ambient = new Vector3<float>();
            specular = new Vector3<float>();
            selfIllum = new Vector3<float>();

            DiffuseMap = string.Empty;
            BumpMap = string.Empty;

            SpecularExponent = 1;

            Alpha = 1;

            sfx = new List<BrgMatSFX>();
        }

        public string ExportToMax()
        {
            Maxscript.Command("mat = StandardMaterial()");
            Maxscript.Command("mat.name = \"{0}\"", DiffuseMap);
            Maxscript.Command("mat.adLock = false");
            Maxscript.Command("mat.useSelfIllumColor = true");
            Maxscript.Command("mat.diffuse = color {0} {1} {2}", diffuse.X * 255f, diffuse.Y * 255f, diffuse.Z * 255f);
            Maxscript.Command("mat.ambient = color {0} {1} {2}", ambient.X * 255f, ambient.Y * 255f, ambient.Z * 255f);
            Maxscript.Command("mat.specular = color {0} {1} {2}", specular.X * 255f, specular.Y * 255f, specular.Z * 255f);
            Maxscript.Command("mat.selfIllumColor = color {0} {1} {2}", selfIllum.X * 255f, selfIllum.Y * 255f, selfIllum.Z * 255f);
            Maxscript.Command("mat.opacity = {0}", Alpha * 100f);
            Maxscript.Command("mat.specularLevel = {0}", SpecularExponent);
            //MaxHelper.Command("print \"{0}\"", name);
            if (Flags.HasFlag(BrgMatFlag.SubtractiveBlend))
            {
                Maxscript.Command("mat.opacityType = 1");
            }
            else if (Flags.HasFlag(BrgMatFlag.AdditiveBlend))
            {
                Maxscript.Command("mat.opacityType = 2");
            }

            Maxscript.Command("tex = BitmapTexture()");
            Maxscript.Command("tex.name = \"{0}\"", DiffuseMap);
            if (Flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
            {
                Maxscript.Command("rTex = BitmapTexture()");
                Maxscript.Command("rTex.name = \"{0}\"", Path.GetFileNameWithoutExtension(sfx[0].Name));
                Maxscript.Command("rTex.filename = \"{0}\"", Path.GetFileNameWithoutExtension(sfx[0].Name) + ".tga");
                Maxscript.Command("mat.reflectionMap = rTex");
            }
            if (Flags.HasFlag(BrgMatFlag.BumpMap))
            {
                Maxscript.Command("aTex = BitmapTexture()");
                Maxscript.Command("aTex.name = \"{0}\"", BumpMap);
                Maxscript.Command("aTex.filename = \"{0}\"", BumpMap + ".tga");
                Maxscript.Command("mat.bumpMap = aTex");
            }
            if (this.Flags.HasFlag(BrgMatFlag.WrapUTx1) && this.Flags.HasFlag(BrgMatFlag.WrapVTx1))
            {
                if (Flags.HasFlag(BrgMatFlag.PixelXForm1))
                {
                    Maxscript.Command("pcCompTex = CompositeTextureMap()");

                    Maxscript.Command("pcTex = BitmapTexture()");
                    Maxscript.Command("pcTex.name = \"{0}\"", DiffuseMap);
                    Maxscript.Command("pcTex.filename = \"{0}\"", DiffuseMap + ".tga");

                    Maxscript.Command("pcTex2 = BitmapTexture()");
                    Maxscript.Command("pcTex2.name = \"{0}\"", DiffuseMap);
                    Maxscript.Command("pcTex2.filename = \"{0}\"", DiffuseMap + ".tga");
                    Maxscript.Command("pcTex2.monoOutput = 1");

                    Maxscript.Command("pcCheck = Checker()");
                    Maxscript.Command("pcCheck.Color1 = color 0 0 255");
                    Maxscript.Command("pcCheck.Color2 = color 0 0 255");

                    Maxscript.Command("pcCompTex.mapList[1] = pcTex");
                    Maxscript.Command("pcCompTex.mapList[2] = pcCheck");
                    Maxscript.Command("pcCompTex.mask[2] = pcTex2");

                    Maxscript.Command("mat.diffusemap = pcCompTex");
                }
                else
                {
                    //MaxHelper.Command("print {0}", name);
                    Maxscript.Command("tex.filename = \"{0}\"", DiffuseMap + ".tga");
                    Maxscript.Command("mat.diffusemap = tex");
                }
            }

            return "mat";
        }
        public void ImportFromMax(string mainObject, int materialIndex)
        {
            id = (Int32)Maxscript.Query("{0}.material.materialIDList[{1}]", Maxscript.QueryType.Integer, mainObject, materialIndex + 1);
            Maxscript.Command("mat = {0}.material[{1}]", mainObject, id);

            diffuse.X = (float)Maxscript.Query("mat.diffuse.r", Maxscript.QueryType.Float) / 255f;
            diffuse.Y = (float)Maxscript.Query("mat.diffuse.g", Maxscript.QueryType.Float) / 255f;
            diffuse.Z = (float)Maxscript.Query("mat.diffuse.b", Maxscript.QueryType.Float) / 255f;
            ambient.X = (float)Maxscript.Query("mat.ambient.r", Maxscript.QueryType.Float) / 255f;
            ambient.Y = (float)Maxscript.Query("mat.ambient.g", Maxscript.QueryType.Float) / 255f;
            ambient.Z = (float)Maxscript.Query("mat.ambient.b", Maxscript.QueryType.Float) / 255f;
            specular.X = (float)Maxscript.Query("mat.specular.r", Maxscript.QueryType.Float) / 255f;
            specular.Y = (float)Maxscript.Query("mat.specular.g", Maxscript.QueryType.Float) / 255f;
            specular.Z = (float)Maxscript.Query("mat.specular.b", Maxscript.QueryType.Float) / 255f;
            selfIllum.X = (float)Maxscript.Query("mat.selfIllumColor.r", Maxscript.QueryType.Float) / 255f;
            selfIllum.Y = (float)Maxscript.Query("mat.selfIllumColor.g", Maxscript.QueryType.Float) / 255f;
            selfIllum.Z = (float)Maxscript.Query("mat.selfIllumColor.b", Maxscript.QueryType.Float) / 255f;
            Alpha = (float)Maxscript.Query("mat.opacity", Maxscript.QueryType.Float) / 100f;
            SpecularExponent = (float)Maxscript.Query("mat.specularLevel", Maxscript.QueryType.Float);
            int opacityType = Maxscript.QueryInteger("mat.opacityType");
            if (SpecularExponent > 0)
            {
                Flags |= BrgMatFlag.SpecularExponent;
            }
            if (Alpha < 1f)
            {
                Flags |= BrgMatFlag.Alpha;
            }
            if (opacityType == 1)
            {
                Flags |= BrgMatFlag.SubtractiveBlend;
            }
            else if (opacityType == 2)
            {
                Flags |= BrgMatFlag.AdditiveBlend;
            }

            if (Maxscript.QueryBoolean("(classof mat.reflectionMap) == BitmapTexture"))
            {
                Flags |= BrgMatFlag.WrapUTx1 | BrgMatFlag.WrapVTx1 | BrgMatFlag.REFLECTIONTEXTURE;
                BrgMatSFX sfxMap = new BrgMatSFX();
                sfxMap.Id = 30;
                sfxMap.Name = (string)Maxscript.Query("getFilenameFile(mat.reflectionMap.filename)", Maxscript.QueryType.String) + ".cub";
                sfx.Add(sfxMap);
            }
            if (Maxscript.QueryBoolean("(classof mat.bumpMap) == BitmapTexture"))
            {
                BumpMap = (string)Maxscript.Query("getFilenameFile(mat.bumpMap.filename)", Maxscript.QueryType.String);
                if (BumpMap.Length > 0)
                {
                    Flags |= BrgMatFlag.WrapUTx3 | BrgMatFlag.WrapVTx3 | BrgMatFlag.BumpMap;
                }
            }
            if (Maxscript.QueryBoolean("(classof mat.diffusemap) == BitmapTexture"))
            {
                DiffuseMap = (string)Maxscript.Query("getFilenameFile(mat.diffusemap.filename)", Maxscript.QueryType.String);
                if (DiffuseMap.Length > 0)
                {
                    Flags |= BrgMatFlag.WrapUTx1 | BrgMatFlag.WrapVTx1;
                }
            }
            else if (Maxscript.QueryBoolean("(classof mat.diffusemap) == CompositeTextureMap") && Maxscript.QueryBoolean("(classof mat.diffusemap.mapList[1]) == BitmapTexture"))
            {
                Flags |= BrgMatFlag.WrapUTx1 | BrgMatFlag.WrapVTx1 | BrgMatFlag.PixelXForm1;
                DiffuseMap = (string)Maxscript.Query("getFilenameFile(mat.diffusemap.mapList[1].filename)", Maxscript.QueryType.String);
            }
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(id);
            writer.Write((int)Flags);

            writer.Write(unknown01b);
            writer.Write(Encoding.UTF8.GetByteCount(DiffuseMap));

            writer.WriteVector3(diffuse);
            writer.WriteVector3(ambient);
            writer.WriteVector3(specular);
            writer.WriteVector3(selfIllum);

            writer.WriteString(DiffuseMap, 0);

            if (Flags.HasFlag(BrgMatFlag.SpecularExponent))
            {
                writer.Write(SpecularExponent);
            }
            if (Flags.HasFlag(BrgMatFlag.BumpMap))
            {
                writer.WriteString(BumpMap, 4);
            }
            if (Flags.HasFlag(BrgMatFlag.Alpha))
            {
                writer.Write(Alpha);
            }

            if (Flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
            {
                writer.Write((byte)sfx.Count);
                for (int i = 0; i < sfx.Count; i++)
                {
                    writer.Write(sfx[i].Id);
                    writer.WriteString(sfx[i].Name, 2);
                }
            }
        }

        public void WriteExternal(FileStream fileStream)
        {
            using (BrgBinaryWriter writer = new BrgBinaryWriter(new LittleEndianBitConverter(), fileStream))
            {
                writer.Write(1280463949); // MTRL
                writer.Write(Encoding.UTF8.GetByteCount(DiffuseMap));

                writer.Write(new byte[20]);

                writer.WriteVector3(diffuse);
                writer.WriteVector3(ambient);
                writer.WriteVector3(specular);
                writer.WriteVector3(selfIllum);
                writer.Write(SpecularExponent);
                writer.Write(Alpha);

                writer.Write(-1);
                writer.Write(16777216);
                writer.Write(65793);
                writer.Write(10);
                writer.Write(new byte[16]);

                if (Flags.HasFlag(BrgMatFlag.PixelXForm1))
                {
                    writer.Write(4);
                }
                else
                {
                    writer.Write(0);
                }

                writer.Write(0);

                if (Flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
                {
                    writer.Write(1275068416);
                    writer.Write(12);
                    writer.Write(0);
                    writer.Write(1);
                }
                else
                {
                    writer.Write(new byte[16]);
                }

                writer.Write(new byte[32]);
                writer.Write(-1);
                writer.Write(-1);
                writer.Write(-1);
                writer.Write(-1);
                writer.Write(-1);
                writer.Write(-1);
                writer.Write(new byte[16]);

                writer.WriteString(DiffuseMap);
            }
        }
    }
}
