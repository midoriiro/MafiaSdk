using Core.IO.ResourceFormats.XBin.CoreTypes;
using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.Missions;

public class MissionItem
{
    public XBinHash64Name Id { get; init; } = null!;
    public XBinHash64Name TextId { get; init; } = null!;
    public XBinHash64Name DescriptionId { get; init; } = null!;
    public uint IconId { get; init; }
    public uint CityId { get; init; }
    public EMissionType Type { get; init; }
    public uint DescriptionOffset1 { get; init; }
    public uint DescriptionOffset2 { get; init; }
    public string MissionId { get; init; } = null!;
    public string? CheckPointFile { get; init; }
    public uint? Unknown { get; init; }

    internal MissionItem()
    {
    }

    public override string ToString()
    {
        return $"{MissionId}";
    }
}