using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.Cities.Mafia3;

public class SouthIslands : ICity
{
    public string Name { get; init; } = null!;
    public string MissionLine { get; init; } = null!;
    public string SdsPrefix { get; init; } = null!;
    public string Map { get; init; } = null!;

    private SouthIslands()
    {
    }

    public static ICity ReadFromFile(BinaryReader reader)
    {
        string name = reader.ReadStringEncoded();
        string missionLine = reader.ReadStringEncoded();
        string sdsPrefix = reader.ReadStringEncoded();
        string map = reader.ReadStringEncoded();

        return new SouthIslands()
        {
            Name = name,
            MissionLine = missionLine,
            SdsPrefix = sdsPrefix,
            Map = map
        };
    }

    public void WriteToFile(XBinWriter writer)
    {
        writer.WriteUnicodeNullTerminatedString(Name);
        writer.WriteUnicodeNullTerminatedString(MissionLine);
        writer.WriteUnicodeNullTerminatedString(SdsPrefix);
        writer.WriteUnicodeNullTerminatedString(Map);
    }
}