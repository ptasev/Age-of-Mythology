namespace AoMEngineLibrary.Graphics.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class VertexWeight
    {
        public int BoneIndex { get; set; }
        public List<int> VertexIndices { get; set; }
        public List<float> Weights { get; set; }
    }
}
