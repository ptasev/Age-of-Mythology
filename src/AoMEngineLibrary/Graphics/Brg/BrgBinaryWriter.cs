using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace AoMEngineLibrary.Graphics.Brg
{
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
            Write(dataEntry.dataNameLength);
            Write(dataEntry.dataType);
            switch (dataEntry.dataType)
            {
                case 1:
                    Write((int)dataEntry.data);
                    WriteString(dataEntry.dataName, 0);
                    break;
                case 2:
                    Write((int)dataEntry.data);
                    WriteString(dataEntry.dataName, 0);
                    break;
                case 3:
                    Write((float)dataEntry.data);
                    WriteString(dataEntry.dataName, 0);
                    break;
                default:
                    Write((int)dataEntry.data);
                    WriteString(dataEntry.dataName, 0);
                    break;
            }
        }

        public void WriteVector3D(Vector3 v, bool isHalf)
        {
            if (!isHalf)
            {
                Write(v.X);
                Write(v.Y);
                Write(v.Z);
            }
            else
            {
                WriteHalf(v.X);
                WriteHalf(v.Y);
                WriteHalf(v.Z);
            }
        }

        public void WriteTexel(Vector4 t)
        {
            var res = Vector4.Clamp(t * 255, Vector4.Zero, Vector4.One);

            Write((byte)res.Z); // B
            Write((byte)res.Y); // G
            Write((byte)res.X); // R
            Write((byte)res.W); // A
        }

        public void WriteHalf(float half)
        {
            var f = BitConverter.GetBytes(half);
            Write(f[2]);
            Write(f[3]);
        }
        public void WriteString(string str)
        {
            var data = Encoding.UTF8.GetBytes(str);
            Write(data);
            Write((byte)0x0);
        }
        public void WriteString(string str, int lengthSize)
        {
            var data = Encoding.UTF8.GetBytes(str);
            if (lengthSize == 2)
            {
                Write((short)data.Length);
            }
            else if (lengthSize == 4)
            {
                Write(data.Length);
            }
            Write(data);
        }
    }
}
