using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Havok.Headers;

public class SectionHeader
{
    public string SectionName { get; private init; } = null!;
    public uint AbsoluteOffset { get; private init; }
    public uint DataPointerOffset { get; private init; }
    public uint ClassMapOffset { get; private init; }
    public uint LinkedEntryOffset { get; private init; }
    public uint ExportsOffset { get; private init; }
    public uint ImportsOffset { get; private init; }
    public uint EndOfFileOffset { get; private init; }

    public static SectionHeader Deserialize(Stream stream, Endian endian)
    {
        string sectionName = stream.ReadStringZ();

        // Section name string size is 20 bytes length.
        // We need to read extra bytes to reach next values in case of a shorter string
        stream.ReadBytes(20 - sectionName.Length - 1);
        
        uint absoluteOffset = stream.ReadValueU32(endian);
        uint dataPointerOffset = stream.ReadValueU32(endian);
        uint classMapOffset = stream.ReadValueU32(endian);
        uint linkedEntryOffset = stream.ReadValueU32(endian);
        uint exportsOffset = stream.ReadValueU32(endian);
        uint importsOffset = stream.ReadValueU32(endian);
        uint endOfFileOffset = stream.ReadValueU32(endian);

        stream.ReadBytes(16); // section delimiter filled with 0xFF * 16

        return new SectionHeader
        {
            SectionName = sectionName,
            AbsoluteOffset = absoluteOffset,
            DataPointerOffset = dataPointerOffset,
            ClassMapOffset = classMapOffset,
            LinkedEntryOffset = linkedEntryOffset,
            ExportsOffset = exportsOffset,
            ImportsOffset = importsOffset,
            EndOfFileOffset = endOfFileOffset
        };
    }
}