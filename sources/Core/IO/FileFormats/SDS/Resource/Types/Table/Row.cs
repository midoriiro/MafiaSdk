using Core.IO.FileFormats.SDS.Resource.Types.Table.Types;

namespace Core.IO.FileFormats.SDS.Resource.Types.Table;

public class Row
{
    public readonly List<ISerializableTableData> Values = new();

    public override string ToString()
    {
        return string.Join(", ", Values.Select(value => value.ToString()));
    }
}