using System;
using System.Collections.Generic;
using System.Linq;

namespace AoMEngineLibrary.Graphics.Grn
{
    public class GrnVertexWeight : IEquatable<GrnVertexWeight>
    {
        public List<int> BoneIndices { get; set; }
        public List<float> Weights { get; set; }

        public GrnVertexWeight()
        {
            BoneIndices = new List<int>();
            Weights = new List<float>();
        }

        public override int GetHashCode() => BoneIndices.FirstOrDefault() ^ (int)Weights.FirstOrDefault();

        public override bool Equals(object? obj) => Equals(obj as GrnVertexWeight);

        public bool Equals(GrnVertexWeight? other)
        {
            if (other is null) return false;

            return BoneIndices.SequenceEqual(other.BoneIndices) &&
                Weights.SequenceEqual(other.Weights);
        }


        public static bool operator ==(GrnVertexWeight one, GrnVertexWeight two)
        {
            if (one is null || two is null)
                return Equals(one, two);

            return one.Equals(two);
        }

        public static bool operator !=(GrnVertexWeight one, GrnVertexWeight two)
        {
            if (one is null || two is null)
                return !Equals(one, two);

            return !(one.Equals(two));
        }
    }
}
