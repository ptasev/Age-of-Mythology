using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgBinaryReader : BinaryReader
    {
        public BrgBinaryReader(Stream stream)
            : base(stream)
        {
        }

        public BrgUserDataEntry ReadUserDataEntry(bool isParticle)
        {
            BrgUserDataEntry dataEntry;

            dataEntry.dataNameLength = ReadInt32();
            dataEntry.dataType = ReadInt32();
            switch (dataEntry.dataType)
            {
                case 1:
                    dataEntry.data = ReadInt32();
                    dataEntry.dataName = ReadString(dataEntry.dataNameLength + (int)dataEntry.data);
                    break;
                case 2:
                    dataEntry.data = ReadInt32();
                    dataEntry.dataName = ReadString(dataEntry.dataNameLength);
                    break;
                case 3:
                    dataEntry.data = ReadSingle();
                    dataEntry.dataName = ReadString(dataEntry.dataNameLength);
                    break;
                default:
                    dataEntry.data = ReadInt32();
                    dataEntry.dataName = ReadString(dataEntry.dataNameLength);
                    break;
            }

            return dataEntry;
        }

        public Vector3 ReadVector3D(bool isHalf)
        {
            var v = new Vector3();

            if (!isHalf)
            {
                v.X = ReadSingle();
                v.Y = ReadSingle();
                v.Z = ReadSingle();
            }
            else
            {
                v.X = ReadHalf();
                v.Y = ReadHalf();
                v.Z = ReadHalf();
            }

            return v;
        }

        public Vector2 ReadVector2D(bool isHalf = false)
        {
            var v = new Vector2();

            if (!isHalf)
            {
                v.X = ReadSingle();
                v.Y = ReadSingle();
            }
            else
            {
                v.X = ReadHalf();
                v.Y = ReadHalf();
            }

            return v;
        }

        public Vector4 ReadTexel()
        {
            var b = ReadByte() / 255.0f;
            var g = ReadByte() / 255.0f;
            var r = ReadByte() / 255.0f;
            var a = ReadByte() / 255.0f;
            return Vector4.Clamp(new Vector4(r, g, b, a),
                Vector4.Zero, Vector4.One);
        }

        public float ReadHalf()
        {
            var f = new byte[4];
            var h = ReadBytes(2);
            f[2] = h[0];
            f[3] = h[1];
            return BitConverter.ToSingle(f, 0);
        }
        public string ReadString(byte terminator = 0x0)
        {
            var filename = "";
            var fnBytes = new List<byte>();
            var filenameByte = ReadByte();
            while (filenameByte != terminator)
            {
                filename += (char)filenameByte;
                fnBytes.Add(filenameByte);
                filenameByte = ReadByte();
            }
            return Encoding.UTF8.GetString(fnBytes.ToArray());
        }
        public string ReadString(int length)
        {
            return Encoding.UTF8.GetString(ReadBytes(length));
        }
    }
}
