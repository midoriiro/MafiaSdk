using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types.Table.Types;

public class Float32 : ITableDataType<float>, ISerializableTableData, IDeserializableTableData<Float32>
{
    public float Value { get; set; }
    
    public void Serialize(Stream stream, Endian endian)
    {
        stream.WriteValueF32(Value, endian);
    }

    public static Float32 Deserialize(Stream stream, Endian endian)
    {
        float value = stream.ReadValueF32(endian);
        
        return new Float32()
        {
            Value = value
        };
    }
}