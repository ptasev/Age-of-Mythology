namespace AoMEngineLibrary.Graphics.Brg
{
    using Extensions;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BrgAnimation
    {
        public float Duration { get; set; }
        public List<float> MeshKeys { get; set; }

        public BrgAnimation()
        {
            this.Duration = 0f;
            this.MeshKeys = new List<float>();
        }
    }
}
