using System.IO;
using System.Text;

namespace AoMEngineLibrary.Extensions;

public static class BinaryWriterExtensions
{
    public static void WriteNullTerminatedString(this BinaryWriter writer, string str)
    {
        writer.WriteNullTerminatedString(str, Encoding.UTF8);
    }

    public static void WriteNullTerminatedString(this BinaryWriter writer, string str, Encoding encoding)
    {
        var data = encoding.GetBytes(str);
        writer.Write(data);
        writer.Write((byte)0x0);
    }
}
