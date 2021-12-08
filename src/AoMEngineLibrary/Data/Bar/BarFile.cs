using AoMEngineLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace AoMEngineLibrary.Data.Bar;

public class BarFile : IDisposable
{
    private bool _disposedValue;
    private bool _leaveOpen;
    private readonly SortedList<string, BarEntry> _entries;

    internal Stream? ArchiveStream { get; private set; }

    /// <summary>
    /// Whether the entire file is loaded into memory.
    /// </summary>
    public bool IsLoadedInMemory { get; private set; }

    public int Unknown { get; private set; }

    public int Version { get; private set; }

    public IReadOnlyCollection<BarEntry> Entries
    {
        get
        {
            ThrowIfDisposed();

            return new ReadOnlyCollection<BarEntry>(_entries.Values);
        }
    }

    public BarFile()
    {
        ArchiveStream = null;
        _leaveOpen = false;
        IsLoadedInMemory = true;

        // Not sure if this is really version, but I've only ever seen it be 8
        Version = 8;
        _entries = new SortedList<string, BarEntry>();
    }

    public BarEntry CreateEntry(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("String cannot be empty.", nameof(filePath));

        ThrowIfDisposed();

        var entry = new BarEntry(this, filePath);
        AddEntry(entry);
        return entry;
    }

    public BarEntry? GetEntry(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        ThrowIfDisposed();

        if (!_entries.TryGetValue(filePath, out var entry))
        {
            return null;
        }

        return entry;
    }

    private void AddEntry(BarEntry entry)
    {
        if (!_entries.TryAdd(entry.FilePath, entry))
        {
            throw new ArgumentException("An entry with the file path already exists.", "filePath");
        }
    }

    internal void RemoveEntry(BarEntry entry)
    {
        _entries.Remove(entry.FilePath);
        IsLoadedInMemory = IsLoadedInMemory || _entries.Values.All(x => x.IsLoadedInMemory);

        // Stream is no longer needed
        if (IsLoadedInMemory)
            CloseStream();
    }

    public void DeleteAll()
    {
        foreach (var entry in _entries.Values)
        {
            entry.Delete();
        }

        IsLoadedInMemory = true;

        // Stream is no longer needed
        CloseStream();
    }

    /// <summary>
    /// Loads all entry data into memory.
    /// </summary>
    public void LoadIntoMemory()
    {
        foreach (var entry in _entries.Values)
        {
            entry.LoadDataInMemory();
        }

        IsLoadedInMemory = true;

        // Stream is no longer needed
        CloseStream();
    }

    /// <summary>
    /// Opens the file from the given stream, and hold onto it for lazy loading of entry data.
    /// The <see cref="BarFile"/> obtains ownership of the stream, unless left open.
    /// </summary>
    public static BarFile Open(Stream stream, bool leaveOpen)
    {
        var file = new BarFile();
        file.Read(stream, leaveOpen);
        return file;
    }

    /// <summary>
    /// Reads the file from the given stream, and hold onto it for lazy loading of entry data.
    /// The <see cref="BarFile"/> obtains ownership of the stream, unless left open.
    /// </summary>
    private void Read(Stream stream, bool leaveOpen)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ThrowIfDisposed();

        DeleteAll();
        IsLoadedInMemory = false;
        ArchiveStream = stream;
        _leaveOpen = leaveOpen;

        using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
        {
            // Ignore first 8 bytes (appear to be all 0s)
            _ = reader.ReadBytes(8);

            Unknown = reader.ReadInt32(); // checksum??
            var entryCount = reader.ReadInt32();
            _ = reader.ReadInt32(); // entry headers length
            var entryOffsetsOffset = reader.ReadInt32();
            Version = reader.ReadInt32(); // not even sure this is version

            reader.BaseStream.Seek(entryOffsetsOffset, SeekOrigin.Begin);
            var entryOffsets = new int[entryCount];
            for (var i = 0; i < entryCount; ++i)
            {
                entryOffsets[i] = reader.ReadInt32();
            }
            var entryHeadersOffset = reader.BaseStream.Position;

            _entries.Capacity = entryCount;
            for (var i = 0; i < entryCount; ++i)
            {
                reader.BaseStream.Seek(entryHeadersOffset + entryOffsets[i], SeekOrigin.Begin);
                var entry = ReadEntry(reader, this);
                AddEntry(entry);
            }
        }

