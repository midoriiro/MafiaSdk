using Core.IO.ResourceFormats.XBin.CoreTypes;
using Core.IO.ResourceFormats.XBin.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands.Vehicle;

public class VehicleInstance
{
    public XBinVector3 Position { get; init; } = null!;
    public XBinVector3 Direction { get; init; } = null!;
    public string EntityName { get; init; } = null!;
    public uint LoadFlags { get; init; }

    private VehicleInstance()
    {
    }

    public static VehicleInstance ReadFromFile(BinaryReader reader)
    {
        XBinVector3 position = XBinVector3.ReadFromFile(reader);
        XBinVector3 direction = XBinVector3.ReadFromFile(reader);
        string entityName = reader.ReadStringPointerWithOffset();
        uint loadFlags = reader.ReadUInt32();

        return new VehicleInstance()
        {
            Position = position,
            Direction = direction,
            EntityName = entityName,
            LoadFlags = loadFlags
        };
    }

    public void WriteToFile(XBinWriter writer)
    {
        Position.WriteToFile(writer);
        Direction.WriteToFile(writer);
        writer.PushStringPointer(EntityName);
        writer.Write(LoadFlags);
    }
    
    public void FixUpStringPointer(XBinWriter writer)
    {
        writer.FixUpStringPointer(EntityName);
    }

    public static int GetSize()
    {
        return 32;
    }
}