using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;

namespace AoMEngineLibrary.Data.Bar;

public static partial class BarFileExtensions
{
    public static void AddEntriesFromDirectory(this BarFile destination, string sourceDirectoryName)
    {
        ArgumentNullException.ThrowIfNull(destination);

        destination.AddEntriesFromDirectory(sourceDirectoryName, false);
    }

    public static void AddEntriesFromDirectory(this BarFile destination, string sourceDirectoryName, bool includeBaseDirectory)
    {
        ArgumentNullException.ThrowIfNull(destination);

        // Rely on Path.GetFullPath for validation of sourceDirectoryName
        sourceDirectoryName = Path.GetFullPath(sourceDirectoryName);

        var archive = destination;

        //add files and directories
        var di = new DirectoryInfo(sourceDirectoryName);

        var basePath = di.FullName;

        if (includeBaseDirectory && di.Parent != null)
            basePath = di.Parent.FullName;

        // Windows' MaxPath (260) is used as an arbitrary default capacity, as it is likely
        // to be greater than the length of typical entry names from the file system, even
        // on non-Windows platforms. The capacity will be increased, if needed.
        const int DefaultCapacity = 260;
        var entryNameBuffer = ArrayPool<char>.Shared.Rent(DefaultCapacity);

        try
        {
            foreach (var file in di.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
            {
                var entryNameLength = file.FullName.Length - basePath.Length;
                Debug.Assert(entryNameLength > 0);

                if (file is FileInfo)
                {
                    // Create entry for file:
                    var entryName = BarFileUtils.EntryFromPath(file.FullName, basePath.Length, entryNameLength, ref entryNameBuffer);
                    archive.CreateEntryFromFile(file.FullName, entryName);
                }
                else
                {
                    // Ignore empty directories (lose knowledge of their existence in archive)
                }
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(entryNameBuffer);
        }
    }

    public static BarEntry CreateEntryFromFile(this BarFile destination, string sourceFileName, string entryName)
    {
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(sourceFileName);
        ArgumentNullException.ThrowIfNull(entryName);

        // Argument checking gets passed down to FileStream's ctor and CreateEntry
        using (var fs = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 0x1000, useAsync: false))
        {
            var lastWrite = File.GetLastWriteTime(sourceFileName);

            var entry = destination.CreateEntry(entryName);
            using (var ms = new MemoryStream())
            {
                fs.CopyTo(ms);
                entry.SetData(ms.ToArray(), lastWrite);
            }

            return entry;
        }
    }
}
