using Core.IO.ResourceFormats.XBin.CoreTypes;

namespace Core.IO.ResourceFormats.XBin.Types.MaterialsPhysics;

public class MaterialsPhysicsItem
{
    public uint Id { get; init; }
    public string MaterialName { get; init; } = null!;
    public XBinHash32Name SoundSwitch { get; init; } = null!;
    public uint GuidPart0 { get; init; }
    public uint GuidPart1 { get; init; }
    public float StaticFriction { get; init; }
    public float DynamicFriction { get; init; }
    public float Restitution { get; init; }
    public string TechnicalNote { get; init; } = null!; // Original property name: Poznamka, not sure if it is a great translation

    internal MaterialsPhysicsItem()
    {
    }

    public override string ToString()
    {
        return $"{Id} {MaterialName}";
    }
}