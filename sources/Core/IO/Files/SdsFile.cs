using Core.Games;
using Core.IO.FileFormats.SDS;

namespace Core.IO.Files;

// ReSharper disable once InconsistentNaming
public class SdsFile : File
{
    protected override string DefinedExtension => "sds";

    public SdsFile(FileInfo fileInfo) : base(fileInfo)
    {
    }

    public override bool Open()
    {
        string filePath = FileInfo.FullName;
        string pathToBackup = GameWorkSpace.Instance().GetBackupPath(filePath);
        string pathToExtract = GameWorkSpace.Instance().GetExtractPath(filePath);
        
        System.IO.Directory.CreateDirectory(Path.GetDirectoryName(pathToBackup)!);
        
        ArchiveFile archiveFile;
        
        using (FileStream stream = System.IO.File.OpenRead(filePath))
        {
            archiveFile = ArchiveFile.Deserialize(stream);
        }
        
        // Resource.DeserializeResources(
        //     dataWri, 
        //     archiveFile, 
        //     GameWorkSpace.Instance().SelectedGame.Type
        // );

        // Unpacking patch file
        string patchFilePath = filePath + ".patch";
        bool IsPatchNeeded = GameWorkSpace.Instance().SelectedGame.Type == GamesEnumerator.Mafia2DefinitiveEdition;
        
        if (System.IO.File.Exists(patchFilePath) && IsPatchNeeded)
        {
            // Resource.DeserializePatchResources(new FileInfo(patchFilePath), archiveFile);
        }

        return true;
    }

    public override void Save()
    {
        throw new NotImplementedException();
    }
}