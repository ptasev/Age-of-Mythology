namespace AoMEngineLibrary.Graphics.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Face
    {
        public List<Int16> Indices { get; set; }
        public Int16 MaterialIndex { get; set; }

        public Face()
        {
            this.Indices = new List<Int16>(3);
            this.MaterialIndex = -1;
        }
    }
}
