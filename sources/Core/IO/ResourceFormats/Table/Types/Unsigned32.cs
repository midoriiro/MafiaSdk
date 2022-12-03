using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Table.Types;

public class Unsigned32 : ITableDataType<uint>, ISerializableTableData, IDeserializableTableData<Unsigned32>
{
    public uint Value { get; set; }
    
    public void Serialize(Stream stream, Endian endian)
    {
        stream.WriteValueU32(Value);
    }

    public static Unsigned32 Deserialize(Stream stream, Endian endian)
    {
        uint value = stream.ReadValueU32(endian);
        
        return new Unsigned32
        {
            Value = value
        };
    }
}