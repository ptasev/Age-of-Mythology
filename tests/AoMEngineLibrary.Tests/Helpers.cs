using System;
using System.Text;
using Xunit.Abstractions;

namespace AoMEngineLibrary.Tests;

internal static class Helpers
{
    /// <summary>
    /// Generates a hex string from bytes for use in code initializer list.
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="bytesPerLine">Puts a new line after every given number of bytes.</param>
    public static string BytesToHexString(ReadOnlySpan<byte> bytes, int bytesPerLine, ITestOutputHelper? output)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < bytes.Length; ++i)
        {
            var b = bytes[i];
            var (_, rem) = Math.DivRem((i + 1), bytesPerLine);
            if (rem == 0)
            {
                sb.AppendLine($"0x{b:X2},");
            }
            else
            {
                sb.Append($"0x{b:X2},");
            }
        }

        var s = sb.ToString();
        output?.WriteLine(s);
        return s;
    }
}
