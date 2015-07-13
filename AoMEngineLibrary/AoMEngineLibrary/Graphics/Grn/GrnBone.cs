using AoMEngineLibrary.Graphics.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoMEngineLibrary.Graphics.Grn
{
    public class GrnBone
    {
        public GrnFile ParentFile { get; set; }
        public string Name
        {
            get
            {
                return this.ParentFile.GetDataExtensionObjectName(this.DataExtensionIndex);
            }
        }
        public Int32 ParentIndex { get; set; }
        public Int32 DataExtensionIndex { get; set; }
        public Vector3D Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Matrix3x3 Scale { get; set; }

        public GrnBone()
        {
            this.ParentIndex = -1;
            this.Position = new Vector3D(0f, 0f, 0f);
            this.Rotation = new Quaternion(0, 0, 0, 1);
            this.Scale = Matrix3x3.Identity;
        }
        public GrnBone(GrnFile parentFile)
            : this()
        {
            this.ParentFile = parentFile;
        }

        public void Read(GrnBinaryReader reader, GrnNode boneNode, uint directoryOffset)
        {
            reader.Seek((int)(boneNode.Offset + directoryOffset), SeekOrigin.Begin);
            this.ParentIndex = reader.ReadInt32();
            this.Position = reader.ReadVector3D();
            this.Rotation = reader.ReadQuaternion();
            this.Scale = reader.ReadMatrix3x3();
        }

    }
}
