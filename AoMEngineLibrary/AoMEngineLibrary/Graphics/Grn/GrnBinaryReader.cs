namespace AoMEngineLibrary.Graphics.Grn
{
    using AoMEngineLibrary.Graphics.Model;
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

        public Vector3D ReadVector3D()
        {
            Vector3D v = new Vector3D();

            v.X = this.ReadSingle();
            v.Y = this.ReadSingle();
            v.Z = this.ReadSingle();

            return v;
        }
        public Quaternion ReadQuaternion()
        {
            Quaternion q;

            q.X = this.ReadSingle();
            q.Y = this.ReadSingle();
            q.Z = this.ReadSingle();
            q.W = this.ReadSingle();

            return q;
        }
        public Matrix3x3 ReadMatrix3x3()
        {
            Matrix3x3 m;

            m.A1 = this.ReadSingle();
            m.A2 = this.ReadSingle();
            m.A3 = this.ReadSingle();
            m.B1 = this.ReadSingle();
            m.B2 = this.ReadSingle();
            m.B3 = this.ReadSingle();
            m.C1 = this.ReadSingle();
            m.C2 = this.ReadSingle();
            m.C3 = this.ReadSingle();

            return m;
        }
    }
}
