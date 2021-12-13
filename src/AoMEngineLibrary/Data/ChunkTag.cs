using System;
using System.Text;

namespace AoMEngineLibrary.Data;

public readonly struct ChunkTag
{
    public ushort Tag { get; }

    public ChunkTag(ushort tag) => Tag = tag;
    public ChunkTag(string tag)
    {
        var byteCount = Encoding.ASCII.GetByteCount(tag);
        if (byteCount != 2)
        {
            throw new ArgumentException("The tag must be exactly 2 characters.", nameof(tag));
        }

        Span<byte> tagBytes = stackalloc byte[2];
        Encoding.ASCII.GetBytes(tag, tagBytes);
        Tag = (ushort)(tagBytes[0] | (tagBytes[1] << 8));
    }

    public static implicit operator ChunkTag(string tag) => new ChunkTag(tag);
}
