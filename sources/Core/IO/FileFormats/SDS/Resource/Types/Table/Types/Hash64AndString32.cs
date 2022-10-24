using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types.Table.Types;

public class Hash64AndString32 : ITableDataType<string>, ISerializableTableData, IDeserializableTableData<Hash64AndString32>
{
    public string Value { get; set; } = null!;
    public ulong Hash { get; set; }

    public void Serialize(Stream stream, Endian endian)
    {
        ulong hash = Hashing.FNV64.Hash(Value);
        stream.WriteValueU64(hash);
        stream.WriteString(Value, 32);
    }

    public static Hash64AndString32 Deserialize(Stream stream, Endian endian)
    {
        ulong hash = stream.ReadValueU64(endian);
        string value = stream.ReadString(32, true);
        
        return new Hash64AndString32()
        {
            Value = value,
            Hash = hash
        };
    }

    public override string ToString()
    {
        return $"{Hash}: {Value}";
    }
}