using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Table.Types;

public class Boolean : ITableDataType<bool>, ISerializableTableData, IDeserializableTableData<Boolean>
{
    public bool Value { get; set; }

    public void Serialize(Stream stream, Endian endian)
    {
        var value = Convert.ToUInt32(Value);
        stream.WriteValueU32(value, endian);
    }

    public static Boolean Deserialize(Stream stream, Endian endian)
    {
        uint value = stream.ReadValueU32(endian);
                    
        if (value != 0 && value != 1)
        {
            throw new FormatException();
        }

        return new Boolean
        {
            Value = value == 1
        };
    }
}