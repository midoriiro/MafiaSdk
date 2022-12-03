using Core.IO.ResourceFormats.XBin.CoreTypes;
using Core.IO.ResourceFormats.XBin.Types.Vehicles.Mafia3;

namespace Core.IO.ResourceFormats.XBin.Types.CarTrafficTuning;

public class CarTrafficTuningItem
{
    public int Id { get; init; }
    public int VehicleId { get; init; }
    public XBinHash64Name Name { get; init; } = null!;
    public int CollectionOffset { get; init; }
    public int CollectionCount1 { get; set; }
    public int CollectionCount2 { get; set; }
    public int[] TuningItems { get; set; } = null!;
    public ETrafficVehicleFlags VehicleFlags { get; init; } 
    public ETrafficVehicleLookFlags VehicleLookFlags { get; init; }
    public float Weight { get; init; }

    internal CarTrafficTuningItem()
    {
    }

    public override string ToString()
    {
        return $"ID = {Id}";
    }
}