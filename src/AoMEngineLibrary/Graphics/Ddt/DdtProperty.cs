using System;
using System.Collections.Generic;
using System.Text;

namespace AoMEngineLibrary.Graphics.Ddt
{
    [Flags]
    public enum DdtProperty : byte
    {
        Normal = 0,
        NoAlphaTest = 1,
        NoLowDetail = 2,
        DisplacementMap = 4,
        CubeMap = 8
    }
}
