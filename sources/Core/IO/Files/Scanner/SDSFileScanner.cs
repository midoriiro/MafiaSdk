using System.Collections.Immutable;

namespace Core.IO.Files.Scanner;

// ReSharper disable once InconsistentNaming
public sealed class SDSFileScanner
{
    public static ImmutableList<SDSFile> Scan(string path)
    {
        var root = new DirectoryInfo(path);
        List<SDSFile> files = new ();
        ScanDirectories(root, files);
        return files.ToImmutableList();
    }

    private static void ScanDirectories(DirectoryInfo directoryInfo, List<SDSFile> files)
    {
        var directories = directoryInfo.GetDirectories();

        if (directories.Length > 0)
        {
            foreach (DirectoryInfo directory in directories)
            {
                ScanDirectories(directory, files);
            }
        }

        ScanFiles(directoryInfo, files);
    }

    private static void ScanFiles(DirectoryInfo node, List<SDSFile> files)
    {
        var directory = new Lazy<Directory>(() => new Directory(node));

        var scannedFiles = node
            .GetFiles("*.sds")
            .Select(fileInfo => new SDSFile(fileInfo, directory.Value))
            .ToList();

        if (scannedFiles.Count == 0)
        {
            return;
        }
        
        files.AddRange(scannedFiles);
    }
}