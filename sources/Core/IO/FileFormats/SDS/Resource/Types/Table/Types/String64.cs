using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types.Table.Types;

public class String64 : ITableDataType<string>, ISerializableTableData, IDeserializableTableData<String64>
{
    public string Value { get; set; }
    
    public void Serialize(Stream stream, Endian endian)
    {
        stream.WriteString(Value, 64);
    }

    public static String64 Deserialize(Stream stream, Endian endian)
    {
        string value = stream.ReadString(64, true);
        
        return new String64()
        {
            Value = value
        };
    }
}