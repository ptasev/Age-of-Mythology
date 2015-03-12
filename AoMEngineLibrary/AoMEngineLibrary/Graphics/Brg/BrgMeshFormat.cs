namespace AoMEngineLibrary.Graphics.Brg
{
    using System;

    [Flags]
    public enum BrgMeshFormat : ushort
    {
        BILLBOARD = 0x0001, // rotates with the player view
        ANIMTEXCOORDSNAP = 0x0002, // Animated UV/animated texture coords snap between keyframes
        HASFACENORMALS = 0x0004, // has face normals
        ANIMATED = 0x0008, // animation length included in extended header
        KEYFRAMESNAP = 0x0010, // keyframe snap, not smooth
        NOLOOPANIMATE = 0x0020, // don't animate Last-First frame
        MFRESRVED0 = 0x0040, // ?
        FACEGROUPMAP = 0x0080, // Mesh has face group list
        STRIPPED = 0x0100  // Mesh data is stripped
    };
}
