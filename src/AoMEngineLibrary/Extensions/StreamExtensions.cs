using System;
using System.IO;
using System.IO.Compression;

namespace AoMEngineLibrary.Extensions;

public static class StreamExtensions
{
    // spells l33t in ASCII
    private static ReadOnlySpan<byte> CompressedTag => new byte[] {0x6C, 0x33, 0x33, 0x74};

    public static void ReadExactly(this Stream stream, Span<byte> buffer)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        var totalRead = 0;
        var count = buffer.Length;
        while (totalRead < count)
        {
            var readCount = stream.Read(buffer[totalRead..]);
            if (readCount == 0)
                throw new EndOfStreamException("The end of the stream is reached.");
            totalRead += readCount;
        }
    }

    public static Stream DecompressIfNecessary(this Stream input, out bool createdNewStream)
    {
        Span<byte> tag = stackalloc byte[4];
        input.ReadExactly(tag);

        if (tag.SequenceEqual(CompressedTag))
        {
            // This is a compressed file, decompress first
            createdNewStream = true;

            // Next 4 bytes is original file length
            input.ReadExactly(tag);
            var originalFileLength = BitConverter.ToInt32(tag);

            var output = new MemoryStream(originalFileLength);
            using (var ds = new ZLibStream(input, CompressionMode.Decompress, true))
            {
                ds.CopyTo(output);
            }

            output.Seek(0, SeekOrigin.Begin);
            return output;
        }
        else
        {
            input.Seek(-tag.Length, SeekOrigin.Current);
            createdNewStream = false;
            return input;
        }
    }

    /// <summary>
    /// Decompresses the input stream if necessary into the output stream.
    /// </summary>
    /// <param name="input">The input stream which is either compressed or not. Must be seekable and readable.</param>
    /// <param name="output">The output stream into which to copy the input. Must be writeable.</param>
    /// <returns>the number of bytes that were copied.</returns>
    public static long DecompressTo(this Stream input, Stream output)
    {
        Span<byte> tag = stackalloc byte[4];
        input.ReadExactly(tag);

        if (tag.SequenceEqual(CompressedTag))
        {
            // Next 4 bytes is original file length
            input.ReadExactly(tag);
            var originalFileLength = BitConverter.ToInt32(tag);

            // MemoryStream optimization
            if (output is MemoryStream ms)
            {
                ms.Capacity = originalFileLength;
            }

            using var ds = new ZLibStream(input, CompressionMode.Decompress, true);
            var startPosition = output.Position;
            ds.CopyTo(output);
            return output.Position - startPosition;
        }
        else
        {
            input.Seek(-tag.Length, SeekOrigin.Current);
            var startPosition = output.Position;
            input.CopyTo(output);
            return output.Position - startPosition;
        }
    }

    /// <summary>
    /// Compresses the input stream into the output stream.
    /// </summary>
    /// <param name="input">The input stream. Must be seekable and readable.</param>
    /// <param name="output">The output stream into which to compress the input. Must be writeable.</param>
    /// <param name="compressionLevel">The compression level.</param>
    /// <returns>the number of bytes that were copied.</returns>
    public static long CompressTo(this Stream input, Stream output, CompressionLevel compressionLevel)
    {
        var startPosition = output.Position;
        output.Write(CompressedTag);

        Span<byte> sizeBytes = stackalloc byte[sizeof(int)];
        var fileSize = Convert.ToInt32(input.Length - input.Position);
        BitConverter.TryWriteBytes(sizeBytes, fileSize);
        output.Write(sizeBytes);

        using (var zs = new ZLibStream(output, compressionLevel, true))
        {
            input.CopyTo(zs);
        }

        return output.Position - startPosition;
    }
}
