namespace AoMEngineLibrary.Graphics.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Animation
    {
        public string Name { get; set; }
        public float Duration { get; set; }
        public float TimeStep { get; set; }
        public MeshAnimationChannel MeshChannel { get; set; }

        public Animation()
        {
            this.Name = "(unnamed)";
            this.Duration = 1f;
            this.TimeStep = 1f;
            this.MeshChannel = new MeshAnimationChannel();
        }
    }
}
