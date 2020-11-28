namespace AoMEngineLibrary.Graphics.Grn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GrnVertexWeight : IEquatable<GrnVertexWeight>
    {
        public List<int> BoneIndices { get; set; }
        public List<float> Weights { get; set; }

        public GrnVertexWeight()
        {
            this.BoneIndices = new List<int>();
            this.Weights = new List<float>();
        }

        public override int GetHashCode()
        {
            return BoneIndices.FirstOrDefault() ^ (int)Weights.FirstOrDefault();
        }

        public override bool Equals(object obj)
        {
            if (obj is GrnVertexWeight vw)
                return Equals(vw);
            else
                return false;
        }

        public bool Equals(GrnVertexWeight other)
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
