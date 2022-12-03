using Core.IO.ResourceFormats.Havok.Headers;
using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Havok.Utils;

public class Class
{
    public uint Signature { get; internal init; }
    public int Delimiter { get; internal init; }
    public long Offset { get; internal init; }
    public string Name { get; internal init; } = null!;

    private Class()
    {
    }

    public static Class Deserialize(Stream stream, Endian endian, SectionHeader header)
    {
        uint signature = stream.ReadValueU32(endian);
        int delimiter = stream.ReadByte().AssertValue(9); // Delimiter between signature and class name
        long offset = stream.Position - header.AbsoluteOffset;
        string name = stream.ReadStringZ();

        return new Class
        {
            Signature = signature,
            Delimiter = delimiter,
            Offset = offset,
            Name = name
        };
    }
}