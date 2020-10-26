using System;
using System.Collections.Generic;
using System.Text;

namespace AoMEngineLibrary.Graphics.Ddt
{
    public class Bt8ImageInfo
    {
        public uint NumColors { get; set; }

        public uint RGB8Offset { get; set; }

        public uint R5G6B5Offset { get; set; }

        public uint R5G5B5Offset { get; set; }

        public uint A1R5B5G5Offset { get; set; }

        public uint A4R4B4G4Offset { get; set; }
    }
}
