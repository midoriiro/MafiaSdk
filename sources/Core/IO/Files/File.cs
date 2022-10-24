using System.Globalization;
using Core.IO.Files.Extensions;

namespace Core.IO.Files;

public abstract class File
{
    public FileInfo FileInfo { get; }
    public Directory ParentDirectory { get; }
    public string Extension { get; }

    protected File(FileInfo fileInfo, Directory parentDirectory)
    {
        FileInfo = fileInfo;
        ParentDirectory = parentDirectory;
        ParentDirectory.AddFile(this);
        Extension = fileInfo.Extension.Replace(".", "").ToUpper();
    }

    public string GetFileSizeAsString()
    {
        return FileInfo.CalculateFileSize();
    }

    public string GetLastTimeWrite()
    {
        return FileInfo.LastWriteTime.ToString(CultureInfo.InvariantCulture);
    }

    public string GetName()
    {
        return FileInfo.Name;
    }

    public string GetNameWithoutExtension()
    {
        int size = Extension.Length + 1;
        return FileInfo.Name.Remove(FileInfo.Name.Length - size, size);
    }

    public abstract bool Open();

    public abstract void Save();

    public void Delete()
    {
        if(FileInfo.Exists)
        {
            FileInfo.Delete();
        }
    }
}