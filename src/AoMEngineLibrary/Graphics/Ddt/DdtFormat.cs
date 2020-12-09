using System;

namespace AoMEngineLibrary.Graphics.Ddt
{
    public enum DdtFormat : byte
    {
        /// <summary>
        /// RGBA32.
        /// Unused.
        /// </summary>
        Raw32 = 1,

        /// <summary>
        /// RGB24.
        /// Unused.
        /// </summary>
        Raw24 = 2,

        /// <summary>
        /// Paletted texture supports 444, 555, 1555, 565.
        /// </summary>
        BT8 = 3,

        /// <summary>
        /// Block compression 1.
        /// AoMTT does not support alpha in this format.
        /// </summary>
        Dxt1 = 4,

        /// <summary>
        /// Block compression 1 with custom alpha data.
        /// This is the only way AoMTT supports DXT1 with 1-bit alpha.
        /// </summary>
        Dxt1Alpha = 5,

        /// <summary>
        /// Block compression 2 with 444 colors and the alpha data after the color data.
        /// This is the only way AoMTT supports DXT3.
        /// </summary>
        Dxt3Swizzled = 6,

        /// <summary>
        /// Alpha data.
        /// Unused.
        /// </summary>
        AlphaData = 7,

        //BC1 = Dxt1,

        /// <summary>
        /// Block compression 2.
        /// AoMEE only.
        /// </summary>
        BC2 = 8,

        /// <summary>
        /// Block compression 3.
        /// AoMEE only.
        /// </summary>
        BC3 = 9,

        /// <summary>
        /// RGBA32 with deflate compression.
        /// AoMEE only.
        /// </summary>
        RgbaDeflated = 10,

        /// <summary>
        /// RGB24 with deflate compression.
        /// AoMEE only.
        /// </summary>
        RgbDeflated = 11,

        /// <summary>
        /// Alpha data with deflate compression.
        /// AoMEE only. Unused.
        /// </summary>
        AlphaDeflated = 12,

        /// <summary>
        /// RG16 data with deflate compression.
        /// AoMEE only. Unused.
        /// </summary>
        RgDeflated = 13
    }

    public static class DdtFormatExtensions
    {
        public static BtiTextureFormat ToBtiTextureFormat(this DdtFormat format)
        {
            return format switch
            {
                DdtFormat.Raw32 => BtiTextureFormat.DeflatedRGBA8,
                DdtFormat.Raw24 => BtiTextureFormat.DeflatedRGB8,
                DdtFormat.BT8 => BtiTextureFormat.BC3,
                DdtFormat.Dxt1 => BtiTextureFormat.BC1,
                DdtFormat.Dxt1Alpha => BtiTextureFormat.BC1,
                DdtFormat.Dxt3Swizzled => BtiTextureFormat.BC2,
                DdtFormat.AlphaData => BtiTextureFormat.DeflatedR8,
                DdtFormat.BC2 => BtiTextureFormat.BC2,
                DdtFormat.BC3 => BtiTextureFormat.BC3,
                DdtFormat.RgbaDeflated => BtiTextureFormat.DeflatedRGBA8,
                DdtFormat.RgbDeflated => BtiTextureFormat.DeflatedRGB8,
                DdtFormat.AlphaDeflated => BtiTextureFormat.DeflatedR8,
                DdtFormat.RgDeflated => BtiTextureFormat.DeflatedRG8,
                _ => throw new InvalidOperationException($"Cannot convert ddt format {format} to bti texture format.")
            };
        }
    }
}