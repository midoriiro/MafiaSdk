using System.Text;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types.Table.Types;

public class String32 : ITableDataType<string>, ISerializableTableData, IDeserializableTableData<String32>
{
    public string Value { get; set; }
    
    public void Serialize(Stream stream, Endian endian)
    {
        stream.WriteString(Value, 32, Encoding.UTF8);
    }

    public static String32 Deserialize(Stream stream, Endian endian)
    {
        string value = stream.ReadString(32, true, Encoding.UTF8);
        
        return new String32()
        {
            Value = value
        };
    }
}