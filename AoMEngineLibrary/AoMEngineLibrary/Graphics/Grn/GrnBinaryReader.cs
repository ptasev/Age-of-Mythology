namespace AoMEngineLibrary.Graphics.Grn
{
    using MiscUtil.Conversion;
    using MiscUtil.IO;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class GrnBinaryReader : EndianBinaryReader
    {
        public GrnBinaryReader(System.IO.Stream stream)
            : base(new LittleEndianBitConverter(), stream)
        {
        }

        public new string ReadString()
        {
            List<byte> fnBytes = new List<byte>();
            byte filenameByte = this.ReadByte();

            while (filenameByte != 0x00)
            {
                fnBytes.Add(filenameByte);
                filenameByte = this.ReadByte();
            }

            return Encoding.UTF8.GetString(fnBytes.ToArray());
        }
    }
}
