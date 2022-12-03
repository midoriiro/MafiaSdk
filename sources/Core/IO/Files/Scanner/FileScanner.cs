using System.Collections.Immutable;

namespace Core.IO.Files.Scanner;

// ReSharper disable once InconsistentNaming
public sealed class FileScanner
{
    public static ImmutableList<TFile> ScanByExtension<TFile>(
        string path, 
        string extension, 
        CancellationToken? cancellationToken = null
    ) where TFile : File
    {
        return Scan(path, $"*.{extension}", false, cancellationToken)
            .Select(fileInfo => (TFile)Activator.CreateInstance(
                typeof(TFile), 
                fileInfo
            )!)
            .ToImmutableList();
    }

    public static string? ScanSingle(
        string path, 
        string pattern, 
        CancellationToken? cancellationToken = null
    )
    {
        try
        {
            return Scan(path, pattern, true, cancellationToken)
                .Single()
                .FullName;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    private static ImmutableList<FileInfo> Scan(
        string path, 
        string pattern, 
        bool stopAtFirstResult,
        CancellationToken? cancellationToken = null
    )
    {
        var root = new DirectoryInfo(path);
        List<FileInfo> files = new ();
        ScanDirectories(root, files, pattern, stopAtFirstResult, cancellationToken);
        return files.ToImmutableList();
    }

    private static bool ScanDirectories(
        DirectoryInfo directoryInfo, 
        List<FileInfo> files, 
        string pattern,
        bool stopAtFirstResult,
        CancellationToken? cancellationToken = null
    )
    {
        if (cancellationToken is not null && cancellationToken.Value.IsCancellationRequested)
        {
            return false;
        }
        
        DirectoryInfo[] directories;

        try
        {
            directories = directoryInfo.GetDirectories();
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }

        // ReSharper disable once InvertIf
        if (directories.Length > 0)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var index = 0; index < directories.Length; index++)
            {
                DirectoryInfo directory = directories[index];
                bool isResultNotEmpty = ScanDirectories(directory, files, pattern, stopAtFirstResult, cancellationToken);

                if (isResultNotEmpty && stopAtFirstResult)
                {
                    return true;
                }
            }
        }

        return ScanFiles(directoryInfo, files, pattern, cancellationToken);
    }

    private static bool ScanFiles(
        DirectoryInfo node, 
        List<FileInfo> files, 
        string pattern, 
        CancellationToken? cancellationToken = null
    )
    {
        if (cancellationToken is not null && cancellationToken.Value.IsCancellationRequested)
        {
            return false;
        }
        
        var scannedFiles = node
            .GetFiles(pattern)
            .ToList();

        if (scannedFiles.Count == 0)
        {
            return false;
        }
        
        files.AddRange(scannedFiles);
        
        return true;
    }
}