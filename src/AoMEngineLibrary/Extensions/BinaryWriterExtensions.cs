using System;
using System.IO;
using System.Numerics;
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

    public static void WriteLengthPrefixedString(this BinaryWriter writer, string str, int lengthSize)
    {
        writer.WriteLengthPrefixedString(str, lengthSize, Encoding.UTF8);
    }

    public static void WriteLengthPrefixedString(this BinaryWriter writer, string str, int lengthSize, Encoding encoding)
    {
        var data = encoding.GetBytes(str);

        switch (lengthSize)
        {
            case 0:
                break;
            case 1:
                writer.Write(Convert.ToByte(data.Length));
                break;
            case 2:
                writer.Write(Convert.ToUInt16(data.Length));
                break;
            case 4:
                writer.Write(data.Length);
                break;
            default:
                throw new ArgumentException("Unsupported string prefix length byte size.", nameof(lengthSize));
        }

        writer.Write(data);
    }

    public static void WriteVector3(this BinaryWriter writer, Vector3 v)
    {
        writer.Write(v.X);
        writer.Write(v.Y);
        writer.Write(v.Z);
    }
}
