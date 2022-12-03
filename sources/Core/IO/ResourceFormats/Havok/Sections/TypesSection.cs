using Core.IO.ResourceFormats.Havok.Headers;
using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Havok.Sections;

public class TypesSection
{
    public Section Section { get; init; } = null!;

    private TypesSection()
    {
    }

    public static TypesSection Deserialize(Stream stream, Endian endian, SectionHeader header)
    {
        return new TypesSection
        {
            Section = Section.Deserialize(stream, endian, header)
        };
    }
}