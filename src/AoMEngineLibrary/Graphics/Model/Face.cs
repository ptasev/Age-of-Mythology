namespace AoMEngineLibrary.Graphics.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Face
    {
        public Int16 MaterialIndex { get; set; }
        public List<Int16> Indices { get; set; }
        public List<Int32> NormalIndices { get; set; }
        public List<Int32> TextureIndices { get; set; }

        public Face()
        {
            this.MaterialIndex = -1;
            this.Indices = new List<Int16>(3);
            this.NormalIndices = new List<int>(3);
            this.TextureIndices = new List<Int32>(3);
        }
    }
}
