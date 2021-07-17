namespace AoMEngineLibrary.Graphics.Brg
{
    public enum BrgMeshAnimType : byte
    {
        KeyFrame = 0x0000, // keyframe based animation
        NonUniform = 0x0001, // Non-uniform animation
        SkinBone = 0x0002 // Skinned Animation
    };
}
