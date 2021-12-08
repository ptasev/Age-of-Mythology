using AoMEngineLibrary.Extensions;
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
                    this.WriteLengthPrefixedString(dataEntry.dataName, 0);
                    break;
                case 2:
                    Write((int)dataEntry.data);
                    this.WriteLengthPrefixedString(dataEntry.dataName, 0);
                    break;
                case 3:
                    Write((float)dataEntry.data);
                    this.WriteLengthPrefixedString(dataEntry.dataName, 0);
                    break;
                default:
                    Write((int)dataEntry.data);
                    this.WriteLengthPrefixedString(dataEntry.dataName, 0);
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

        private static readonly Vector4 MaxBytes = new(byte.MaxValue);
        private static readonly Vector4 Half = new(0.5f);
        public void WriteTexel(Vector4 t)
        {
            t *= MaxBytes;
            t += Half;
            t = Vector4.Clamp(t, Vector4.Zero, MaxBytes);

            Write((byte)t.Z); // B
            Write((byte)t.Y); // G
            Write((byte)t.X); // R
            Write((byte)t.W); // A
        }

        public void WriteHalf(float half)
        {
            var f = BitConverter.GetBytes(half);
            Write(f[2]);
            Write(f[3]);
        }
    }
}
