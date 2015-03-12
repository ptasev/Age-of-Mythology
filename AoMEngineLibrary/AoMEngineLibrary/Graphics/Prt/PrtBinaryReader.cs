namespace AoMEngineLibrary.Graphics.Prt
{
    using MiscUtil.Conversion;
    using MiscUtil.IO;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class PrtBinaryReader : EndianBinaryReader
    {
        public PrtBinaryReader(EndianBitConverter bitConvertor, System.IO.Stream stream)
            : base(bitConvertor, stream)
        {
        }

        public VertexColor ReadVertexColor()
        {
            VertexColor color = new VertexColor();
            color.R = this.ReadByte();
            color.G = this.ReadByte();
            color.B = this.ReadByte();
            color.A = this.ReadByte();
            return color;
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
