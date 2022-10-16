using System.Collections.Immutable;

namespace Core.IO.Files;

public sealed class Directory
{
    private DirectoryInfo DirectoryInfo { get; }
    private readonly List<File> _files;

    public Directory(DirectoryInfo directoryInfo)
    {
        DirectoryInfo = directoryInfo;
        _files = new List<File>();
    }

    public void AddFile(File file)
    {
        _files.Add(file);
    }
     
    public ImmutableList<T> GetFiles<T>()
    {
        return _files
            .OfType<T>()
            .ToImmutableList();
    }

    public void Delete()
    {
        if(DirectoryInfo.Exists)
        {
            DirectoryInfo.Delete(true);
        }
    }
}