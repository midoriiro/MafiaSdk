using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.Cities.Mafia3;

public class TestScenes : ICity
{
    public string Name { get; init; } = null!;
    public string SdsPrefix { get; init; } = null!;

    private TestScenes()
    {
    }

    public static ICity ReadFromFile(BinaryReader reader)
    {
        string name = reader.ReadStringEncoded();
        string sdsPrefix = reader.ReadStringEncoded();

        return new TestScenes()
        {
            Name = name,
            SdsPrefix = sdsPrefix
        };
    }

    public void WriteToFile(XBinWriter writer)
    {
        writer.WriteUnicodeNullTerminatedString(Name);
        writer.WriteUnicodeNullTerminatedString(SdsPrefix);
    }
}