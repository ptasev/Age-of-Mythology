namespace AoMEngineLibrary.Graphics.Brg
{
    using MiscUtil.Conversion;
    using MiscUtil.IO;
    using System;
    using System.Text;

    public class BrgBinaryWriter : EndianBinaryWriter
    {
        public BrgBinaryWriter(EndianBitConverter bitConvertor, System.IO.Stream stream)
            : base(bitConvertor, stream)
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
        public void WriteVector3(Vector3<float> v, bool isAom = true, bool isHalf = false)
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
        public void WriteVector3(ref Vector3<Int16> v, bool isAom = true)
        {
            if (isAom)
            {
                this.Write(v.X);
                this.Write(v.Y);
                this.Write(v.Z);
            }
            else
            {
                this.Write((Int16)(-v.X));
                this.Write((Int16)(-v.Z));
                this.Write(v.Y);
            }
        }
        #endregion
        #region Vector2
        public void WriteVector2(ref Vector2<float> v, bool isHalf = false)
        {
            if (!isHalf)
            {
                this.Write(v.X);
                this.Write(v.Y);
            }
            else
            {
                this.WriteHalf(v.X);
                this.WriteHalf(v.Y);
            }
        }
        #endregion

        public void WriteHalf(float half)
        {
            byte[] f = EndianBitConverter.Little.GetBytes(half);
            this.Write(f[2]);
            this.Write(f[3]);
        }
        public void WriteVertexColor(ref VertexColor vertexColor)
        {
            this.Write(vertexColor.R);
            this.Write(vertexColor.G);
            this.Write(vertexColor.B);
            this.Write(vertexColor.A);
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
