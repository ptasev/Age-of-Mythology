namespace AoMEngineLibrary.Graphics.Prt
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class PrtBinaryReader : BinaryReader
    {
        public PrtBinaryReader(System.IO.Stream stream)
            : base(stream)
        {
        }

        public Texel ReadTexel()
        {
            Texel t = new Texel();
            t.B = this.ReadByte();
            t.G = this.ReadByte();
            t.R = this.ReadByte();
            t.A = this.ReadByte();
            return t;
        }

        public new string ReadString()
        {
            Int32 length = this.ReadInt32();
            return Encoding.Unicode.GetString(this.ReadBytes(2*length));
        }
        public string ReadString(int length)
        {
            return Encoding.Unicode.GetString(this.ReadBytes(2*length));
        }
    }
}
