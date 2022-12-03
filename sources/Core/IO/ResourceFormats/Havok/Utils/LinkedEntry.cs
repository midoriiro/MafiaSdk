using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Havok.Utils;

public class LinkedEntry
{
    public uint Source { get; init; }
    public uint Flags { get; init; }
    public uint Destination { get; init; }

    private LinkedEntry()
    {
    }

    public static LinkedEntry Deserialize(Stream stream, Endian endian)
    {
        uint source = stream.ReadValueU32(endian);
        uint flags = stream.ReadValueU32(endian);
        uint destination = stream.ReadValueU32(endian);

        return new LinkedEntry
        {
            Source = source,
            Flags = flags,
            Destination = destination
        };
    }
}