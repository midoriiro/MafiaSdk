using Core.Games;
using Core.IO.FileFormats.SDS;
using Core.IO.FileFormats.SDS.Resource;

namespace Core.IO.Files;

// ReSharper disable once InconsistentNaming
public class SDSFile : File
{
    public SDSFile(FileInfo fileInfo, Directory parentDirectory) : base(fileInfo, parentDirectory)
    {
    }

    public override bool Open()
    {
        string filePath = FileInfo.FullName;
        string pathToBackup = GameWorkSpace.Instance().GetBackupPath(filePath);
        string pathToExtract = GameWorkSpace.Instance().GetExtractPath(filePath);

        // We should backup file before unpacking..
        System.IO.Directory.CreateDirectory(Path.GetDirectoryName(pathToBackup)!);

        // Place the backup in the folder recently created
        System.IO.File.Copy(FileInfo.FullName, pathToBackup, true);

        // Begin the unpacking process.
        ArchiveFile archiveFile;
        
        using (FileStream fileStream = System.IO.File.OpenRead(filePath))
        {
            using (Stream? archiveStream = ArchiveEncryption.Unwrap(fileStream))
            {
                archiveFile = new ArchiveFile();
                archiveFile.Deserialize(archiveStream ?? fileStream);
            }
        }
        
        Resource.SaveResources(
            new FileInfo(pathToExtract), 
            archiveFile, 
            GameWorkSpace.Instance().SelectedGame.Type
        );

        // Unpacking patch file
        string patchFilePath = filePath + ".patch";
        bool IsPatchNeeded = GameWorkSpace.Instance().SelectedGame.Type == GamesEnumerator.Mafia2DefinitiveEdition;
        
        // if (System.IO.File.Exists(patchFilename) && IsPatchNeeded)
        // {
        //     archiveFile.ExtractPatch(new FileInfo(patchFilename));
        // }

        return true;
    }

    public override void Save()
    {
        throw new NotImplementedException();
    }
}