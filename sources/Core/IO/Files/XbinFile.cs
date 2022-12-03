using Core.Games;
using Core.IO.ResourceFormats.XBin;

namespace Core.IO.Files;

// ReSharper disable once InconsistentNaming
public class XbinFile : File
{
    protected override string DefinedExtension => "xbin";
    public XBin XBin { get; private set; } = null!;
    
    public XbinFile(FileInfo fileInfo) : base(fileInfo)
    {
    }

    public override bool Open()
    {
        string filePath = FileInfo.FullName;
        string pathToBackup = GameWorkSpace.Instance().GetBackupPath(filePath);
        string pathToExtract = GameWorkSpace.Instance().GetExtractPath(filePath);

        using var reader = new BinaryReader(System.IO.File.Open(filePath, FileMode.Open));
        XBin = XBin.ReadFromFile(reader);

        return true;
    }

    public override void Save()
    {
        throw new NotImplementedException();
    }
}