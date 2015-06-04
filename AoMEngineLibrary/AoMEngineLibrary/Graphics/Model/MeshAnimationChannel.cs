namespace AoMEngineLibrary.Graphics.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MeshAnimationChannel
    {
        public string Name { get; set; }
        public List<float> MeshTimes { get; set; }

        public MeshAnimationChannel()
        {
            this.Name = "(unnamed)";
            this.MeshTimes = new List<float>();
        }
    }
}
