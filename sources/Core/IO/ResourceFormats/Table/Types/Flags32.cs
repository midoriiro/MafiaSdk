using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Table.Types;

public class Flags32 : ITableDataType<uint>, ISerializableTableData, IDeserializableTableData<Flags32>
{
    public uint Value { get; set; }
    
    public void Serialize(Stream stream, Endian endian)
    {
        stream.WriteValueU32(Value);
    }

    public static Flags32 Deserialize(Stream stream, Endian endian)
    {
        uint value = stream.ReadValueU32(endian);
        
        return new Flags32
        {
            Value = value
        };
    }
}