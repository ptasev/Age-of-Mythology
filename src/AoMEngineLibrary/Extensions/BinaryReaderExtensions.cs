using System.Collections.Generic;
using System.IO;
using System.Numerics;
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

    public static string ReadStringOfLength(this BinaryReader reader, int length)
    {
        return reader.ReadStringOfLength(length, Encoding.UTF8);
    }

    public static string ReadStringOfLength(this BinaryReader reader, int length, Encoding encoding)
    {
        return encoding.GetString(reader.ReadBytes(length));
    }

    public static Vector3 ReadVector3(this BinaryReader reader)
    {
        // avoiding using initializer list or ctor params
        // not sure if order is guaranteed to read values in proper order
        var v = new Vector3();
        v.X = reader.ReadSingle();
        v.Y = reader.ReadSingle();
        v.Z = reader.ReadSingle();
        return v;
    }
}
