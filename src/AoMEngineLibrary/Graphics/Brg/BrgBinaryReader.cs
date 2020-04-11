namespace AoMEngineLibrary.Graphics.Brg
{
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Numerics;
    using System.Text;

    public class BrgBinaryReader : BinaryReader
    {
        public BrgBinaryReader(System.IO.Stream stream)
            : base(stream)
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
        public Vector3 ReadVector3D(bool isHalf)
        {
            Vector3 v = new Vector3();

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

            return v;
        }
        #endregion

        #region ReadVector2
        public Vector2 ReadVector2D(bool isHalf = false)
        {
            Vector2 v = new Vector2();

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
        public Texel ReadTexel()
        {
            Texel t = new Texel();
            t.B = this.ReadByte();
            t.G = this.ReadByte();
            t.R = this.ReadByte();
            t.A = this.ReadByte();
            return t;
        }

        public float ReadHalf()
        {
            byte[] f = new byte[4];
            byte[] h = this.ReadBytes(2);
            f[2] = h[0];
            f[3] = h[1];
            return BitConverter.ToSingle(f, 0);
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
