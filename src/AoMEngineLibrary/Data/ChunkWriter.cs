using AoMEngineLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace AoMEngineLibrary.Data;

public sealed class ChunkWriter : IDisposable
{
    private readonly BinaryWriter _writer;
    private readonly Stack<Entry> _chunkStack;

    public ChunkWriter(BinaryWriter writer)
    {
        _writer = writer;
        _chunkStack = new Stack<Entry>();
    }

    public void WriteTag(ChunkTag tag, int size)
    {
        _writer.Write(tag.Tag);
        _writer.Write(size);
    }

    public void WriteTagPostSized(ChunkTag tag, out long sizePosition)
    {
        _writer.Write(tag.Tag);

        sizePosition = _writer.BaseStream.Position;
        _writer.Write(0);

        var entry = new Entry(tag, sizePosition);
        _chunkStack.Push(entry);
    }

    public void WriteSize(ChunkTag tag, long sizePosition)
    {
        var stack = _chunkStack.Pop();
        if (stack.Tag.Tag != tag.Tag)
        {
            ThrowFailWrite();
        }

        if (stack.SizePosition != sizePosition)
        {
            ThrowFailWrite();
        }

        var endPosition = _writer.BaseStream.Position;
        var size = Convert.ToInt32(endPosition - sizePosition - sizeof(int));

        _writer.BaseStream.Seek(sizePosition, SeekOrigin.Begin);
        _writer.Write(size);
        _writer.BaseStream.Seek(endPosition, SeekOrigin.Begin);

        [DoesNotReturn]
        static void ThrowFailWrite()
        {
            throw new FileFormatException("Failed to write chunk.");
        }
    }

    public void WriteTagged(ChunkTag tag, uint data)
    {
        WriteTag(tag, sizeof(uint));
        _writer.Write(data);
    }

    public void Write(int data) => _writer.Write(data);

    public void WriteUnicodeString(string data)
    {
        var bytes = Encoding.Unicode.GetBytes(data);
        var size = bytes.Length / 2;
        _writer.Write(size);
        _writer.Write(bytes);
    }

    public void Dispose() => _writer.Dispose();

    private record Entry(ChunkTag Tag, long SizePosition);
}
