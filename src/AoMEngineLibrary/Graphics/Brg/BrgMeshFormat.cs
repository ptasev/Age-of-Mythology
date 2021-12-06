using System;

namespace AoMEngineLibrary.Graphics.Brg
{
    [Flags]
    public enum BrgMeshFormat : ushort
    {
        /// <summary>
        /// Mesh is a billboard (front always faces camera).
        /// </summary>
        IsBillboard = 0x0001,

        /// <summary>
        /// Animated texture coordinates snap between key-frames.
        /// </summary>
        AnimTextureSnap = 0x0002,

        /// <summary>
        /// Has face normals.
        /// </summary>
        HasFaceNormals = 0x0004,

        /// <summary>
        /// Animation length is included in extended header.
        /// </summary>
        AnimationLength = 0x0008,

        /// <summary>
        /// Snap between key-frames.
        /// </summary>
        KeyFrameSnap = 0x0010,

        /// <summary>
        /// Don't loop the animation.
        /// </summary>
        NoLoop = 0x0020,

        /// <summary>
        /// Not used.
        /// </summary>
        Reserved2 = 0x0040,

        /// <summary>
        /// Bone based animation.
        /// </summary>
        BoneAnim = 0x0080,

        /// <summary>
        /// Face group list is included.
        /// </summary>
        FaceGroupMap = 0x0100,

        /// <summary>
        /// Mesh data is stripped.
        /// </summary>
        Stripped = 0x0200
    };
}
