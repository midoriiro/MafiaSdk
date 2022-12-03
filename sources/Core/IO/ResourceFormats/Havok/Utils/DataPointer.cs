using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Havok.Utils;

public class DataPointer
{
    public uint Source { get; init; }
    public uint Destination { get; init; }

    private DataPointer()
    {
    }

    public static DataPointer Deserialize(Stream stream, Endian endian)
    {
        uint source = stream.ReadValueU32(endian);
        uint destination = stream.ReadValueU32(endian);

        return new DataPointer
        {
            Source = source,
            Destination = destination
        };
    }

    public static DataPointer operator +(DataPointer dataPointer, uint value)
    {
        return new DataPointer
        {
            Source = dataPointer.Source + value,
            Destination = dataPointer.Destination + value
        };
    }
    
    public static DataPointer operator -(DataPointer dataPointer, uint value)
    {
        return new DataPointer
        {
            Source = dataPointer.Source - value,
            Destination = dataPointer.Destination - value
        };
    }
}