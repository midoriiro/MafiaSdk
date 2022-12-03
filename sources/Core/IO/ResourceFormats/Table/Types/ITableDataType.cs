namespace Core.IO.ResourceFormats.Table.Types;

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