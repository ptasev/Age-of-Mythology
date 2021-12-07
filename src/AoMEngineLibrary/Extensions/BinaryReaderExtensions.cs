using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AoMEngineLibrary.Extensions;

public static class BinaryReaderExtensions
{
    public static string ReadNullTerminatedString(this BinaryReader reader)
    {
        return reader.ReadNullTerminatedString(Encoding.UTF8);
    }

    public static string ReadNullTerminatedString(this BinaryReader reader, Encoding encoding)
    {
        var strBytes = new List<byte>();
        var filenameByte = reader.ReadByte();
        while (filenameByte != 0)
        {
            strBytes.Add(filenameByte);
            filenameByte = reader.ReadByte();
        }
        return encoding.GetString(strBytes.ToArray());
    }
}
