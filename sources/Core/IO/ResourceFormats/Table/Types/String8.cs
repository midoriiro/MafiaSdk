using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Table.Types;

public class String8 : ITableDataType<string>, ISerializableTableData, IDeserializableTableData<String8>
{
    public string Value { get; set; } = null!;
    
    public void Serialize(Stream stream, Endian endian)
    {
        stream.WriteString(Value, 8);
    }

    public static String8 Deserialize(Stream stream, Endian endian)
    {
        string value = stream.ReadString(8, true);
        
        return new String8
        {
            Value = value
        };
    }
}