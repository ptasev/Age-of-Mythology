﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class GltfBrgParameters
    {
        public float SampleRateFps { get; set; }

        public GltfBrgParameters()
        {
            SampleRateFps = 15.0f;
        }
    }
}