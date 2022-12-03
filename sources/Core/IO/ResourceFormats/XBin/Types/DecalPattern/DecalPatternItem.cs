using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.DecalPattern;

public class DecalPatternItem
{
    public long Id { get; init; }
    public float UvMinX { get; init; }
    public float UvMinY { get; init; }
    public float UvMaxX { get; init; }
    public float UvMaxY { get; init; }
    public uint MaterialGuidPart0 { get; init; }
    public uint MaterialGuidPart1 { get; init; }
    public float MinRadius { get; init; }
    public float MaxRadius { get; init; }
    public EDecalFlags Flags { get; init; }
    public float Impact { get; init; }
    public int TexCols { get; init; }
    public int TexRows { get; init; }
    public int TexStart { get; init; }
    public int TexEnd { get; init; } 
    public uint Group { get; init; }
    public int MultiDecal { get; init; }
    public float BlendTime { get; init; }
    public int FootStep { get; init; }
    public string Notes { get; init; } = null!;

    internal DecalPatternItem()
    {
    }

    public override string ToString()
    {
        return $"ID = {Id}";
    }
}