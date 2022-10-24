using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types.Table.Types;

public class Color : ITableDataType<System.Drawing.Color>, ISerializableTableData, IDeserializableTableData<Color>
{
    public System.Drawing.Color Value { get; set; }

    public void Serialize(Stream stream, Endian endian)
    {
        float red = IntToFloat(Value.R);
        float green = IntToFloat(Value.G);
        float blue = IntToFloat(Value.B);
        stream.WriteValueF32(red);
        stream.WriteValueF32(green);
        stream.WriteValueF32(blue);
    }

    public static Color Deserialize(Stream stream, Endian endian)
    {
        float red = stream.ReadValueF32(endian);
        float green = stream.ReadValueF32(endian);
        float blue = stream.ReadValueF32(endian);

        return new Color()
        {
            Value = System.Drawing.Color.FromArgb(
                FloatToInt(red),
                FloatToInt(green),
                FloatToInt(blue)
            )
        };
    }

    private static int FloatToInt(float value)
    {
        return (int)MathF.Floor(value * 255);
    }

    private static float IntToFloat(int value)
    {
        return value / 255f;
    }

    public override string ToString()
    {
        return $"{Value.R}, {Value.G}, {Value.B}";
    }
}