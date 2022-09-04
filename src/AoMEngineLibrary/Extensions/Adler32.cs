using System;
using System.Buffers;
using System.IO;

namespace AoMEngineLibrary.Extensions;

public static class Adler32
{
    /// <summary>
    /// Update the given checksum with the source data.
    /// </summary>
    /// <param name="checksum">The current checksum. Typically starts with 1 on first call.</param>
    /// <param name="source">The source data with which to update the checksum.</param>
    /// <returns>The updated checksum.</returns>
    public static uint Update(uint checksum, ReadOnlySpan<byte> source)
    {
        const int mod = 65521;
        var a = checksum & 0xFFFF;
        var b = checksum >> 16;
        for (var i = 0; i < source.Length; ++i)
        {
            a = (a + source[i]) % mod;
            b = (b + a) % mod;
        }

        return (b << 16) | a;
    }

    /// <summary>
    /// Update the given checksum with the stream's data.
    /// </summary>
    /// <param name="checksum">The current checksum. Typically starts with 1 on first call.</param>
    /// <param name="stream">The stream data with which to update the checksum.</param>
    /// <returns>The updated checksum.</returns>
    public static uint Update(uint checksum, Stream stream)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(4096);

        while (true)
        {
            var read = stream.Read(buffer, 0, buffer.Length);

            if (read == 0)
            {
                break;
            }

            checksum = Update(checksum, new ReadOnlySpan<byte>(buffer, 0, read));
        }

        ArrayPool<byte>.Shared.Return(buffer);
        return checksum;
    }
}
