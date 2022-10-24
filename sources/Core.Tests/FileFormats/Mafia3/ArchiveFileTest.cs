using Core.Games;
using Core.IO.FileFormats.SDS;
using Core.IO.Files;
using Core.IO.Files.Scanner;
using Directory = Core.IO.Files.Directory;
using File = System.IO.File;

namespace Core.Tests.FileFormats.Mafia3;

public class ArchiveFileTest
{
    [Fact]
    public void DecompressScripts()
    {
        var workspace = GameWorkSpace.Create(
            GamesEnumerator.Mafia3,
            @"N:\games\steam\steamapps\common\Mafia III",
            @"D:\mafia-sdk-workspaces"
        );

        var files = SDSFileScanner.Scan(workspace.SelectedGame.InstallationPath);

        foreach (SDSFile file in files)
        {
            file.Open();
        }
    }
    
    [Fact]
    public void DebugGenericEntry()
    {
        var workspace = GameWorkSpace.Create(
            GamesEnumerator.Mafia3,
            @"N:\games\steam\steamapps\common\Mafia III",
            @"D:\mafia-sdk-workspaces"
        );

        var info = new FileInfo(
            @"N:\games\steam\steamapps\common\Mafia III\dlc\598022\content500\sds\city\bc_loc_sammys_club_base_dlc3_collisions.sds");

        var file = new SDSFile(
            info,
            new Directory(info.Directory!)    
        );

        file.Open();
    }
}