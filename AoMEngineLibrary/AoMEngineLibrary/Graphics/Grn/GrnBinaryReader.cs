namespace AoMEngineLibrary.Graphics.Grn
{
    using AoMEngineLibrary.Graphics.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Numerics;
    using System.Text;

    public class GrnBinaryReader : BinaryReader
    {
        public GrnBinaryReader(System.IO.Stream stream)
            : base(stream)
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

        public Vector3 ReadVector3D()
        {
            Vector3 v = new Vector3();

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
        public Matrix4x4 ReadMatrix3x3()
        {
            Matrix4x4 m = new Matrix4x4();

            m.M11 = this.ReadSingle();
            m.M12 = this.ReadSingle();
            m.M13 = this.ReadSingle();
            m.M21 = this.ReadSingle();
            m.M22 = this.ReadSingle();
            m.M23 = this.ReadSingle();
            m.M31 = this.ReadSingle();
            m.M32 = this.ReadSingle();
            m.M33 = this.ReadSingle();

            return m;
        }
    }
}
