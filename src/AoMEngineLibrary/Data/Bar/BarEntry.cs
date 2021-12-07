using AoMEngineLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace AoMEngineLibrary.Data.Bar;

public class BarEntry
{
    private IReadOnlyList<byte> _data;

    /// <summary>
    /// The archive the entry belongs to, or null if entry is deleted.
    /// </summary>
    public BarFile? Archive { get; private set; }

    /// <summary>
    /// Whether the entry's data is loaded in memory.
    /// </summary>
    public bool IsLoadedInMemory { get; private set; }

    public int Offset { get; internal set; }

    public int Size { get; private set; }

    internal int Size2 { get; private set; }

    public DateTime Modified { get; private set; }

    public string FilePath { get; }

    internal BarEntry(BarFile parent, string filePath, IReadOnlyList<byte> data)
        : this(parent, filePath, data, DateTime.Now)
    {
    }

    internal BarEntry(BarFile parent, string filePath, IReadOnlyList<byte> data, DateTime modified)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(filePath);

        Archive = parent;
        _data = data;
        IsLoadedInMemory = true;

        Offset = 0;
        Size = data.Count;
        Size2 = data.Count;
        Modified = modified;
        FilePath = filePath;
    }

    internal BarEntry(BarFile parent, string filePath, int offset, int size, int size2, DateTime modified)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(filePath);

        Archive = parent;
        _data = Array.Empty<byte>();
        IsLoadedInMemory = false;

        Offset = offset;
        Size = size;
        Size2 = size2;
        Modified = modified;
        FilePath = filePath;
    }

    public void Delete()
    {
        // entry already deleted
        if (Archive is null)
            return;

        Archive.ThrowIfDisposed();

        Archive.RemoveEntry(this);
        Archive = null;
    }

    public void SetData(IReadOnlyList<byte> data)
    {
        ThrowIfInvalidArchive();
        SetData(data, DateTime.Now);
    }

    public void SetData(IReadOnlyList<byte> data, DateTime modified)
    {
        ThrowIfInvalidArchive();

        _data = data;
        IsLoadedInMemory = true;

        Size = data.Count;
        Size2 = data.Count;
        Modified = modified;
    }

    public IReadOnlyList<byte> GetData()
    {
        ThrowIfInvalidArchive();

        if (IsLoadedInMemory)
        {
            return _data;
        }
        else
        {
            return GetDataFromStream();
        }
    }

    /// <summary>
    /// Loads and keeps entry data in memory.
    /// </summary>
    public void LoadDataInMemory()
    {
        // already loaded in memory
        if (IsLoadedInMemory)
            return;

        ThrowIfInvalidArchive();

        _data = GetDataFromStream();
        IsLoadedInMemory = true;
    }

    private IReadOnlyList<byte> GetDataFromStream()
    {
        var stream = Archive?.ArchiveStream;
        if (stream is null)
        {
            return _data;
        }

        var data = new byte[Size];
        stream.Seek(Offset, SeekOrigin.Begin);
        stream.ReadExactly(data);
        return data;
    }

    private void ThrowIfInvalidArchive()
    {
        if (Archive is null)
            throw new InvalidOperationException("Cannot perform operation on deleted entry.");
        Archive.ThrowIfDisposed();
    }
}
