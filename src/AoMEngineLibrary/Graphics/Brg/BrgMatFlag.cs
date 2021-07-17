using System;

namespace AoMEngineLibrary.Graphics.Brg
{
    [Flags]
    public enum BrgMatFlag : uint
    {
        AdditiveCubeBlend = 0x10000000,
        InverseAlpha = 0x08000000,
        CubeMapInfo = 0x04000000, // use a cube map/reflection texture
        PixelXForm3 = 0x02000000, // darker player color?
        PlayerXFormColor3 = 0x01000000, // low player color overlay for faces
        Alpha = 0x00800000, // This, and Below are from Executable, except for MatNone
        SubtractiveBlend = 0x00400000, // stay with highlight
        AdditiveBlend = 0x00200000, // stay with highlight
        FaceMap = 0x00100000,
        PixelXForm2 = 0x00080000, // fuller player color
        PixelXForm1 = 0x00040000, // default player color
        SpecularExponent = 0x00020000,
        Reserved = 0x00010000, // no idea
        BumpMap = 0x00008000,
        PlayerXFormTx2 = 0x00004000, // ground texture?
        PlayerXFormTx1 = 0x00002000, // smooth/ambient?
        PlayerXFormColor2 = 0x00001000, // low player color overlay
        PlayerXFormColor1 = 0x00000800, // high player color overlay
        TwoSided = 0x00000400,
        WrapVTx3 = 0x00000200,
        WrapUTx3 = 0x00000100,
        WrapVTx2 = 0x00000080,
        WrapUTx2 = 0x00000040,
        WrapVTx1 = 0x00000020,
        WrapUTx1 = 0x00000010,
        Specular = 0x00000008,
        UseColors = 0x00000004,
        Updateable = 0x00000002,
        HasTexture = 0x00000001
    };
}
