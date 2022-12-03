using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.Cities.Mafia3;

public class BourbonCity : ICity
{
    public string Name { get; init; } = null!;
    public string MissionLine { get; init; } = null!;
    public string SdsPrefix { get; init; } = null!;
    public string Map { get; init; } = null!;
    public string RespawnLocation1 { get; init; } = null!;
    public string RespawnLocation2 { get; init; } = null!;
    public string RespawnLocation3 { get; init; } = null!;

    private BourbonCity()
    {
    }

    public static ICity ReadFromFile(BinaryReader reader)
    {
        string name = reader.ReadStringEncoded();
        string missionLine = reader.ReadStringEncoded();
        string sdsPrefix = reader.ReadStringEncoded();
        string map = reader.ReadStringEncoded();
        string respawnLocation1 = reader.ReadStringEncoded();
        string respawnLocation2 = reader.ReadStringEncoded();
        string respawnLocation3 = reader.ReadStringEncoded();

        return new BourbonCity()
        {
            Name = name,
            MissionLine = missionLine,
            SdsPrefix = sdsPrefix,
            Map = map,
            RespawnLocation1 = respawnLocation1,
            RespawnLocation2 = respawnLocation2,
            RespawnLocation3 = respawnLocation3
        };
    }

    public void WriteToFile(XBinWriter writer)
    {
        writer.WriteUnicodeNullTerminatedString(Name);
        writer.WriteUnicodeNullTerminatedString(MissionLine);
        writer.WriteUnicodeNullTerminatedString(SdsPrefix);
        writer.WriteUnicodeNullTerminatedString(Map);
        writer.WriteUnicodeNullTerminatedString(RespawnLocation1);
        writer.WriteUnicodeNullTerminatedString(RespawnLocation2);
        writer.WriteUnicodeNullTerminatedString(RespawnLocation3);
    }
}