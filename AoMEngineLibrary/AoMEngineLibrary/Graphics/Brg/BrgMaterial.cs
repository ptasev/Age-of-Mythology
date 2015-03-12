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
        public BrgMatFlag flags;
        public int unknown01b;
        //int nameLength;
        Vector3<float> diffuse; //unknown02 [36 bytes]
        Vector3<float> ambient;
        Vector3<float> specular;
        Vector3<float> selfIllum; //unknown03 [12 bytes]
        public string name;
        public string name2;
        public float specularLevel;
        public float alphaOpacity; //unknown04
        public List<BrgMatSFX> sfx;

        public BrgMaterial(BrgBinaryReader reader, BrgFile file)
        {
            ParentFile = file;

            id = reader.ReadInt32();
            flags = (BrgMatFlag)reader.ReadInt32();
            unknown01b = reader.ReadInt32();
            int nameLength = reader.ReadInt32();
            reader.ReadVector3(out diffuse);
            reader.ReadVector3(out ambient);
            reader.ReadVector3(out specular);
            reader.ReadVector3(out selfIllum);

            name = reader.ReadString(nameLength);
            if (flags.HasFlag(BrgMatFlag.USESPECLVL))
            {
                specularLevel = reader.ReadSingle();
            }
            if (flags.HasFlag(BrgMatFlag.MATTEXURE2))
            {
                name2 = reader.ReadString(reader.ReadInt32());
            }
            if (true)
            {
                alphaOpacity = reader.ReadSingle();
            }

            if (flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
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
            flags = 0;
            unknown01b = 0;
            //nameLength = 0;
            diffuse = new Vector3<float>();
            ambient = new Vector3<float>();
            specular = new Vector3<float>();
            selfIllum = new Vector3<float>();

            name = string.Empty;
            name2 = string.Empty;

            specularLevel = 1;

            alphaOpacity = 1;

            sfx = new List<BrgMatSFX>();
        }
        public BrgMaterial(BrgMaterial copy)
        {
            ParentFile = copy.ParentFile;

            id = copy.id;
            flags = copy.flags;
            unknown01b = copy.unknown01b;
            //nameLength = copy.nameLength;
            diffuse = copy.diffuse;
            ambient = copy.ambient;
            specular = copy.specular;
            selfIllum = copy.selfIllum;

            name = copy.name;
            if (flags.HasFlag(BrgMatFlag.USESPECLVL))
            {
                specularLevel = copy.specularLevel;
            }
            if (flags.HasFlag(BrgMatFlag.MATTEXURE2))
            {
                name2 = copy.name2;
            }
            alphaOpacity = copy.alphaOpacity;

            if (flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
            {
                sfx = new List<BrgMatSFX>(copy.sfx.Count);
                for (int i = 0; i < copy.sfx.Count; i++)
                {
                    sfx.Add(copy.sfx[i]);
                }
            }
            else
            {
                sfx = new List<BrgMatSFX>();
            }
        }

        public string ExportToMax()
        {
            Maxscript.Command("mat = StandardMaterial()");
            Maxscript.Command("mat.name = \"{0}\"", name);
            Maxscript.Command("mat.adLock = false");
            Maxscript.Command("mat.useSelfIllumColor = true");
            Maxscript.Command("mat.diffuse = color {0} {1} {2}", diffuse.X * 255f, diffuse.Y * 255f, diffuse.Z * 255f);
            Maxscript.Command("mat.ambient = color {0} {1} {2}", ambient.X * 255f, ambient.Y * 255f, ambient.Z * 255f);
            Maxscript.Command("mat.specular = color {0} {1} {2}", specular.X * 255f, specular.Y * 255f, specular.Z * 255f);
            Maxscript.Command("mat.selfIllumColor = color {0} {1} {2}", selfIllum.X * 255f, selfIllum.Y * 255f, selfIllum.Z * 255f);
            Maxscript.Command("mat.opacity = {0}", alphaOpacity * 100f);
            Maxscript.Command("mat.specularLevel = {0}", specularLevel);
            //MaxHelper.Command("print \"{0}\"", name);

            Maxscript.Command("tex = BitmapTexture()");
            Maxscript.Command("tex.name = \"{0}\"", name);
            if (flags.HasFlag(BrgMatFlag.MATNONE25))
            {
                if (flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
                {
                    Maxscript.Command("rTex = BitmapTexture()");
                    Maxscript.Command("rTex.name = \"{0}\"", Path.GetFileNameWithoutExtension(sfx[0].Name));
                    Maxscript.Command("rTex.filename = \"{0}\"", Path.GetFileNameWithoutExtension(sfx[0].Name) + ".tga");
                    Maxscript.Command("mat.reflectionMap = rTex");
                }
                if (flags.HasFlag(BrgMatFlag.MATTEXURE2))
                {
                    Maxscript.Command("aTex = BitmapTexture()");
                    Maxscript.Command("aTex.name = \"{0}\"", name2);
                    Maxscript.Command("aTex.filename = \"{0}\"", name2 + ".tga");
                    Maxscript.Command("mat.ambientMap = aTex");
                }
                if (flags.HasFlag(BrgMatFlag.WHITESELFILLUMCOLOR))
                {
                    Maxscript.Command("tex.filename = \"{0}\"", name + ".tga");
                    Maxscript.Command("mat.selfIllumMap = tex");
                }
                if (flags.HasFlag(BrgMatFlag.PLAYERCOLOR))
                {
                    Maxscript.Command("pcCompTex = CompositeTextureMap()");

                    Maxscript.Command("pcTex = BitmapTexture()");
                    Maxscript.Command("pcTex.name = \"{0}\"", name);
                    Maxscript.Command("pcTex.filename = \"{0}\"", name + ".tga");

                    Maxscript.Command("pcTex2 = BitmapTexture()");
                    Maxscript.Command("pcTex2.name = \"{0}\"", name);
                    Maxscript.Command("pcTex2.filename = \"{0}\"", name + ".tga");
                    Maxscript.Command("pcTex2.monoOutput = 1");

                    Maxscript.Command("pcCheck = Checker()");
                    Maxscript.Command("pcCheck.Color1 = color 0 0 255");
                    Maxscript.Command("pcCheck.Color2 = color 0 0 255");

                    Maxscript.Command("pcCompTex.mapList[1] = pcTex");
                    Maxscript.Command("pcCompTex.mapList[2] = pcCheck");
                    Maxscript.Command("pcCompTex.mask[2] = pcTex2");

                    Maxscript.Command("mat.diffusemap = pcCompTex");
                }
            }
            if (flags.HasFlag(BrgMatFlag.DIFFUSETEXTURE) && !flags.HasFlag(BrgMatFlag.PLAYERCOLOR))
            {
                //MaxHelper.Command("print {0}", name);
                Maxscript.Command("tex.filename = \"{0}\"", name + ".tga");
                Maxscript.Command("mat.diffusemap = tex");
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
            alphaOpacity = (float)Maxscript.Query("mat.opacity", Maxscript.QueryType.Float) / 100f;
            specularLevel = (float)Maxscript.Query("mat.specularLevel", Maxscript.QueryType.Float);
            if (specularLevel > 0)
            {
                flags |= BrgMatFlag.USESPECLVL;
            }

            //flags |= BrgMatFlag.MATNONE1;
            if (Maxscript.QueryBoolean("(classof mat.reflectionMap) == BitmapTexture"))
            {
                flags |= BrgMatFlag.MATNONE25 | BrgMatFlag.REFLECTIONTEXTURE;
                BrgMatSFX sfxMap = new BrgMatSFX();
                sfxMap.Id = 30;
                sfxMap.Name = (string)Maxscript.Query("getFilenameFile(mat.reflectionMap.filename)", Maxscript.QueryType.String) + ".cub";
                sfx.Add(sfxMap);
            }
            if (Maxscript.QueryBoolean("(classof mat.ambientMap) == BitmapTexture"))
            {
                flags |= BrgMatFlag.MATNONE25 | BrgMatFlag.MATTEXURE2;
                name2 = (string)Maxscript.Query("getFilenameFile(mat.ambientMap.filename)", Maxscript.QueryType.String);
            }
            if (Maxscript.QueryBoolean("(classof mat.selfIllumMap) == BitmapTexture"))
            {
                flags |= BrgMatFlag.MATNONE25 | BrgMatFlag.DIFFUSETEXTURE | BrgMatFlag.WHITESELFILLUMCOLOR;
                name = (string)Maxscript.Query("getFilenameFile(mat.selfIllumMap.filename)", Maxscript.QueryType.String);
            }
            if (Maxscript.QueryBoolean("(classof mat.bumpMap) == BitmapTexture"))
            {
                flags |= BrgMatFlag.MATNONE25 | BrgMatFlag.DIFFUSETEXTURE | BrgMatFlag.PLAYERCOLOR;
                name = (string)Maxscript.Query("getFilenameFile(mat.bumpMap.filename)", Maxscript.QueryType.String);
            }
            if (Maxscript.QueryBoolean("(classof mat.diffusemap) == BitmapTexture"))
            {
                flags |= BrgMatFlag.DIFFUSETEXTURE;
                name = (string)Maxscript.Query("getFilenameFile(mat.diffusemap.filename)", Maxscript.QueryType.String);
                if (name.Length > 0)
                {
                    flags |= BrgMatFlag.MATNONE25;
                }
            }
            else if (Maxscript.QueryBoolean("(classof mat.diffusemap) == CompositeTextureMap") && Maxscript.QueryBoolean("(classof mat.diffusemap.mapList[1]) == BitmapTexture"))
            {
                flags |= BrgMatFlag.MATNONE25 | BrgMatFlag.DIFFUSETEXTURE | BrgMatFlag.PLAYERCOLOR;
                name = (string)Maxscript.Query("getFilenameFile(mat.diffusemap.mapList[1].filename)", Maxscript.QueryType.String);
            }
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(id);
            writer.Write((int)flags);

            writer.Write(unknown01b);
            writer.Write(Encoding.UTF8.GetByteCount(name));

            writer.WriteVector3(ref diffuse);
            writer.WriteVector3(ref ambient);
            writer.WriteVector3(ref specular);
            writer.WriteVector3(ref selfIllum);

            writer.WriteString(name, 0);

            if (flags.HasFlag(BrgMatFlag.USESPECLVL))
            {
                writer.Write(specularLevel);
            }
            if (flags.HasFlag(BrgMatFlag.MATTEXURE2))
            {
                writer.WriteString(name2, 4);
            }

            writer.Write(alphaOpacity);

            if (flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
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
                writer.Write(Encoding.UTF8.GetByteCount(name));

                writer.Write(new byte[20]);

                writer.WriteVector3(ref diffuse);
                writer.WriteVector3(ref ambient);
                writer.WriteVector3(ref specular);
                writer.WriteVector3(ref selfIllum);
                writer.Write(specularLevel);
                writer.Write(alphaOpacity);

                writer.Write(-1);
                writer.Write(16777216);
                writer.Write(65793);
                writer.Write(10);
                writer.Write(new byte[16]);

                if (flags.HasFlag(BrgMatFlag.PLAYERCOLOR))
                {
                    writer.Write(4);
                }
                else
                {
                    writer.Write(0);
                }

                writer.Write(0);

                if (flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
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

                writer.WriteString(name);
            }
        }

        public void ReadBr3(BrgBinaryReader reader)
        {
            id = reader.ReadInt32();
            flags = (BrgMatFlag)reader.ReadInt32();
            name = reader.ReadString((byte)0x0);
            //nameLength = Encoding.UTF8.GetByteCount(name);
        }
        public void WriteBr3(BrgBinaryWriter writer)
        {
            writer.Write(id);
            writer.Write((int)flags);
            writer.WriteString(name);
        }
    }
}
