using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.Cities.Mafia3;

public class Saigon : ICity
{
    public string Name { get; init; } = null!;
    public string MissionLine { get; init; } = null!;
    public string SdsPrefix { get; init; } = null!;

    private Saigon()
    {
    }

    public static ICity ReadFromFile(BinaryReader reader)
    {
        string name = reader.ReadStringEncoded();
        string missionLine = reader.ReadStringEncoded();
        string sdsPrefix = reader.ReadStringEncoded();

        return new Saigon()
        {
            Name = name,
            MissionLine = missionLine,
            SdsPrefix = sdsPrefix
        };
    }

    public void WriteToFile(XBinWriter writer)
    {
        writer.WriteUnicodeNullTerminatedString(Name);
        writer.WriteUnicodeNullTerminatedString(MissionLine);
        writer.WriteUnicodeNullTerminatedString(SdsPrefix);
    }
}