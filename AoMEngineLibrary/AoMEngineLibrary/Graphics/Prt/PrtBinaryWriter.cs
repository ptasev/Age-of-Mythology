namespace AoMEngineLibrary.Graphics.Prt
{
    using MiscUtil.Conversion;
    using MiscUtil.IO;
    using System;

    public class PrtBinaryWriter : EndianBinaryWriter
    {
        public PrtBinaryWriter(EndianBitConverter bitConvertor, System.IO.Stream stream)
            : base(bitConvertor, stream)
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
