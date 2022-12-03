using System.Globalization;
using Core.IO.Files.Extensions;

namespace Core.IO.Files;

public abstract class File
{
    public FileInfo FileInfo { get; }
    public Directory ParentDirectory { get; }
    public string Extension { get; }
    protected abstract string DefinedExtension { get; }
    
    protected File(FileInfo fileInfo)
    {
        FileInfo = fileInfo;
        ParentDirectory = new Directory(FileInfo.Directory!);
        ParentDirectory.AddFile(this);
        Extension = fileInfo.Extension.Replace(".", "");


        // ReSharper disable once VirtualMemberCallInConstructor
        if (Extension != DefinedExtension)
        {
            throw new InvalidOperationException(
                // ReSharper disable once VirtualMemberCallInConstructor
                $"Extension '{Extension}' is different from defined extension '{DefinedExtension}'"
            );
        }
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