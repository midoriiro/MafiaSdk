namespace Core.IO.ResourceFormats.XBin.Types.CarSkidmarks;

public class CarSkidmarksItem
{
    public uint Id { get; init; }
    public string MaterialName { get; init; } = null!;
    public int SkidId { get; init; }
    public float SkidAlpha { get; init; }
    public int RideId { get; init; }
    public float RideAlpha { get; init; }
    public float TerrainDeep { get; init; }
    public float FadeTime { get; init; }

    internal CarSkidmarksItem()
    {
    }

    public override string ToString()
    {
        return $"{Id} - {MaterialName}";
    }
}