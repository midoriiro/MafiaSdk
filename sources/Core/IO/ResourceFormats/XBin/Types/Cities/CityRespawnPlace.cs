using Core.IO.ResourceFormats.XBin.CoreTypes;
using Core.IO.ResourceFormats.XBin.Enumerators;
using Core.IO.ResourceFormats.XBin.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.Cities;

public class CityRespawnPlace
{
    public string StreamMapPart { get; init; } = null!;
    public XBinVector3 PlayerPosition { get; init; } = null!;
    public XBinVector3 PlayerDirection { get; init; } = null!;
    public ERespawnPlaceType RespawnType { get; init; }

    private CityRespawnPlace()
    {
    }

    public static CityRespawnPlace ReadFromFile(BinaryReader reader)
    {
        XBinVector3 playerPosition = XBinVector3.ReadFromFile(reader);
        XBinVector3 playerDirection = XBinVector3.ReadFromFile(reader);
        var respawnType = (ERespawnPlaceType)reader.ReadInt32();
        string streamMapPart = reader.ReadStringPointerWithOffset();

        return new CityRespawnPlace()
        {
            StreamMapPart  = streamMapPart,
            PlayerPosition = playerPosition,
            PlayerDirection = playerDirection,
            RespawnType = respawnType
        };
    }
}