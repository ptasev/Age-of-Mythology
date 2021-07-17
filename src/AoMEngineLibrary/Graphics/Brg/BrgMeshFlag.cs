using System;

namespace AoMEngineLibrary.Graphics.Brg
{
    [Flags]
    public enum BrgMeshFlag : ushort
    {
        ANIMVERTCOLORALPHA = 0x0001, // animated vertex color alpha
        TEXCOORDSA = 0x0002, // Mesh has first set of tex coords
        MULTTEXCOORDS = 0x0004, // mesh has texture coords sets 2..n
        ANIMATEDMESH = 0x0008, // Deprecated - Not used after revision 0x0008
        RESERVED = 0x0010, // ?
        COLORCHANNEL = 0x0020, // Mesh has a vertex color channel
        MATERIAL = 0x0040, // Mesh has material data
        BUMPMAPINFO = 0x0080, // Mesh has bump/normal map info
        ATTACHPOINTS = 0x0100, // Mesh contains dummy objects
        NOZBUFFER = 0x0200, // mesh should not be rendered with z-testing
        SECONDARYMESH = 0x0400, // Secondary Mesh 2..n
        ANIMTEXCOORDS = 0x0800, // Mesh contains animated tex coords
        PARTICLESYSTEM = 0x1000, // Mesh is a Particle System
        PARTICLEPOINTS = 0x2000, // Mesh vertices are treated as particle points with radii
        COLORALPHACHANNEL = 0x4000, // Vertex color channel is treated as alpha channel
        ANIMVERTCOLORSNAP = 0x8000 // Animated vertex colors snap between keyframes
    };
}
