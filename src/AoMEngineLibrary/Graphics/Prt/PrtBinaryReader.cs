using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace AoMEngineLibrary.Graphics.Prt
{
    public class PrtBinaryReader : BinaryReader
    {
        public PrtBinaryReader(Stream stream)
            : base(stream, Encoding.Unicode, true)
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
