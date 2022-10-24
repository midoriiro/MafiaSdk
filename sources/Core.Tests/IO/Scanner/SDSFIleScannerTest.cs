using Core.IO.Files.Scanner;

namespace Core.Tests.IO.Scanner;

public class SDSFIleScannerTest
{
    [Fact]
    public void TestScan()
    {
        var scanner = new SDSFileScanner();
        var files = SDSFileScanner.Scan(@"N:\games\steam\steamapps\common\Mafia III\sds_retail");
    }
}