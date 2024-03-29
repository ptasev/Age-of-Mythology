﻿using AoMEngineLibrary.Extensions;
using System;
using System.Numerics;
using System.Text;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgMaterial : IEquatable<BrgMaterial>
    {
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
                DiffuseMapNameLength = Convert.ToByte(Encoding.UTF8.GetByteCount(value));
                _diffuseMap = value;
            }
        }

        private string _bumpMap;
        public string BumpMapName
        {
            get => _bumpMap;
            set
            {
                BumpMapNameLength = Encoding.UTF8.GetByteCount(value);
                _bumpMap = value;
            }
        }

        public BrgCubeMapInfo CubeMapInfo { get; set; }

        public BrgMaterial(BrgBinaryReader reader)
            : this()
        {
            Id = reader.ReadInt32();
            Flags = (BrgMatFlag)reader.ReadInt32();
            Reserved = reader.ReadSingle();
            DiffuseMapNameLength = reader.ReadByte();
            reader.ReadBytes(3); // padding
            AmbientColor = reader.ReadVector3D(false);
            DiffuseColor = reader.ReadVector3D(false);
            SpecularColor = reader.ReadVector3D(false);
            EmissiveColor = reader.ReadVector3D(false);

            DiffuseMapName = reader.ReadStringOfLength(DiffuseMapNameLength);
            if (Flags.HasFlag(BrgMatFlag.BumpMap))
            {
                BumpMapNameLength = reader.ReadInt32();
                BumpMapName = reader.ReadStringOfLength(BumpMapNameLength);
            }

            if (Flags.HasFlag(BrgMatFlag.SpecularExponent))
            {
                SpecularExponent = reader.ReadSingle();
            }
            if (Flags.HasFlag(BrgMatFlag.Alpha))
            {
                Opacity = reader.ReadSingle();
            }

            if (Flags.HasFlag(BrgMatFlag.CubeMapInfo))
            {
                CubeMapInfo.Mode = reader.ReadByte();
                CubeMapInfo.TextureFactor = reader.ReadByte();
                var cubeMapNameLength = reader.ReadByte();
                var textureMapNameLength = reader.ReadByte();

                if (cubeMapNameLength > 0)
                {
                    CubeMapInfo.CubeMapName = reader.ReadStringOfLength(cubeMapNameLength);
                }
                if (textureMapNameLength > 0)
                {
                    CubeMapInfo.TextureMapName = reader.ReadStringOfLength(textureMapNameLength);
                }
            }
        }
        public BrgMaterial()
        {
            Id = 0;
            Flags = 0;
            Reserved = 0;

            AmbientColor = Vector3.One;
            DiffuseColor = Vector3.One;
            SpecularColor = Vector3.Zero;
            EmissiveColor = Vector3.Zero;

            Opacity = 1f;
            SpecularExponent = 0f;

            _diffuseMap = string.Empty;
            _bumpMap = string.Empty;

            CubeMapInfo = new BrgCubeMapInfo();
        }

        public void Write(BrgBinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write((int)Flags);

            writer.Write(Reserved);
            writer.Write((int)DiffuseMapNameLength);

            writer.WriteVector3D(AmbientColor, false);
            writer.WriteVector3D(DiffuseColor, false);
            writer.WriteVector3D(SpecularColor, false);
            writer.WriteVector3D(EmissiveColor, false);

            writer.WriteLengthPrefixedString(DiffuseMapName, 0);
            if (Flags.HasFlag(BrgMatFlag.BumpMap))
            {
                writer.Write(BumpMapNameLength);
                writer.WriteLengthPrefixedString(BumpMapName, 0);
            }

            if (Flags.HasFlag(BrgMatFlag.SpecularExponent))
            {
                writer.Write(SpecularExponent);
            }
            if (Flags.HasFlag(BrgMatFlag.Alpha))
            {
                writer.Write(Opacity);
            }

            if (Flags.HasFlag(BrgMatFlag.CubeMapInfo))
            {
                writer.Write(CubeMapInfo.Mode);
                writer.Write(CubeMapInfo.TextureFactor);
                writer.Write(CubeMapInfo.CubeMapNameLength);
                writer.Write(CubeMapInfo.TextureMapNameLength);

                if (CubeMapInfo.CubeMapNameLength > 0)
                {
                    writer.WriteLengthPrefixedString(CubeMapInfo.CubeMapName, 0);
                }
                if (CubeMapInfo.TextureMapNameLength > 0)
                {
                    writer.WriteLengthPrefixedString(CubeMapInfo.TextureMapName, 0);
                }
            }
        }

        public bool Equals(BrgMaterial? m)
        {
            // If parameter is null return false:
            if (m == null)
            {
                return false;
            }

            //ret = ret && this.id == m.id;
            var ret = Flags == m.Flags &&
                Reserved == m.Reserved &&
                AmbientColor == m.AmbientColor &&
                DiffuseColor == m.DiffuseColor &&
                SpecularColor == m.SpecularColor &&
                EmissiveColor == m.EmissiveColor &&
                DiffuseMapNameLength == m.DiffuseMapNameLength &&
                BumpMapNameLength == m.BumpMapNameLength &&
                DiffuseMapName == m.DiffuseMapName &&
                BumpMapName == m.BumpMapName &&
                SpecularExponent == m.SpecularExponent &&
                Opacity == m.Opacity &&
                CubeMapInfo.Equals(m.CubeMapInfo);

            // Return true if the fields match:
            return ret;
        }

        public override bool Equals(object? obj) => Equals(obj as BrgMaterial);

        public override int GetHashCode() => HashCode.Combine(Id, Flags);
    }
}
