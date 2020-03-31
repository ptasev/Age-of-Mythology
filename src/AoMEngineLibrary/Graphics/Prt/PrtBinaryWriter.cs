namespace AoMEngineLibrary.Graphics.Prt
{
    using System;
    using System.IO;

    public class PrtBinaryWriter : BinaryWriter
    {
        public PrtBinaryWriter(System.IO.Stream stream)
            : base(stream)
        {
        }

        public void WriteTexel(Texel t)
        {
            this.Write(t.B);
            this.Write(t.G);
            this.Write(t.R);
            this.Write(t.A);
        }

        public new void Write(string str)
        {
            byte[] data = System.Text.Encoding.Unicode.GetBytes(str);
            this.Write(data.Length/2);
            this.Write(data);
        }
    }
}
