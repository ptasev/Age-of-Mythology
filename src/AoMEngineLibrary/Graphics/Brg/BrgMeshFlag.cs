using System;

namespace AoMEngineLibrary.Graphics.Brg
{
    [Flags]
    public enum BrgMeshFlag : ushort
    {
        /// <summary>
        /// Mesh has animated vertex color alpha channel.
        /// </summary>
        AnimVertexColor = 0x0001,

        /// <summary>
        /// Mesh has tex coord set 1.
        /// </summary>
        Texture1 = 0x0002,

        /// <summary>
        /// Mesh has tex coords sets 2..n.
        /// </summary>
        MultiTexture = 0x0004,

        /// <summary>
        /// Deprecated - Not used after revision 0x0008.
        /// </summary>
        Animation = 0x0008,

        /// <summary>
        /// Not used.
        /// </summary>
        Reserved = 0x0010,

        /// <summary>
        /// Mesh has a vertex color channel.
        /// </summary>
        ColorChannel = 0x0020,

        /// <summary>
        /// Mesh has material included.
        /// </summary>
        Material = 0x0040,

        /// <summary>
        /// Mesh has bump/normal map info.
        /// </summary>
        BumpMapInfo = 0x0080,

        /// <summary>
        /// Mesh contains dummy objects.
        /// </summary>
        DummyObjects = 0x0100,

        /// <summary>
        /// Mesh should not be rendered with z-testing.
        /// </summary>
        NoZBuffer = 0x0200,

        /// <summary>
        /// Indicates that this is a secondary frame (2..n).
        /// </summary>
        Secondary = 0x0400,

        /// <summary>
        /// Mesh has animated texture coordinates.
        /// </summary>
        AnimTxCoords = 0x0800,

        /// <summary>
        /// Mesh is a particle system.
        /// Only seen in the Aug 2002 Multiplayer Alpha.
        /// </summary>
        ParticleSystem = 0x1000,

        /// <summary>
        /// Mesh vertices are treated as particle points with a radius.
        /// Only seen in the Aug 2002 Multiplayer Alpha.
        /// </summary>
        ParticlePoints = 0x2000,

        /// <summary>
        /// Vertex color channel is treated as alpha channel.
        /// </summary>
        AlphaChannel = 0x4000,

        /// <summary>
        /// Mesh has nimated vertex color alpha channel that snaps between key-frames.
        /// </summary>
        AnimVertexColorSnap = 0x8000
    };
}
