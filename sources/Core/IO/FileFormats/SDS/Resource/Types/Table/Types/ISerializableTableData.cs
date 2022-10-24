using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types.Table.Types;

public interface ISerializableTableData
{
    void Serialize(Stream stream, Endian endian);
}