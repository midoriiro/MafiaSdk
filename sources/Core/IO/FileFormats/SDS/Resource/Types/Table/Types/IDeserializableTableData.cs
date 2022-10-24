using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types.Table.Types;

public interface IDeserializableTableData<out TType> where TType : IDeserializableTableData<TType>
{
    static abstract TType Deserialize(Stream stream, Endian endian);
}