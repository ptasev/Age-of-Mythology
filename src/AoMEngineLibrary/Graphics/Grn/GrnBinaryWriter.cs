namespace AoMEngineLibrary.Graphics.Grn
{
    using System;
    using System.IO;
    using System.Numerics;
    using System.Text;

    public class GrnBinaryWriter : BinaryWriter
    {
        public GrnBinaryWriter(System.IO.Stream stream)
            : base(stream)
        {
        }

        public new void Write(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            this.Write(data);
            this.Write((byte)0x0);
        }

        public void Write(Vector3 v)
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
        public void WriteMatrix3x3(Matrix4x4 m)
        {
            this.Write(m.M11);
            this.Write(m.M12);
            this.Write(m.M13);
            this.Write(m.M21);
            this.Write(m.M22);
            this.Write(m.M23);
            this.Write(m.M31);
            this.Write(m.M32);
            this.Write(m.M33);
        }
    }
}
