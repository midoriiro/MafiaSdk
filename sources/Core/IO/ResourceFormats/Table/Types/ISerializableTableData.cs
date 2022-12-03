using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Table.Types;

public interface ISerializableTableData
{
    void Serialize(Stream stream, Endian endian);
}