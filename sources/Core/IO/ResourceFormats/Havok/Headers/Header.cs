using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Havok.Headers;

public class Header
{
    public ulong Signature { get; private init; }
    public uint UserTag { get; private init; }
    public uint Version { get; private init; }
    public int PointerSize { get; private init; }
    public int Endian { get; private init; }
    public int Padding { get; private init; }
    public int BaseClass { get; private init; }
    public uint SectionCount { get; private init; }
    public uint DataSectionIndex { get; private init; }
    public uint DataSectionOffset { get; private init; }
    public uint ClassNameIndex { get; private init; }
    public uint ClassNameOffset { get; private init; }
    public string VersionName { get; private init; } = null!;
    public int Unk2 { get; private init; }
    public uint Flags { get; private init; }
    public uint MaximumPredicate { get; private init; }
    public uint PredicateSize { get; private init; }
    public ushort SectionOffset { get; private init; }
    public uint? ExtraUnk0 { get; private init; }
    public uint? ExtraUnk1 { get; private init; }
    public uint? ExtraUnk2 { get; private init; }
    public uint? ExtraUnk3 { get; private init; }

    public static Header Deserialize(Stream stream, Endian endian)
    {
        ulong signature = stream.ReadValueU64(endian).AssertValue(0x10C0C01057E0E057U);
        uint userTag = stream.ReadValueU32(endian).AssertValue(0U); // always 0 ?
        uint version = stream.ReadValueU32(endian).AssertValue(11U); // always 11 ?
        int pointerSize = stream.ReadByte().AssertValue(4, 8); // always 8 on PC and 4 on console ?
        int endianness = stream.ReadByte().AssertValue(0, 1); // always 1 on PC and 0 on console ?
        int padding = stream.ReadByte().AssertValue(0, 1); // always 0 on PC ?
        int baseClass = stream.ReadByte().AssertValue(1); // always 1 ?
        uint sectionCount = stream.ReadValueU32(endian).AssertValue(3U); // always 3 ?
        uint dataSectionIndex = stream.ReadValueU32(endian).AssertValue(2U); // always 2 ?
        uint dataSectionOffset = stream.ReadValueU32(endian).AssertValue(0U); // always 0 ?
        uint classNameSectionIndex = stream.ReadValueU32(endian).AssertValue(0U); // always 0 ?
        uint classNameSectionOffset = stream.ReadValueU32(endian).AssertValue(75U); // always 75 ?
        string versionName = stream.ReadStringZ().AssertValue(14);
        int unknown2 = stream.ReadByte().AssertValue(0xFF); // always 255 ?
        uint flags = stream.ReadValueU32(endian).AssertValue(0U); // always 0 ?
        byte maximumPredicate = stream.ReadValueU8().AssertValue(21); // always 21 ?
        byte predicateSize = stream.ReadValueU8().AssertValue(0); // always 0 ?
        ushort sectionOffset = stream.ReadValueU16().AssertValue(0, 16); // 0 or 16; if 16, the header is 16 bytes longer

        uint? extraUnknown0 = null;
        uint? extraUnknown1 = null;
        uint? extraUnknown2 = null;
        uint? extraUnknown3 = null;

        // ReSharper disable once InvertIf
        if (sectionOffset == 16)
        {
            extraUnknown0 = stream.ReadValueU32(endian).AssertValue(20U); // always 20 ?
            extraUnknown1 = stream.ReadValueU32(endian).AssertValue(0U); // always 0 ?
            extraUnknown2 = stream.ReadValueU32(endian).AssertValue(0U); // always 0 ?
            extraUnknown3 = stream.ReadValueU32(endian).AssertValue(0U); // always 0 ?
        }

        return new Header
        {
            Signature = signature,
            UserTag = userTag,
            Version = version,
            PointerSize = pointerSize,
            Endian = endianness,
            Padding = padding,
            BaseClass = baseClass,
            SectionCount = sectionCount,
            DataSectionIndex = dataSectionIndex,
            DataSectionOffset = dataSectionOffset,
            ClassNameIndex = classNameSectionIndex,
            ClassNameOffset = classNameSectionOffset,
            VersionName = versionName,
            Unk2 = unknown2,
            Flags = flags,
            MaximumPredicate = maximumPredicate,
            PredicateSize = predicateSize,
            SectionOffset = sectionOffset,
            ExtraUnk0 = extraUnknown0,
            ExtraUnk1 = extraUnknown1,
            ExtraUnk2 = extraUnknown2,
            ExtraUnk3 = extraUnknown3
        };
    }
}