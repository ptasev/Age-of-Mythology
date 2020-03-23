namespace AoMEngineLibrary.Graphics.Brg
{
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.IO;
    using System.Numerics;
    using System.Text;

    public class BrgBinaryWriter : BinaryWriter
    {
        public BrgBinaryWriter(System.IO.Stream stream)
            : base(stream)
        {
        }

        public void WriteUserDataEntry(ref BrgUserDataEntry dataEntry, bool isParticle)
        {
            this.Write(dataEntry.dataNameLength);
            this.Write(dataEntry.dataType);
            switch (dataEntry.dataType)
            {
                case 1:
                    this.Write((Int32)dataEntry.data);
                    this.WriteString(dataEntry.dataName, 0);
                    break;
                case 2:
                    this.Write((Int32)dataEntry.data);
                    this.WriteString(dataEntry.dataName, 0);
                    break;
                case 3:
                    this.Write((Single)dataEntry.data);
                    this.WriteString(dataEntry.dataName, 0);
                    break;
                default:
                    this.Write((Int32)dataEntry.data);
                    this.WriteString(dataEntry.dataName, 0);
                    break;
            }
        }

        #region Vector3
        public void WriteVector3D(Vector3 v, bool isAom = true, bool isHalf = false)
        {
            if (isAom)
            {
                if (!isHalf)
                {
                    this.Write(v.X);
                    this.Write(v.Y);
                    this.Write(v.Z);
                }
                else
                {
                    this.WriteHalf(v.X);
                    this.WriteHalf(v.Y);
                    this.WriteHalf(v.Z);
                }
            }
            else
            {
                if (!isHalf)
                {
                    this.Write(-v.X);
                    this.Write(-v.Z);
                    this.Write(v.Y);
                }
                else
                {
                    this.WriteHalf(-v.X);
                    this.WriteHalf(-v.Z);
                    this.WriteHalf(v.Y);
                }
            }
        }
        #endregion

        public void WriteColor3D(Color3D color)
        {
            this.Write(color.R);
            this.Write(color.G);
            this.Write(color.B);
        }
        public void WriteColor4D(Color4D color)
        {
            this.Write(color.R);
            this.Write(color.G);
            this.Write(color.B);
            this.Write(color.A);
        }
        public void WriteTexel(Texel t)
        {
            this.Write(t.B);
            this.Write(t.G);
            this.Write(t.R);
            this.Write(t.A);
        }

        public void WriteHalf(float half)
        {
            byte[] f = BitConverter.GetBytes(half);
            this.Write(f[2]);
            this.Write(f[3]);
        }
        public void WriteString(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            this.Write(data);
            this.Write((byte)0x0);
        }
        public void WriteString(string str, int lengthSize)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            if (lengthSize == 2)
            {
                this.Write((Int16)data.Length);
            }
            else if (lengthSize == 4)
            {
                this.Write(data.Length);
            }
            this.Write(data);
        }
    }
}
