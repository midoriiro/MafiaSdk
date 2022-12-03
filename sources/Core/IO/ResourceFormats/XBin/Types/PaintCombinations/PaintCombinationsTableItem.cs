namespace Core.IO.ResourceFormats.XBin.Types.PaintCombinations;

public class PaintCombinationsTableItem
{
    public int Id { get; init; }
    public int Unk01 { get; init; }
    public int MinOccurences { get; init; }
    public int MaxOccurences { get; init; }
    public string CarName { get; init; } = null!;
    public int[] ColorIndex { get; init; } = null!;

    internal PaintCombinationsTableItem()
    {
    }

    public override string ToString()
    {
        return $"{Id} {CarName}";
    }
}