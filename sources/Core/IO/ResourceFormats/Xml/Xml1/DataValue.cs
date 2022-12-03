namespace Core.IO.ResourceFormats.Xml.Xml1;

internal class DataValue
{
    public DataType Type { get; set; }
    public object Value { get; set; }

    public DataValue(DataType type, object value)
    {
        this.Type = type;
        this.Value = value;
    }

    public override string ToString()
    {
        return this.Value.ToString();
    }
}