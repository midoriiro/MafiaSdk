using Core.IO.ResourceFormats.Table.Types;

namespace Core.IO.ResourceFormats.Table;

public class Row
{
    public readonly List<ISerializableTableData> Values = new();

    public override string ToString()
    {
        return string.Join(", ", Values.Select(value => value.ToString()));
    }
}