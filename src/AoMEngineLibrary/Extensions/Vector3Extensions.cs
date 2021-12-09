using System.Numerics;

namespace AoMEngineLibrary.Extensions;

public static class Vector3Extensions
{
    public static bool IsFinite(this in Vector3 vec)
    {
        return float.IsFinite(vec.X) && float.IsFinite(vec.Y) && float.IsFinite(vec.Z);
    }
}
