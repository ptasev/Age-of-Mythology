namespace AoMEngineLibrary.Graphics.Brg
{
    using System;

    public enum BrgMeshAnimType : byte
    {
        KEYFRAME = 0x0000, // keyframe based animation
        NONUNIFORM = 0x0001, // Non-uniform animation
        SKINBONE = 0x0002 // Skinned Animation
    };
}
