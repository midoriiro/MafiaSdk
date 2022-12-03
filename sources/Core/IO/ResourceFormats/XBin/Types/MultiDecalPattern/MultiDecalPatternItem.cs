using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.MultiDecalPattern;

public class MultiDecalPatternItem
{
    public uint Id { get; init; }
    public float Probability { get; init; }
    public EMultiDecalFlags Flags { get; init; }
    public uint NumDecals { get; init; }
    public float MaxRightShift { get; init; }
    public float MaxUpShift { get; init; }
    public float ScaleFactor { get; init; }
    public float ScaleRand { get; init; }

    internal MultiDecalPatternItem()
    {
    }

    public override string ToString()
    {
        return $"ID = {Id}";
    }
}