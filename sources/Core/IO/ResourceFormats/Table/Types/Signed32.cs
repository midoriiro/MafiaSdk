using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Table.Types;

public class Signed32 : ITableDataType<int>, ISerializableTableData, IDeserializableTableData<Signed32>
{
    public int Value { get; set; }
    
    public void Serialize(Stream stream, Endian endian)
    {
        stream.WriteValueS32(Value, endian);
    }

    public static Signed32 Deserialize(Stream stream, Endian endian)
    {
        int value = stream.ReadValueS32(endian);
        
        return new Signed32
        {
            Value = value
        };
    }
}