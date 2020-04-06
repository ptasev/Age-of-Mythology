namespace AoMEngineLibrary.Graphics.Grn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GrnFace
    {
        public int MaterialIndex { get; set; }
        public List<int> Indices { get; set; }
        public List<int> NormalIndices { get; set; }
        public List<int> TextureIndices { get; set; }

        public GrnFace()
        {
            this.MaterialIndex = -1;
            this.Indices = new List<int>(3);
            this.NormalIndices = new List<int>(3);
            this.TextureIndices = new List<int>(3);
        }
    }
}
