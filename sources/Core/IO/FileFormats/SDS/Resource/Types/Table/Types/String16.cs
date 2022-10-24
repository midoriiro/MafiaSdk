using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types.Table.Types;

public class String16 : ITableDataType<string>, ISerializableTableData, IDeserializableTableData<String16>
{
    public string Value { get; set; }
    
    public void Serialize(Stream stream, Endian endian)
    {
        stream.WriteString(Value, 16);
    }

    public static String16 Deserialize(Stream stream, Endian endian)
    {
        string value = stream.ReadString(16, true);
        
        return new String16()
        {
            Value = value
        };
    }
}