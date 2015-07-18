namespace AoMEngineLibrary.Graphics.Grn
{
    using AoMEngineLibrary.Graphics.Model;
    using MiscUtil.Conversion;
    using MiscUtil.IO;
    using System;
    using System.Text;

    public class GrnBinaryWriter : EndianBinaryWriter
    {
        public GrnBinaryWriter(System.IO.Stream stream)
            : base(new LittleEndianBitConverter(), stream)
        {
        }

        public new void Write(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            this.Write(data);
            this.Write((byte)0x0);
        }

        public void Write(Vector3D v)
        {
            this.Write(v.X);
            this.Write(v.Y);
            this.Write(v.Z);
        }
        public void Write(Quaternion q)
        {
            this.Write(q.X);
            this.Write(q.Y);
            this.Write(q.Z);
            this.Write(q.W);
        }
        public void Write(Matrix3x3 m)
        {
            this.Write(m.A1);
            this.Write(m.A2);
            this.Write(m.A3);
            this.Write(m.B1);
            this.Write(m.B2);
            this.Write(m.B3);
            this.Write(m.C1);
            this.Write(m.C2);
            this.Write(m.C3);
        }
    }
}
