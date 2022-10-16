using Core.IO.FileFormats.SDS;

namespace Core.Tests.FileFormats.Mafia3;

public class ArchiveFileTest
{
    [Fact]
    public void DecompressScripts()
    {
        var archiveFile = new ArchiveFile();
        using (var input = File.OpenRead(@"N:\games\steam\steamapps\common\Mafia III\sds_retail\tables\scripts.sds"))
        {
            archiveFile.Deserialize(input);
        }

        using (var output = File.Create(@"N:\test.sds"))
        {
            archiveFile.Serialize(output, ArchiveSerializeOptions.OneBlock);
        }
    }
    
    [Fact]
    public void DecompressCity()
    {
        var archiveFile = new ArchiveFile();
        using (var input = File.OpenRead(@"N:\games\steam\steamapps\common\Mafia III\sds_retail\city\bourbon_tex_global.sds"))
        {
            archiveFile.Deserialize(input);
        }

        using (var output = File.Create(@"N:\test.sds"))
        {
            archiveFile.Serialize(output, ArchiveSerializeOptions.OneBlock);
        }
    }
}