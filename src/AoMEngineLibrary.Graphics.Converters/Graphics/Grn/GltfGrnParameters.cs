using System;
using System.Collections.Generic;
using System.Text;

namespace AoMEngineLibrary.Graphics.Grn
{
    public class GltfGrnParameters
    {
        public bool ConvertMeshes { get; set; }

        public bool ConvertAnimations { get; set; }

        public GltfGrnParameters()
        {
            ConvertMeshes = true;
            ConvertAnimations = false;
        }
    }
}
