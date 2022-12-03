using Core.IO.ResourceFormats.XBin.CoreTypes;

namespace Core.IO.ResourceFormats.XBin.Types.MaterialsShots;

public class MaterialsShotsItem
{
    public uint Id { get; init; }
    public string MaterialName { get; init; } = null!;
    public uint GuidPart0 { get; init; }
    public uint GuidPart1 { get; init; }
    public XBinHash32Name SoundSwitch { get; init; } = null!;
    public float Penetration { get; init; }
    public uint Particle { get; init; }
    public uint Decal { get; init; }
    public uint DecalCold { get; init; }

    internal MaterialsShotsItem()
    {
    }

    public override string ToString()
    {
        return $"{Id} {MaterialName}";
    }
}