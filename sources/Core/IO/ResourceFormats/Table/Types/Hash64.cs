using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Table.Types;

public class Hash64 : ITableDataType<ulong>, ISerializableTableData, IDeserializableTableData<Hash64>
{
    public ulong Value { get; set; }
    
    public void Serialize(Stream stream, Endian endian)
    {
        stream.WriteValueU64(Value);
    }

    public static Hash64 Deserialize(Stream stream, Endian endian)
    {
        ulong value = stream.ReadValueU64(endian);
        
        return new Hash64
        {
            Value = value 
        };
    }
}