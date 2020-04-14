using AoMEngineLibrary.Graphics.Grn.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AoMEngineLibrary.Graphics.Grn
{
    public class GrnBoneTrack : IGrnObject
    {
        public GrnFile ParentFile { get; set; }
        public string Name
        {
            get
            {
                return this.ParentFile.GetDataExtensionObjectName(this.DataExtensionIndex);
            }
        }
        public Int32 DataExtensionIndex { get; set; }
        public List<float> PositionKeys { get; set; }
        public List<float> RotationKeys { get; set; }
        public List<float> ScaleKeys { get; set; }

        public List<Vector3> Positions { get; set; }
        public List<Quaternion> Rotations { get; set; }
        public List<Matrix4x4> Scales { get; set; }

        public GrnBoneTrack(GrnFile parentFile)
        {
            this.ParentFile = parentFile;
            this.PositionKeys = new List<float>();
            this.RotationKeys = new List<float>();
            this.ScaleKeys = new List<float>();
            this.Positions = new List<Vector3>();
            this.Rotations = new List<Quaternion>();
            this.Scales = new List<Matrix4x4>();
        }

        public void Read(List<int> transformChannels, GrnAnimationTransformTrackKeysNode animTransTrackKeys)
        {
            this.DataExtensionIndex = transformChannels[animTransTrackKeys.TransformChannelIndex - 1];
            this.PositionKeys = animTransTrackKeys.PositionKeys;
            this.RotationKeys = animTransTrackKeys.RotationKeys;
            this.ScaleKeys = animTransTrackKeys.ScaleKeys;
            this.Positions = animTransTrackKeys.Positions;
            this.Rotations = animTransTrackKeys.Rotations;
            this.Scales = animTransTrackKeys.Scales;
        }
        public void Write(GrnNode animTraTraSecNode, int boneTrackIndex)
        {
            GrnAnimationTransformTrackKeysNode attKeysNode = 
                new GrnAnimationTransformTrackKeysNode(animTraTraSecNode);
            attKeysNode.TransformChannelIndex = boneTrackIndex + 1;
            attKeysNode.PositionKeys = this.PositionKeys;
            attKeysNode.RotationKeys = this.RotationKeys;
            attKeysNode.ScaleKeys = this.ScaleKeys;
            attKeysNode.Positions = this.Positions;
            attKeysNode.Rotations = this.Rotations;
            attKeysNode.Scales = this.Scales;
            animTraTraSecNode.AppendChild(attKeysNode);
        }
    }
}
