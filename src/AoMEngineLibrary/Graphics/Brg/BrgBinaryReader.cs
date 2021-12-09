using AoMEngineLibrary.Extensions;
using System;
using System.IO;
using System.Numerics;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgBinaryReader : BinaryReader
    {
        public BrgBinaryReader(Stream stream)
            : base(stream)
        {
        }

        public BrgUserDataEntry ReadUserDataEntry()
        {
            BrgUserDataEntry dataEntry;

            dataEntry.dataNameLength = ReadInt32();
            dataEntry.dataType = ReadInt32();
            switch (dataEntry.dataType)
            {
                case 1:
                    dataEntry.data = ReadInt32();
                    dataEntry.dataName = this.ReadStringOfLength(dataEntry.dataNameLength + (int)dataEntry.data);
                    break;
                case 2:
                    dataEntry.data = ReadInt32();
                    dataEntry.dataName = this.ReadStringOfLength(dataEntry.dataNameLength);
                    break;
                case 3:
                    dataEntry.data = ReadSingle();
                    dataEntry.dataName = this.ReadStringOfLength(dataEntry.dataNameLength);
                    break;
                default:
                    dataEntry.data = ReadInt32();
                    dataEntry.dataName = this.ReadStringOfLength(dataEntry.dataNameLength);
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
                v.X = ReadAoMHalf();
                v.Y = ReadAoMHalf();
                v.Z = ReadAoMHalf();
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
                v.X = ReadAoMHalf();
                v.Y = ReadAoMHalf();
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

        private float ReadAoMHalf()
        {
            var f = new byte[4];
            var h = ReadBytes(2);
            f[2] = h[0];
            f[3] = h[1];
            return BitConverter.ToSingle(f, 0);
        }
    }
}