        static BarEntry ReadEntry(BinaryReader reader, BarFile parent)
        {
            var offset = reader.ReadInt32();
            var size = reader.ReadInt32();
            var size2 = reader.ReadInt32();

            if (size != size2)
            {
                // these two sizes are always expected to match
                // in Alpha bar files size2 doesn't exist
                // we need to back up so we can read date properly
                reader.BaseStream.Seek(-4, SeekOrigin.Current);

                // patch-up size2
                size2 = size;
            }

            var year = reader.ReadUInt16();
            var month = reader.ReadByte();
            var day = reader.ReadByte();
            if (day == 0)
            {
                // in the case the this is an Alpha file and sizes match by chance
                // we can also make sure that day is not 0 as it never should be
                // back up and re-read
                reader.BaseStream.Seek(-8, SeekOrigin.Current);
                year = reader.ReadUInt16();
                month = reader.ReadByte();
                day = reader.ReadByte();
            }
            var hour = reader.ReadByte();
            var minute = reader.ReadByte();
            var second = reader.ReadByte();
            var millisecond = reader.ReadByte();
            var modified = new DateTime(year, month, day, hour, minute, second, millisecond);

            var filePath = reader.ReadNullTerminatedString();
            return new BarEntry(parent, filePath, offset, size, size2, modified);
        }
    }

    /// <summary>
    /// Writes the file to the given stream, and hold onto it for lazy loading of entry data.
    /// The <see cref="BarFile"/> obtains ownership of the stream, unless left open.
    /// </summary>
    public void Write(Stream stream, bool leaveOpen)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (ReferenceEquals(ArchiveStream, stream))
        {
            throw new InvalidOperationException("Cannot write to the same stream that was opened");
        }
        ThrowIfDisposed();

        using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
        {
            ReadOnlySpan<byte> emptyBytes = stackalloc byte[8];
            writer.Write(emptyBytes);
            writer.Write(Unknown);
            writer.Write(_entries.Count);
            writer.Write(0); // entry headers length will fill in after
            writer.Write(0); // entry offsets offset will fill in after
            writer.Write(Version);

            // Now write the data. Save offsets and apply after
            // we don't want to store them into entries now in case of failure
            // this would corrupt the existing data if not loaded into memory
            var dataOffsets = new SortedList<string, int>(_entries.Count);
            foreach (var entry in _entries.Values)
            {
                dataOffsets.Add(entry.FilePath, Convert.ToInt32(writer.BaseStream.Position));
                var data = entry.GetData().ToArray();
                writer.Write(data);
            }

            // Write the entry offsets
            var entryOffsetsOffset = Convert.ToInt32(writer.BaseStream.Position);
            const int entryStaticLength = 21; // extra 1 for null terminating char
            var currentEntryOffset = 0;
            foreach (var entry in _entries.Values)
            {
                writer.Write(currentEntryOffset);
                var entryOffset = entryStaticLength + Encoding.UTF8.GetByteCount(entry.FilePath);
                currentEntryOffset += entryOffset;
            }

            // Write the entry headers
            var entryHeadersOffset = writer.BaseStream.Position;
            foreach (var entry in _entries.Values)
            {
                var dataOffset = dataOffsets[entry.FilePath];
                WriteEntryHeader(writer, entry, dataOffset);
            }

            // Go back and fill in file header
            var endOfFile = writer.BaseStream.Position;
            var entryHeadersLength = Convert.ToInt32(endOfFile - entryHeadersOffset);
            writer.BaseStream.Seek(16, SeekOrigin.Begin);
            writer.Write(entryHeadersLength);
            writer.Write(entryOffsetsOffset);
            writer.BaseStream.Seek(endOfFile, SeekOrigin.Begin);

            // Writing finished, commit data offsets to entries
            foreach (var entry in _entries.Values)
            {
                var dataOffset = dataOffsets[entry.FilePath];
                entry.Offset = dataOffset;
            }
        }

        // We no longer old stream, switch to new stream
        CloseStream();
        ArchiveStream = stream;
        _leaveOpen = leaveOpen;

        if (IsLoadedInMemory)
        {
            // if the data is already loaded into memory close new stream too
            CloseStream();
        }

        static void WriteEntryHeader(BinaryWriter writer, BarEntry entry, int dataOffset)
        {
            writer.Write(dataOffset);
            writer.Write(entry.Size);
            writer.Write(entry.Size2);

            var modified = entry.Modified;
            writer.Write((ushort)modified.Year);
            writer.Write((byte)modified.Month);
            writer.Write((byte)modified.Day);
            writer.Write((byte)modified.Hour);
            writer.Write((byte)modified.Minute);
            writer.Write((byte)modified.Second);
            writer.Write((byte)0); // it appears millisecond is always 0

            writer.WriteNullTerminatedString(entry.FilePath);
        }
    }

    private void CloseStream()
    {
        if (!_leaveOpen)
        {
            ArchiveStream?.Dispose();
            ArchiveStream = null;
        }
    }

    #region IDisposable
    internal void ThrowIfDisposed()
    {
        if (_disposedValue)
            throw new ObjectDisposedException(nameof(BarFile));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
                CloseStream();
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
