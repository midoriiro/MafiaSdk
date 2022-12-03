namespace Core.IO.ResourceFormats.XBin.Types.CarColors;

public class CarColorsItem
{
    public uint Id { get; init; }
    public string ColorNameIndex { get; init; } = null!;
    public byte Red { get; init; }
    public byte Green { get; init; }
    public byte Blue { get; init; }
    public string ColorName { get; init; } = null!;
    public string SpeechDarkLight { get; init; } = null!;
    public string PoliceComm { get; init; } = null!;
    public byte Unk0 { get; init; } // Unknown, maybe always zero? Could be for padding too.

    internal CarColorsItem()
    {
    }

    public override string ToString()
    {
        return $"{Id} - {ColorNameIndex}";
    }
}