using System;
using System.Numerics;

namespace AoMEngineLibrary.Graphics.Textures
{
    public interface IBcBlock
    {
        void Decode(Span<Vector4> colors);
    }
}
