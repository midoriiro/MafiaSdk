using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.Cities.Mafia3;

public class Cuba : ICity
{
    public string Name { get; init; } = null!;
    public string MissionLine { get; init; } = null!;
    public string RespawnLocation1 { get; init; } = null!;
    public string RespawnLocation2 { get; init; } = null!;

    private Cuba()
    {
    }

    public static ICity ReadFromFile(BinaryReader reader)
    {
        string name = reader.ReadStringEncoded();
        string missionLine = reader.ReadStringEncoded();
        string respawnLocation1 = reader.ReadStringEncoded();
        string respawnLocation2 = reader.ReadStringEncoded();

        return new Cuba()
        {
            Name = name,
            MissionLine = missionLine,
            RespawnLocation1 = respawnLocation1,
            RespawnLocation2 = respawnLocation2
        };
    }

    public void WriteToFile(XBinWriter writer)
    {
        writer.WriteUnicodeNullTerminatedString(Name);
        writer.WriteUnicodeNullTerminatedString(MissionLine);
        writer.WriteUnicodeNullTerminatedString(RespawnLocation1);
        writer.WriteUnicodeNullTerminatedString(RespawnLocation2);
    }
}