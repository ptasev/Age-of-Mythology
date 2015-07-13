using AoMEngineLibrary.Graphics.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoMEngineLibrary.Graphics.Grn
{
    public class GrnBoneTrack
    {
        public Int32 DataExtensionIndex { get; set; }
        public List<float> PositionKeys { get; set; }
        public List<float> RotationKeys { get; set; }
        public List<float> ScaleKeys { get; set; }

        public List<Vector3D> Positions { get; set; }
        public List<Quaternion> Rotations { get; set; }
        public List<Matrix3x3> Scales { get; set; }

        public GrnBoneTrack()
        {
            this.PositionKeys = new List<float>();
            this.RotationKeys = new List<float>();
            this.ScaleKeys = new List<float>();
            this.Positions = new List<Vector3D>();
            this.Rotations = new List<Quaternion>();
            this.Scales = new List<Matrix3x3>();
        }

        public void Read(GrnBinaryReader reader, List<int> transformChannels, GrnNode animTransTrackKeys, uint directoryOffset)
        {
            reader.Seek((int)(animTransTrackKeys.Offset + directoryOffset), SeekOrigin.Begin);
            this.DataExtensionIndex = transformChannels[reader.ReadInt32() - 1];
            
            // read 5 unknowns
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadInt32();

            int numPositions = reader.ReadInt32();
            int numRotations = reader.ReadInt32();
            int numTransforms = reader.ReadInt32();

            // read 4 unknowns
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadInt32();

            for (int i = 0; i < numPositions; ++i)
            {
                this.PositionKeys.Add(reader.ReadSingle());
            }
            for (int i = 0; i < numRotations; ++i)
            {
                this.RotationKeys.Add(reader.ReadSingle());
            }
            for (int i = 0; i < numTransforms; ++i)
            {
                this.ScaleKeys.Add(reader.ReadSingle());
            }

            for (int i = 0; i < numPositions; ++i)
            {
                this.Positions.Add(reader.ReadVector3D());
            }
            for (int i = 0; i < numRotations; ++i)
            {
                this.Rotations.Add(reader.ReadQuaternion());
            }
            for (int i = 0; i < numTransforms; ++i)
            {
                this.Scales.Add(reader.ReadMatrix3x3());
            }
        }
    }
}
