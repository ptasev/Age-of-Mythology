using AoMEngineLibrary.Graphics.Grn.Nodes;
using AoMEngineLibrary.Graphics.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoMEngineLibrary.Graphics.Grn
{
    public class GrnBoneTrack : IGrnObject
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
