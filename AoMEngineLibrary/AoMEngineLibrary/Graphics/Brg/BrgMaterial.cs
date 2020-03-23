namespace AoMEngineLibrary.Graphics.Brg
{
    using AoMEngineLibrary.Graphics.Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;

    public class BrgMaterial : Material, IEquatable<BrgMaterial>
    {
        public BrgFile ParentFile;
        public string EditorName
        {
            get
            {
                return "Mat ID: " + Id;
            }
        }

        public int Id { get; set; }
        public BrgMatFlag Flags { get; set; }
        public int unknown01b;
        public string DiffuseMap { get; set; }
        public string BumpMap { get; set; }
        public List<BrgMatSFX> sfx;

        public BrgMaterial(BrgBinaryReader reader, BrgFile file)
            : this(file)
        {
            Id = reader.ReadInt32();
            Flags = (BrgMatFlag)reader.ReadInt32();
            unknown01b = reader.ReadInt32();
            int nameLength = reader.ReadInt32();
            this.DiffuseColor = reader.ReadColor3D();
            this.AmbientColor = reader.ReadColor3D();
            this.SpecularColor = reader.ReadColor3D();
            this.EmissiveColor = reader.ReadColor3D();

            this.DiffuseMap = reader.ReadString(nameLength);
            if (Flags.HasFlag(BrgMatFlag.SpecularExponent))
            {
                this.SpecularExponent = reader.ReadSingle();
            }
            if (Flags.HasFlag(BrgMatFlag.BumpMap))
            {
                this.BumpMap = reader.ReadString(reader.ReadInt32());
            }
            if (Flags.HasFlag(BrgMatFlag.Alpha))
            {
                this.Opacity = reader.ReadSingle();
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
            : base()
        {
            this.ParentFile = file;
            this.Id = 0;
            this.Flags = 0;
            this.unknown01b = 0;

            this.DiffuseMap = string.Empty;
            this.BumpMap = string.Empty;

            this.sfx = new List<BrgMatSFX>();
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(this.Id);
            writer.Write((int)this.Flags);

            writer.Write(this.unknown01b);
            writer.Write(Encoding.UTF8.GetByteCount(this.DiffuseMap));

            writer.WriteColor3D(this.DiffuseColor);
            writer.WriteColor3D(this.AmbientColor);
            writer.WriteColor3D(this.SpecularColor);
            writer.WriteColor3D(this.EmissiveColor);

            writer.WriteString(this.DiffuseMap, 0);

            if (this.Flags.HasFlag(BrgMatFlag.SpecularExponent))
            {
                writer.Write(this.SpecularExponent);
            }
            if (this.Flags.HasFlag(BrgMatFlag.BumpMap))
            {
                writer.WriteString(this.BumpMap, 4);
            }
            if (this.Flags.HasFlag(BrgMatFlag.Alpha))
            {
                writer.Write(this.Opacity);
            }

            if (this.Flags.HasFlag(BrgMatFlag.REFLECTIONTEXTURE))
            {
                writer.Write((byte)this.sfx.Count);
                for (int i = 0; i < this.sfx.Count; i++)
                {
                    writer.Write(this.sfx[i].Id);
                    writer.WriteString(this.sfx[i].Name, 2);
                }
            }
        }

        public bool Equals(BrgMaterial m)
        {
            // If parameter is null return false:
            if ((object)m == null)
            {
                return false;
            }

            //ret = ret && this.ParentFile == m.ParentFile;
            //ret = ret && this.id == m.id;
            bool ret = this.Flags == m.Flags &&
                this.unknown01b == m.unknown01b &&
                this.DiffuseColor == m.DiffuseColor &&
                this.AmbientColor == m.AmbientColor &&
                this.SpecularColor == m.SpecularColor &&
                this.EmissiveColor == m.EmissiveColor &&
                this.DiffuseMap == m.DiffuseMap &&
                this.BumpMap == m.BumpMap &&
                this.SpecularExponent == m.SpecularExponent &&
                this.Opacity == m.Opacity &&
                this.sfx.Count == m.sfx.Count;

            if (ret)
            {
                for (int i = 0; i < this.sfx.Count; ++i)
                {
                    ret = ret && 
                        this.sfx[i].Id == m.sfx[i].Id &&
                        this.sfx[i].Name == m.sfx[i].Name;
                }
            }

            // Return true if the fields match:
            return ret;
        }
    }
}
