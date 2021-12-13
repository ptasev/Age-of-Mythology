using AoMEngineLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace AoMEngineLibrary.Data;

public sealed class ChunkReader : IDisposable
{
    private readonly BinaryReader _reader;
    private readonly Stack<Entry> _chunkStack;

    public ChunkReader(BinaryReader reader)
    {
        _reader = reader;
        _chunkStack = new Stack<Entry>();
    }

    private void ReadTag(out ChunkTag tag, out int size)
    {
        tag = new ChunkTag(_reader.ReadUInt16());

        // the size of the data in this chunk
        size = _reader.ReadInt32();
    }

    private void ReadExpectedTagSimple(ChunkTag tag, out int size)
    {
        ReadTag(out var actualTag, out size);

        if (actualTag.Tag != tag.Tag)
        {
            throw new FileFormatException("Could not read expected tag.");
        }
    }

    /// <summary>
    /// Reads the chunk's tag and data size.
    /// </summary>
    /// <param name="tag">The expected chunk tag.</param>
    /// <param name="atPosition">The position at which to read, or current position if less than 0.</param>
    public void ReadExpectedTag(ChunkTag tag, long atPosition = -1) => ReadExpectedTag(tag, out _, atPosition);

    /// <summary>
    /// Reads the chunk's tag and data size.
    /// </summary>
    /// <param name="tag">The expected chunk tag.</param>
    /// <param name="size">The data size of the chunk.</param>
    /// <param name="atPosition">The position at which to read, or current position if less than 0.</param>
    public void ReadExpectedTag(ChunkTag tag, out int size, long atPosition = -1)
    {
        var previousPosition = -1L;
        if (atPosition >= 0)
        {
            previousPosition = _reader.BaseStream.Position;
            _reader.BaseStream.Seek(atPosition, SeekOrigin.Begin);
        }

        ReadExpectedTagSimple(tag, out size);
        var startPosition = _reader.BaseStream.Position;

        var entry = new Entry(tag, size, startPosition, previousPosition);
        _chunkStack.Push(entry);
    }

    /// <summary>
    /// Validates that the expected chunk tag and data size was read.
    /// </summary>
    public void ValidateChunkRead(ChunkTag tag)
    {
        var stack = _chunkStack.Pop();
        if (stack.Tag.Tag != tag.Tag)
        {
            ThrowFailRead();
        }

        var actualRead = _reader.BaseStream.Position - stack.StartPosition;
        if (actualRead != stack.Size)
        {
            ThrowFailRead();
        }

        // if we had a previous position, seek back
        if (stack.PreviousPosition != -1)
        {
            _reader.BaseStream.Seek(stack.PreviousPosition, SeekOrigin.Begin);
        }

        [DoesNotReturn]
        static void ThrowFailRead()
        {
            throw new FileFormatException("Failed to read chunk.");
        }
    }

    public uint ReadTaggedUInt32(ChunkTag tag)
    {
        ReadExpectedTagSimple(tag, out _);
        return _reader.ReadUInt32();
    }

    public int ReadInt32() => _reader.ReadInt32();

    public string ReadUnicodeString()
    {
        var size = _reader.ReadInt32() * 2;
        return _reader.ReadStringOfLength(size, Encoding.Unicode);
    }

    public void Dispose() => _reader.Dispose();

    private record Entry(ChunkTag Tag, int Size, long StartPosition, long PreviousPosition);
}
