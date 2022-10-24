using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types.Table.Types;

public interface ITableDataType<TValue> where TValue : notnull
{
    TValue Value { get; set; }

    Type GetDataType()
    {
        return typeof(TValue);
    }

    string? ToString()
    {
        return Value.ToString()!;
    }
}