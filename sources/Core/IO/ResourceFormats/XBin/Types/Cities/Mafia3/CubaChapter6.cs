using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.Cities.Mafia3;

public class CubaChapter6 : ICity
{
    public string Name { get; init; } = null!;
    public string MissionLine { get; init; } = null!;
    public string SdsPrefix { get; init; } = null!;
    public string Map { get; init; } = null!;
    public string RespawnLocation1 { get; init; } = null!;

    private CubaChapter6()
    {
    }

    public static ICity ReadFromFile(BinaryReader reader)
    {
        string name = reader.ReadStringEncoded();
        string missionLine = reader.ReadStringEncoded();
        string sdsPrefix = reader.ReadStringEncoded();
        string map = reader.ReadStringEncoded();
        string respawnLocation1 = reader.ReadStringEncoded();

        return new CubaChapter6()
        {
            Name = name,
            MissionLine = missionLine,
            SdsPrefix = sdsPrefix,
            Map = map,
            RespawnLocation1 = respawnLocation1
        };
    }

    public void WriteToFile(XBinWriter writer)
    {
        writer.WriteUnicodeNullTerminatedString(Name);
        writer.WriteUnicodeNullTerminatedString(MissionLine);
        writer.WriteUnicodeNullTerminatedString(SdsPrefix);
        writer.WriteUnicodeNullTerminatedString(Map);
        writer.WriteUnicodeNullTerminatedString(RespawnLocation1);
    }
}