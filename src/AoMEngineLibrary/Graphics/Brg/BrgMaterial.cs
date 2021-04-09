namespace AoMEngineLibrary.Graphics.Brg
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Numerics;
    using System.Text;

    public class BrgMaterial : IEquatable<BrgMaterial>
    {
        public BrgFile ParentFile;

        public int Id { get; set; }
        public BrgMatFlag Flags { get; set; }
        public float Reserved { get; set; }
        public byte DiffuseMapNameLength { get; private set; }

        public Vector3 DiffuseColor { get; set; }
        public Vector3 AmbientColor { get; set; }
        public Vector3 SpecularColor { get; set; }
        public Vector3 EmissiveColor { get; set; }
        public int BumpMapNameLength { get; private set; }
        public float Opacity { get; set; }
        public float SpecularExponent { get; set; }

        private string _diffuseMap;
        public string DiffuseMapName 
        {
            get => _diffuseMap;
            set
            {
                _diffuseMap = value;
                DiffuseMapNameLength = (byte)Encoding.UTF8.GetByteCount(value);
            }
        }

        private string _bumpMap;
        public string BumpMapName
        {
            get => _bumpMap;
            set
            {
                _bumpMap = value;
                BumpMapNameLength = Encoding.UTF8.GetByteCount(value);
            }
        }

        public BrgCubeMapInfo CubeMapInfo { get; set; }

        public BrgMaterial(BrgBinaryReader reader, BrgFile file)
            : this(file)
        {
            Id = reader.ReadInt32();
            Flags = (BrgMatFlag)reader.ReadInt32();
            Reserved = reader.ReadSingle();
            DiffuseMapNameLength = reader.ReadByte();
            reader.ReadBytes(3); // padding
            this.AmbientColor = reader.ReadVector3D(false);
            this.DiffuseColor = reader.ReadVector3D(false);
            this.SpecularColor = reader.ReadVector3D(false);
            this.EmissiveColor = reader.ReadVector3D(false);

            this.DiffuseMapName = reader.ReadString((int)DiffuseMapNameLength);
            if (Flags.HasFlag(BrgMatFlag.BumpMap))
            {
                BumpMapNameLength = reader.ReadInt32();
                this.BumpMapName = reader.ReadString(BumpMapNameLength);
            }

            if (Flags.HasFlag(BrgMatFlag.SpecularExponent))
            {
                this.SpecularExponent = reader.ReadSingle();
            }
            if (Flags.HasFlag(BrgMatFlag.Alpha))
            {
                this.Opacity = reader.ReadSingle();
            }

            if (Flags.HasFlag(BrgMatFlag.CubeMapInfo))
            {
                CubeMapInfo.Mode = reader.ReadByte();
                CubeMapInfo.TextureFactor = reader.ReadByte();
                var cubeMapNameLength = reader.ReadByte();
                var textureMapNameLength = reader.ReadByte();

                if (cubeMapNameLength > 0)
                {
                    CubeMapInfo.CubeMapName = reader.ReadString((int)cubeMapNameLength);
                }
                if (textureMapNameLength > 0)
                {
                    CubeMapInfo.TextureMapName = reader.ReadString((int)textureMapNameLength);
                }
            }
        }
        public BrgMaterial(BrgFile file)
        {
            this.ParentFile = file;
            this.Id = 0;
            this.Flags = 0;
            this.Reserved = 0;

            this.AmbientColor = Vector3.One;
            this.DiffuseColor = Vector3.One;
            this.SpecularColor = Vector3.Zero;
            this.EmissiveColor = Vector3.Zero;

            this.Opacity = 1f;
            this.SpecularExponent = 0f;

            this._diffuseMap = string.Empty;
            this._bumpMap = string.Empty;

            this.CubeMapInfo = new BrgCubeMapInfo();
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(this.Id);
            writer.Write((int)this.Flags);

            writer.Write(this.Reserved);
            writer.Write((int)DiffuseMapNameLength);

            writer.WriteVector3D(this.AmbientColor, false);
            writer.WriteVector3D(this.DiffuseColor, false);
            writer.WriteVector3D(this.SpecularColor, false);
            writer.WriteVector3D(this.EmissiveColor, false);

            writer.WriteString(this.DiffuseMapName, 0);
            if (this.Flags.HasFlag(BrgMatFlag.BumpMap))
            {
                writer.Write(this.BumpMapNameLength);
                writer.WriteString(this.BumpMapName, 0);
            }

            if (this.Flags.HasFlag(BrgMatFlag.SpecularExponent))
            {
                writer.Write(this.SpecularExponent);
            }
            if (this.Flags.HasFlag(BrgMatFlag.Alpha))
            {
                writer.Write(this.Opacity);
            }

            if (this.Flags.HasFlag(BrgMatFlag.CubeMapInfo))
            {
                writer.Write(CubeMapInfo.Mode);
                writer.Write(CubeMapInfo.TextureFactor);
                writer.Write(CubeMapInfo.CubeMapNameLength);
                writer.Write(CubeMapInfo.TextureMapNameLength);

                if (CubeMapInfo.CubeMapNameLength > 0)
                {
                    writer.WriteString(CubeMapInfo.CubeMapName, 0);
                }
                if (CubeMapInfo.TextureMapNameLength > 0)
                {
                    writer.WriteString(CubeMapInfo.TextureMapName, 0);
                }
            }
        }

        public bool Equals(BrgMaterial m)
        {
            // If parameter is null return false:
            if (m == null)
            {
                return false;
            }

            //ret = ret && this.ParentFile == m.ParentFile;
            //ret = ret && this.id == m.id;
            bool ret = this.Flags == m.Flags &&
                this.Reserved == m.Reserved &&
                this.AmbientColor == m.AmbientColor &&
                this.DiffuseColor == m.DiffuseColor &&
                this.SpecularColor == m.SpecularColor &&
                this.EmissiveColor == m.EmissiveColor &&
                this.DiffuseMapNameLength == m.DiffuseMapNameLength &&
                this.BumpMapNameLength == m.BumpMapNameLength &&
                this.DiffuseMapName == m.DiffuseMapName &&
                this.BumpMapName == m.BumpMapName &&
                this.SpecularExponent == m.SpecularExponent &&
                this.Opacity == m.Opacity &&
                this.CubeMapInfo.Equals(m.CubeMapInfo);

            // Return true if the fields match:
            return ret;
        }
    }
}
