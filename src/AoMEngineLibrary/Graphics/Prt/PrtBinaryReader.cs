namespace AoMEngineLibrary.Graphics.Prt
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Numerics;
    using System.Text;

    public class PrtBinaryReader : BinaryReader
    {
        public PrtBinaryReader(System.IO.Stream stream)
            : base(stream)
        {
        }

        public Vector4 ReadTexel()
        {
            return Vector4.Clamp(new Vector4(this.ReadByte() / 255.0f, this.ReadByte() / 255.0f, this.ReadByte() / 255.0f, this.ReadByte() / 255.0f),
                Vector4.Zero, Vector4.One);
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
