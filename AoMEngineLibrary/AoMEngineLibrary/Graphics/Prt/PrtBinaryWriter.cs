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

        public void WriteVertexColor(VertexColor vertexColor)
        {
            this.Write(vertexColor.R);
            this.Write(vertexColor.G);
            this.Write(vertexColor.B);
            this.Write(vertexColor.A);
        }

        public new void Write(string str)
        {
            byte[] data = System.Text.Encoding.Unicode.GetBytes(str);
            this.Write(data.Length/2);
            this.Write(data);
        }
    }
}
