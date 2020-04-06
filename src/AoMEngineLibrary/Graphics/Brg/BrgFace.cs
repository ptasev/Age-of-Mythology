﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AoMEngineLibrary.Graphics.Brg
{
    public class BrgFace
    {
        public short MaterialIndex { get; set; }
        public List<short> Indices { get; set; }

        public BrgFace()
        {
            this.MaterialIndex = -1;
            this.Indices = new List<short>(3);
        }
    }
}
