namespace Core.IO.ResourceFormats.XBin.Types.CarWindowsTint;

public class CarWindowTintItem
{
    public uint Id { get; init; }
    public string Description { get; init; } = null!;
    public byte Red { get; init; }
    public byte Green { get; init; }
    public byte Blue { get; init; }
    public byte Alpha { get; init; }

    internal CarWindowTintItem()
    {
    }

    public override string ToString()
    {
        return $"{Id} - {Description}";
    }
}