using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Havok.Utils;

public class ClassMap
{
    public uint Source { get; init; }
    public uint Flags { get; init; }
    public uint Destination { get; init; }

    private ClassMap()
    {
    }

    public static ClassMap Deserialize(Stream stream, Endian endian)
    {
        uint source = stream.ReadValueU32(endian);
        uint flags = stream.ReadValueU32(endian);
        uint destination = stream.ReadValueU32(endian);

        return new ClassMap
        {
            Source = source,
            Flags = flags,
            Destination = destination
        };
    }
}