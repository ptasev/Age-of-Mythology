namespace AoMEngineLibrary.Graphics.Prt
{
    using System;
    using System.IO;
    using System.Numerics;

    public class PrtBinaryWriter : BinaryWriter
    {
        public PrtBinaryWriter(System.IO.Stream stream)
            : base(stream)
        {
        }

        public void WriteTexel(Vector4 t)
        {
            var res = Vector4.Clamp(t * 255, Vector4.Zero, Vector4.One);

            this.Write((byte)res.X);
            this.Write((byte)res.Y);
            this.Write((byte)res.Z);
            this.Write((byte)res.W);
        }

        public new void Write(string str)
        {
            byte[] data = System.Text.Encoding.Unicode.GetBytes(str);
            this.Write(data.Length/2);
            this.Write(data);
        }
    }
}
