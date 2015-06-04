namespace AoMEngineLibrary.Graphics.Brg
{
    using AoMEngineLibrary.Graphics.Model;
    using MiscUtil.Conversion;
    using MiscUtil.IO;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class BrgBinaryReader : EndianBinaryReader
    {
        public BrgBinaryReader(EndianBitConverter bitConvertor, System.IO.Stream stream)
            : base(bitConvertor, stream)
        {
        }

        public BrgUserDataEntry ReadUserDataEntry(bool isParticle)
        {
            BrgUserDataEntry dataEntry;

            dataEntry.dataNameLength = this.ReadInt32();
            dataEntry.dataType = this.ReadInt32();
            switch (dataEntry.dataType)
            {
                case 1:
                    dataEntry.data = this.ReadInt32();
                    dataEntry.dataName = this.ReadString(dataEntry.dataNameLength + (int)dataEntry.data);
                    break;
                case 2:
                    dataEntry.data = this.ReadInt32();
                    dataEntry.dataName = this.ReadString(dataEntry.dataNameLength);
                    break;
                case 3:
                    dataEntry.data = this.ReadSingle();
                    dataEntry.dataName = this.ReadString(dataEntry.dataNameLength);
                    break;
                default:
                    dataEntry.data = this.ReadInt32();
                    dataEntry.dataName = this.ReadString(dataEntry.dataNameLength);
                    break;
            }

            return dataEntry;
        }
        public BrgMatSFX ReadMaterialSFX()
        {
            BrgMatSFX sfx;

            sfx.Id = this.ReadByte();
            sfx.Name = this.ReadString(this.ReadInt16());

            return sfx;
        }

        #region ReadVector3
        public Vector3D ReadVector3D(bool isAom = true, bool isHalf = false)
        {
            Vector3D v = new Vector3D();

            if (isAom)
            {
                if (!isHalf)
                {
                    v.X = this.ReadSingle();
                    v.Y = this.ReadSingle();
                    v.Z = this.ReadSingle();
                }
                else
                {
                    v.X = this.ReadHalf();
                    v.Y = this.ReadHalf();
                    v.Z = this.ReadHalf();
                }
            }
            else
            {
                if (!isHalf)
                {
                    v.X = -this.ReadSingle();
                    v.Z = -this.ReadSingle();
                    v.Y = this.ReadSingle();
                }
                else
                {
                    v.X = -this.ReadHalf();
                    v.Z = -this.ReadHalf();
                    v.Y = this.ReadHalf();
                }
            }

            return v;
        }
        #endregion

        #region ReadVector2
        public Vector2D ReadVector2D(bool isHalf = false)
        {
            Vector2D v = new Vector2D();

            if (!isHalf)
            {
                v.X = this.ReadSingle();
                v.Y = this.ReadSingle();
            }
            else
            {
                v.X = this.ReadHalf();
                v.Y = this.ReadHalf();
            }

            return v;
        }
        #endregion

        public Color3D ReadColor3D()
        {
            Color3D color = new Color3D();

            color.R = this.ReadSingle();
            color.G = this.ReadSingle();
            color.B = this.ReadSingle();

            return color;
        }
        public Color4D ReadVertexColor()
        {
            Color4D color = new Color4D();

            color.R = this.ReadByte() / (float)Byte.MaxValue;
            color.G = this.ReadByte() / (float)Byte.MaxValue;
            color.B = this.ReadByte() / (float)Byte.MaxValue;
            color.A = this.ReadByte() / (float)Byte.MaxValue;

            return color;
        }

        public float ReadHalf()
        {
            byte[] f = new byte[4];
            byte[] h = this.ReadBytes(2);
            f[2] = h[0];
            f[3] = h[1];
            return EndianBitConverter.Little.ToSingle(f, 0);
        }
        public string ReadString(byte terminator = 0x0)
        {
            string filename = "";
            List<byte> fnBytes = new List<byte>();
            byte filenameByte = this.ReadByte();
            while (filenameByte != terminator)
            {
                filename += (char)filenameByte;
                fnBytes.Add(filenameByte);
                filenameByte = this.ReadByte();
            }
            return Encoding.UTF8.GetString(fnBytes.ToArray());
        }
        public string ReadString(int length)
        {
            return Encoding.UTF8.GetString(this.ReadBytes(length));
        }
    }
}
