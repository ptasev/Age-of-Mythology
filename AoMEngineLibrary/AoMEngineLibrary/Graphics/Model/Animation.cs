namespace AoMEngineLibrary.Graphics.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Animation
    {
        public float Duration { get; set; }
        public float TimeStep { get; set; }
        public List<float> MeshKeys { get; set; }

        public Animation()
        {
            this.Duration = 1f;
            this.TimeStep = 1f;
            this.MeshKeys = new List<float>();
        }
    }
}
