using Core.IO.ResourceFormats.XBin.CoreTypes;

namespace Core.IO.ResourceFormats.XBin.Types.CarTuningPackAvailability;

public class CarTuningPackAvailabilityItem
{
    public int Id { get; init; }
    public uint OverrideTuningItemsOffset { get; init; }
    public int[] TuningItems { get; init; } = null!;
    public int VehicleId { get; init; }
    public int Zero { get; init; }
    public XBinHash64Name PackageName { get; init; } = null!;

    private CarTuningPackAvailabilityItem()
    {
    }

    public static CarTuningPackAvailabilityItem ReadFromFile(BinaryReader reader)
    {
        int id = reader.ReadInt32();
        uint overrideTuningItemsOffset = reader.ReadUInt32();
        int count0 = reader.ReadInt32();
        int count1 = reader.ReadInt32();
        
        if (count0 != count1)
        {
            throw new InvalidDataException(
                $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
            );
        }
        
        int vehicleId = reader.ReadInt32();
        int zero = reader.ReadInt32();
        XBinHash64Name packageName = XBinHash64Name.ReadFromFile(reader);

        var tuningItems = new int[count0];

        return new CarTuningPackAvailabilityItem()
        {
            Id = id,
            OverrideTuningItemsOffset = overrideTuningItemsOffset,
            TuningItems = tuningItems,
            VehicleId = vehicleId,
            Zero = zero,
            PackageName = packageName
        };
    }

    public override string ToString()
    {
        return $"ID = {Id}";
    }
}