using System.Collections.Immutable;

namespace Core.IO.Files.Scanner;

// ReSharper disable once InconsistentNaming
public sealed class SDSFileScanner
{
    public static ImmutableList<SDSFile> Scan(string path)
    {
        var root = new DirectoryInfo(path);
        return ScanDirectories(root).ToImmutableList();
    }

    private static IEnumerable<SDSFile> ScanDirectories(DirectoryInfo directoryInfo)
    {
        return directoryInfo
            .GetDirectories()
            .SelectMany(ScanFiles)
            .ToList()
            .Concat(ScanFiles(directoryInfo));
    }

    private static IEnumerable<SDSFile> ScanFiles(DirectoryInfo node)
    {
        var directory = new Lazy<Directory>(() => new Directory(node));

        return node
            .GetFiles("*.sds")
            .Select(fileInfo => new SDSFile(fileInfo, directory.Value));
    }
}