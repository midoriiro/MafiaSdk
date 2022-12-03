namespace Core.IO.ResourceFormats.XBin.Types.DecalGroupPattern;

public class DecalGroupPatternItem
{
    public uint Id { get; init; }
    public uint MaxPc { get; init; }
    public uint MaxXbox { get; init; }
    public uint MaxPs3 { get; init; }
    public float FadeOut { get; init; }
    public uint MaxDistance { get; init; }

    internal DecalGroupPatternItem()
    {
    }

    public override string ToString()
    {
        return $"ID = {Id}";
    }
}