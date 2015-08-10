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

    public class MtrlFile
    {
        uint magic;
        uint nameLength;
        uint[] unk; //5   

        Color3D diffuse;
        Color3D ambient;
        Color3D specular;
        Color3D emissive;
        float specularLevel;
        float alpha;

        int id;

        byte selfIlluminating;
        byte clampU;
        byte clampV;
        byte lightSpecular;
        byte affectsAmbient;
        byte affectsDiffuse;
        byte affectsSpecular;
        byte updateable;

        int alphaMode; // Seems to be very often 10, wave has a 2 here, phoenix has 6
        int ambientIntensity;
        int diffuseIntensity;
        int specularIntensity;
        int emissiveIntensity;
        int colorTransform; // Val of 4 seems to be PC
        int textureTransform;
        int textureFactor; // Has something to do with Cube Map
        int multiTextureMode; // Has something to do with Cube Map
        int texGenMode0;
        int texGenMode1; // Has something to do with Cube Map
        int texCoordSet0;
        int texCoordSet1;
        int texCoordSet2;
        int texCoordSet3;
        int texCoordSet4;
        int texCoordSet5;
        int texCoordSet6;
        int texCoordSet7;

        Color4D[] unk2;
        Color4D[] unk3;
        int[] unk4;
        string texture;

        public MtrlFile()
        {
            this.unk = new uint[5];
            this.alpha = 1f;

            this.id = -1;

            this.selfIlluminating = 0;
            this.clampU = 0;
            this.clampV = 0;
            this.lightSpecular = 1;
            this.affectsAmbient = 1;
            this.affectsDiffuse = 1;
            this.affectsSpecular = 1;

            this.alphaMode = 10;

            unk2 = new Color4D[3] { new Color4D(1f), new Color4D(1f), new Color4D(1f) };
            unk3 = new Color4D[3] { new Color4D(1f), new Color4D(1f), new Color4D(1f) };
            unk4 = new int[4];
        }
        public MtrlFile(BrgMaterial mat)
            : this()
        {
            this.nameLength = (uint)Encoding.UTF8.GetByteCount(mat.DiffuseMap);

            this.diffuse = mat.DiffuseColor;
            this.ambient = mat.AmbientColor;
            this.specular = mat.SpecularColor;
            this.emissive = mat.EmissiveColor;
            this.specularLevel = mat.SpecularExponent;
            this.alpha = mat.Opacity;

            if (!mat.Flags.HasFlag(BrgMatFlag.WrapUTx1))
            {
                this.clampU = 1;
            }

            if (!mat.Flags.HasFlag(BrgMatFlag.WrapVTx1))
            {
                this.clampV = 1;
            }

            if (mat.Flags.HasFlag(BrgMatFlag.AdditiveBlend))
            {
                this.alphaMode = 6;
            }

            if (mat.Flags.HasFlag(BrgMatFlag.PixelXForm1))
            {
                this.colorTransform = 4;
            }

            if (mat.Flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
            {
                this.textureFactor = 1275068416;
                this.multiTextureMode = 12;
                this.texGenMode1 = 1;
            }

            this.texture = mat.DiffuseMap;
        }

        public void Read(Stream stream)
        {
            using (BrgBinaryReader reader = new BrgBinaryReader(new LittleEndianBitConverter(), stream))
            {
                this.magic = reader.ReadUInt32();
                if (this.magic != 1280463949)
                {
                    throw new Exception("This is not a MTRL file!");
                }

                this.nameLength = reader.ReadUInt32();
                for (int i = 0; i < 5; ++i)
                {
                    this.unk[i] = reader.ReadUInt32();
                }

                this.diffuse = reader.ReadColor3D();
                this.ambient = reader.ReadColor3D();
                this.specular = reader.ReadColor3D();
                this.emissive = reader.ReadColor3D();
                this.specularLevel = reader.ReadSingle();
                this.alpha = reader.ReadSingle();

                this.id = reader.ReadInt32();

                this.selfIlluminating = reader.ReadByte();
                this.clampU = reader.ReadByte();
                this.clampV = reader.ReadByte();
                this.lightSpecular = reader.ReadByte();
                this.affectsAmbient = reader.ReadByte();
                this.affectsDiffuse = reader.ReadByte();
                this.affectsSpecular = reader.ReadByte();
                this.updateable = reader.ReadByte();

                this.alphaMode = reader.ReadInt32(); // Seems to be very often 10, wave has a 2 here, phoenix has 6
                this.ambientIntensity = reader.ReadInt32();
                this.diffuseIntensity = reader.ReadInt32();
                this.specularIntensity = reader.ReadInt32();
                this.emissiveIntensity = reader.ReadInt32();
                this.colorTransform = reader.ReadInt32(); // Val of 4 seems to be PC
                this.textureTransform = reader.ReadInt32();
                this.textureFactor = reader.ReadInt32(); // Has something to do with Cube Map
                this.multiTextureMode = reader.ReadInt32(); // Has something to do with Cube Map
                this.texGenMode0 = reader.ReadInt32();
                this.texGenMode1 = reader.ReadInt32(); // Has something to do with Cube Map
                this.texCoordSet0 = reader.ReadInt32();
                this.texCoordSet1 = reader.ReadInt32();
                this.texCoordSet2 = reader.ReadInt32();
                this.texCoordSet3 = reader.ReadInt32();
                this.texCoordSet4 = reader.ReadInt32();
                this.texCoordSet5 = reader.ReadInt32();
                this.texCoordSet6 = reader.ReadInt32();
                this.texCoordSet7 = reader.ReadInt32();

                for (int i = 0; i < 3; ++i)
                {
                    this.unk2[i] = reader.ReadVertexColor();
                }
                for (int i = 0; i < 3; ++i)
                {
                    this.unk3[i] = reader.ReadVertexColor();
                }
                for (int i = 0; i < 4; ++i)
                {
                    this.unk4[i] = reader.ReadInt32();
                }

                this.texture = reader.ReadString(0x0);
            }
        }

        public void Write(Stream stream)
        {
            using (BrgBinaryWriter writer = new BrgBinaryWriter(new LittleEndianBitConverter(), stream))
            {
                writer.Write(1280463949); // MTRL
                writer.Write(this.nameLength);
                for (int i = 0; i < 5; ++i)
                {
                    writer.Write(this.unk[i]);
                }

                writer.WriteColor3D(this.diffuse);
                writer.WriteColor3D(this.ambient);
                writer.WriteColor3D(this.specular);
                writer.WriteColor3D(this.emissive);
                writer.Write(this.specularLevel);
                writer.Write(this.alpha);

                writer.Write(this.id);

                writer.Write(this.selfIlluminating);
                writer.Write(this.clampU);
                writer.Write(this.clampV);
                writer.Write(this.lightSpecular);
                writer.Write(this.affectsAmbient);
                writer.Write(this.affectsDiffuse);
                writer.Write(this.affectsSpecular);
                writer.Write(this.updateable);

                writer.Write(this.alphaMode); // Seems to be very often 10, wave has a 2 here, phoenix has 6
                writer.Write(this.ambientIntensity);
                writer.Write(this.diffuseIntensity);
                writer.Write(this.specularIntensity);
                writer.Write(this.emissiveIntensity);
                writer.Write(this.colorTransform); // Val of 4 seems to be PC
                writer.Write(this.textureTransform);
                writer.Write(this.textureFactor); // Has something to do with Cube Map
                writer.Write(this.multiTextureMode); // Has something to do with Cube Map
                writer.Write(this.texGenMode0);
                writer.Write(this.texGenMode1); // Has something to do with Cube Map
                writer.Write(this.texCoordSet0);
                writer.Write(this.texCoordSet1);
                writer.Write(this.texCoordSet2);
                writer.Write(this.texCoordSet3);
                writer.Write(this.texCoordSet4);
                writer.Write(this.texCoordSet5);
                writer.Write(this.texCoordSet6);
                writer.Write(this.texCoordSet7);

                for (int i = 0; i < 3; ++i)
                {
                    writer.WriteColor4D(this.unk2[i]);
                }
                for (int i = 0; i < 3; ++i)
                {
                    writer.WriteColor4D(this.unk3[i]);
                }
                for (int i = 0; i < 4; ++i)
                {
                    writer.Write(this.unk4[i]);
                }

                writer.WriteString(this.texture);
            }
        }
    }
}
