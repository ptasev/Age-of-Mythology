namespace AoMEngineLibrary.Graphics.Brg
{
    using System;
    using System.IO;
    using System.Numerics;
    using System.Text;

    public class BrgBinaryWriter : BinaryWriter
    {
        public BrgBinaryWriter(Stream stream)
            : base(stream)
        {
        }

        public BrgBinaryWriter(Stream stream, bool leaveOpen)
            : base(stream, new UTF8Encoding(false, true), leaveOpen)
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

        public void WriteVector3D(Vector3 v, bool isHalf)
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

        public void WriteTexel(Vector4 t)
        {
            var res = Vector4.Clamp(t * 255, Vector4.Zero, Vector4.One);

            this.Write((byte)res.Z); // B
            this.Write((byte)res.Y); // G
            this.Write((byte)res.X); // R
            this.Write((byte)res.W); // A
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
