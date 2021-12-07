using System.Buffers;
using System.Diagnostics;
using System.IO;

namespace AoMEngineLibrary.Data.Bar;

internal static partial class BarFileUtils
{
    //All slashes should be backward slashes
    private const char PathSeparatorChar = '\\';
    private const string PathSeparatorString = "\\";

    public static string EntryFromPath(string entry, int offset, int length, ref char[] buffer, bool appendPathSeparator = false)
    {
        Debug.Assert(length <= entry.Length - offset);
        Debug.Assert(buffer != null);

        // Remove any leading slashes from the entry name:
        while (length > 0)
        {
            if (entry[offset] != Path.DirectorySeparatorChar &&
                entry[offset] != Path.AltDirectorySeparatorChar)
                break;

            offset++;
            length--;
        }

        if (length == 0)
            return appendPathSeparator ? PathSeparatorString : string.Empty;

        var resultLength = appendPathSeparator ? length + 1 : length;
        EnsureCapacity(ref buffer, resultLength);
        entry.CopyTo(offset, buffer, 0, length);

        // '/' is a more broadly recognized directory separator on all platforms (eg: mac, linux)
        // We don't use Path.DirectorySeparatorChar or AltDirectorySeparatorChar because this is
        // explicitly trying to standardize to '\'
        for (var i = 0; i < length; i++)
        {
            var ch = buffer[i];
            if (ch == Path.DirectorySeparatorChar || ch == Path.AltDirectorySeparatorChar)
                buffer[i] = PathSeparatorChar;
        }

        if (appendPathSeparator)
            buffer[length] = PathSeparatorChar;

        return new string(buffer, 0, resultLength);
    }

    public static void EnsureCapacity(ref char[] buffer, int min)
    {
        Debug.Assert(buffer != null);
        Debug.Assert(min > 0);

        if (buffer.Length < min)
        {
            var newCapacity = buffer.Length * 2;
            if (newCapacity < min)
                newCapacity = min;

            var oldBuffer = buffer;
            buffer = ArrayPool<char>.Shared.Rent(newCapacity);
            ArrayPool<char>.Shared.Return(oldBuffer);
        }
    }
}

