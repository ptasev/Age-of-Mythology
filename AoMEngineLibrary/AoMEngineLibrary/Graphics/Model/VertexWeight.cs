namespace AoMEngineLibrary.Graphics.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class VertexWeight
    {
        public List<int> BoneIndices { get; set; }
        public List<float> Weights { get; set; }

        public VertexWeight()
        {
            this.BoneIndices = new List<int>();
            this.Weights = new List<float>();
        }
    }
}
