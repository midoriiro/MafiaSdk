namespace Core.IO.ResourceFormats.XBin.Types.CarTuningModificators;

public class CarTuningModificatorsItem
{
    public int Id { get; init; }
    public int CarId { get; init; }
    public int ItemId { get; init; }
    public int MemberId { get; init; }
    public int Value { get; init; }

    internal CarTuningModificatorsItem()
    {
    }

    public override string ToString()
    {
        return $"CarID = {CarId} ItemId = {ItemId}";
    }
}