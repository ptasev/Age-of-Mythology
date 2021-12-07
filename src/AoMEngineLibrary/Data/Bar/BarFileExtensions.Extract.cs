using System;
using System.IO;
using System.Linq;

namespace AoMEngineLibrary.Data.Bar;

public static partial class BarFileExtensions
{
    public static void ExtractToDirectory(this BarFile source, string destinationDirectoryName, bool overwriteFiles)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destinationDirectoryName);

        foreach (BarEntry entry in source.Entries)
        {
            entry.ExtractRelativeToDirectory(destinationDirectoryName, overwriteFiles);
        }
    }

    internal static void ExtractRelativeToDirectory(this BarEntry source, string destinationDirectoryName, bool overwrite)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destinationDirectoryName);

        // Note that this will give us a good DirectoryInfo even if destinationDirectoryName exists:
        DirectoryInfo di = Directory.CreateDirectory(destinationDirectoryName);
        string destinationDirectoryFullPath = di.FullName;
        if (!destinationDirectoryFullPath.EndsWith(Path.DirectorySeparatorChar))
            destinationDirectoryFullPath += Path.DirectorySeparatorChar;

        string fileDestinationPath = Path.GetFullPath(Path.Combine(destinationDirectoryFullPath, source.FilePath));

        if (!fileDestinationPath.StartsWith(destinationDirectoryFullPath, PathInternal.StringComparison))
            throw new IOException("Extracting Zip entry would have resulted in a file outside the specified destination directory.");

        // If it is a file:
        // Create containing directory:
        Directory.CreateDirectory(Path.GetDirectoryName(fileDestinationPath)!);
        source.ExtractToFile(fileDestinationPath, overwrite: overwrite);
    }

    public static void ExtractToFile(this BarEntry source, string destinationFileName, bool overwrite)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destinationFileName);

        // Rely on FileStream's ctor for further checking destinationFileName parameter
        var fMode = overwrite ? FileMode.Create : FileMode.CreateNew;

        using (var fs = new FileStream(destinationFileName, fMode, FileAccess.Write, FileShare.None, bufferSize: 0x1000, useAsync: false))
        {
            var data = source.GetData();
            fs.Write(data.ToArray(), 0, data.Count);
        }

        try
        {
            File.SetLastWriteTime(destinationFileName, source.Modified);
        }
        catch (UnauthorizedAccessException)
        {
            // some OSes like Android (#35374) might not support setting the last write time, the extraction should not fail because of that
        }
    }
}

/// <summary>Contains internal path helpers that are shared between many projects.</summary>
internal static partial class PathInternal
{
    /// <summary>Returns a comparison that can be used to compare file and directory names for equality.</summary>
    internal static StringComparison StringComparison
    {
        get
        {
            return IsCaseSensitive ?
                StringComparison.Ordinal :
                StringComparison.OrdinalIgnoreCase;
        }
    }

    /// <summary>Gets whether the system is case-sensitive.</summary>
    internal static bool IsCaseSensitive
    {
        get
        {
            return !(OperatingSystem.IsWindows() || OperatingSystem.IsMacOS() || OperatingSystem.IsIOS() || OperatingSystem.IsTvOS() || OperatingSystem.IsWatchOS());
        }
    }
}
