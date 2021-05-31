using System.Collections.Generic;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgAnimation
    {
        public float Duration { get; set; }

        public List<float> MeshKeys { get; }

        public BrgAnimation()
        {
            Duration = 0f;
            MeshKeys = new List<float>();
        }
    }
}
