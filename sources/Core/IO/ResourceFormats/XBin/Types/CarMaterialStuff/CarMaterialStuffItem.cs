using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.CarMaterialStuff;

public class CarMaterialStuffItem
{
    public uint Id { get; init; }
    public string MaterialName { get; init; } = null!;
    public ECarMtrStuffFlags Flags { get; init; } // TODO move this here
    public float DirtSpeedMin { get; init; }
    public float DirtSpeedMax { get; init; }
    public float DirtCoefficient { get; init; }
    public float TemperaturePercentCoefficient { get; init; }

    internal CarMaterialStuffItem()
    {
    }

    public override string ToString()
    {
        return $"{Id} - {MaterialName}";
    }
}