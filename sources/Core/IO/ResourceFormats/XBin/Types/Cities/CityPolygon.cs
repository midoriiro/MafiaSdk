using Core.IO.ResourceFormats.XBin.CoreTypes;
using Core.IO.ResourceFormats.XBin.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.Cities;

public class CityPolygon
{
    public string Name { get; init; } = null!;
    public XBinHash64Name TextId { get; init; } = null!;
    public ulong Unk0 { get; init; } // TODO: Could include property - PoliceArrivalMultiplier?
    public ushort[] Indexes { get; init; } = null!;

    private CityPolygon()
    {
    }

    public static CityPolygon ReadFromFile(BinaryReader reader)
    {
        reader.ReadUInt32(); // offset
        uint count0 = reader.ReadUInt32();
        uint count1 = reader.ReadUInt32();
            
        if (count0 != count1)
        {
            throw new InvalidDataException(
                $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
            );
        }
        
        string name = reader.ReadStringPointerWithOffset();
        XBinHash64Name textId = XBinHash64Name.ReadFromFile(reader);
        ulong unk0 = reader.ReadUInt64();
        var indexes = new ushort[count0];

        for (var z = 0; z < indexes.Length; z++)
        {
            indexes[z] = reader.ReadUInt16();
        }

        return new CityPolygon()
        {
            Name = name,
            TextId = textId,
            Unk0 = unk0,
            Indexes = indexes
        };
    }
}