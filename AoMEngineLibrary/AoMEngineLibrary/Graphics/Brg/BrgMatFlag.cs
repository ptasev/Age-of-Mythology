namespace AoMEngineLibrary.Graphics.Brg
{
    using System;

    [Flags]
    public enum BrgMatFlag : uint
    {
        ILLUMREFLECTION = 0x10000000, // Don't use the VectorFloat specular?
        MATNONE10 = 0x08000000, // no idea
        REFLECTIONTEXTURE = 0x04000000, // use a reflection texture
        PLAYERCOLOR2 = 0x02000000, // darker player color?
        LOWPLAYERCOLOR2 = 0x01000000, // low player color overlay for faces
        DIFFUSETEXTURE = 0x00800000, // use texture, no idea
        USESELFILLUMCOLOR = 0x00400000, // black, stay with highlight
        WHITESELFILLUMCOLOR = 0x00200000, // white, stay with highlight
        MATNONE14 = 0x00100000, // white, except for highlight
        PLAYERCOLOR4 = 0x00080000, // fuller player color
        PLAYERCOLOR = 0x00040000, // default player color
        USESPECLVL = 0x00020000, // also use specular level var
        MATNONE16 = 0x00010000, // no idea
        MATTEXURE2 = 0x00008000, // use texture for something
        GROUNDTEXTUREOVERLAY = 0x00004000, // ground texture?
        MATNONE19 = 0x00002000, // smooth/ambient?
        LOWPLAYERCOLOROVERLAY = 0x00001000, // low player color overlay
        PLAYERCOLOROVERWHITE = 0x00000800, // high player color overlay
        MATNONE22 = 0x00000400, // 
        MATNONE23 = 0x00000200, // does nothing?
        MATNONE24 = 0x00000100, // 
        MATNONE25 = 0x00000030
    };
}
