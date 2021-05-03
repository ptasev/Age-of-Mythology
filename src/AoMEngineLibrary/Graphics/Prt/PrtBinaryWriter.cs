using System.IO;
using System.Numerics;
using System.Text;

namespace AoMEngineLibrary.Graphics.Prt
{
    public class PrtBinaryWriter : BinaryWriter
    {
        private static readonly Vector4 MaxTexel = new Vector4(byte.MaxValue);

        public PrtBinaryWriter(Stream stream)
            : base(stream, Encoding.Unicode, true)
        {
        }

        public void WriteTexel(Vector4 t)
        {
            var res = Vector4.Clamp(t * 255, Vector4.Zero, MaxTexel);

            this.Write((byte)res.X);
            this.Write((byte)res.Y);
            this.Write((byte)res.Z);
            this.Write((byte)res.W);
        }

        public new void Write(string str)
        {
            byte[] data = Encoding.Unicode.GetBytes(str);
            this.Write(data.Length/2);
            this.Write(data);
        }
    }
}
