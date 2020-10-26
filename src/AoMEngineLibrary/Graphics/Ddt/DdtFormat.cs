namespace AoMEngineLibrary.Graphics.Ddt
{
    public enum DdtFormat : byte
    {
        Raw32 = 1,
        Raw24 = 2,
        BT8 = 3,
        Dxt1 = 4,
        Dxt1Alpha = 5,
        Dxt3Swizzled = 6,
        AlphaData = 7,
        //BC1 = Dxt1,
        BC2 = 8,
        BC3 = 9,
        RgbaDeflated = 10,
        RgbDeflated = 11,
        AlphaDeflated = 12,
        RgDeflated = 13
    }
}