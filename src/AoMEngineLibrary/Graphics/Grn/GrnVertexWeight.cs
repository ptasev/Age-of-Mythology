namespace AoMEngineLibrary.Graphics.Grn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GrnVertexWeight
    {
        public List<int> BoneIndices { get; set; }
        public List<float> Weights { get; set; }

        public GrnVertexWeight()
        {
            this.BoneIndices = new List<int>();
            this.Weights = new List<float>();
        }
    }
}
