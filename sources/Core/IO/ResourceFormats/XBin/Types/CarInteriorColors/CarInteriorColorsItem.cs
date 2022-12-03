namespace Core.IO.ResourceFormats.XBin.Types.CarInteriorColors;

public class CarInteriorColorsItem
{
    public int CarId { get; init; }
    public byte R1 { get; init; }
    public byte G1 { get; init; }
    public byte B1 { get; init; }
    
    public byte R2 { get; init; }
    public byte G2 { get; init; }
    public byte B2 { get; init; }
    
    public byte R3 { get; init; }
    public byte G3 { get; init; }
    public byte B3 { get; init; }
    
    public byte R4 { get; init; }
    public byte G4 { get; init; }
    public byte B4 { get; init; }
    
    public byte R5 { get; init; }
    public byte G5 { get; init; }
    public byte B5 { get; init; }
    
    public byte? Alpha { get; init; }
    public int Description { get; init; }

    internal CarInteriorColorsItem()
    {
    }

    public override string ToString()
    {
        return $"CarID = {CarId}";
    }
}