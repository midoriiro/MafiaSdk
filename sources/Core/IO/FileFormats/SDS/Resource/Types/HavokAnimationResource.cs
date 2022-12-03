using Core.IO.FileFormats.Hashing;
using Core.IO.FileFormats.SDS.Resource.Manifest.Attributes;
using Core.IO.ResourceFormats.Havok.Classes;
using Core.IO.ResourceFormats.Havok.Headers;
using Core.IO.ResourceFormats.Havok.Sections;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

// We do not actually use this yet; We just use it to get the names.
public class HavokAnimationResource : IResourceType<HavokAnimationResource>
{
    public uint Unk0 { get; set; }
    public ulong FileHash { get; set; }
    public uint Unk1 { get; set; }
    public string RootClassName { get; set; } = null!;
    [IgnoreFieldDescriptor]
    public byte[] Data { get; set; } = null!;

    private HavokAnimationResource()
    {
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteValueU32(Unk0, endian);
        stream.WriteValueU64(FileHash, endian);
        stream.WriteStringU32(RootClassName, endian);
        stream.WriteValueU32(Unk1);
        stream.WriteBytes(Data);
    }

    public static HavokAnimationResource Deserialize(ushort version, Stream stream, Endian endian)
    {
        uint unk0 = stream.ReadValueU32(endian);
        ulong fileHash = stream.ReadValueU64(endian);
        string rootClassName = stream.ReadStringU32(endian);
        uint unk1 = stream.ReadValueU32(endian);
        byte[] data = stream.ReadBytes((int)(stream.Length - stream.Position));

        return new HavokAnimationResource
        {
            Unk0 = unk0,
            FileHash = fileHash,
            Unk1 = unk1,
            RootClassName = rootClassName,
            Data = data 
        };
    }

    public string DetermineName(Endian endian)
    {
        using var stream = new MemoryStream(Data);

        // Port from https://github.com/krenyy/botw_havok
        Header header = Header.Deserialize(stream, endian);
        SectionHeader classNamesSectionHeader = SectionHeader.Deserialize(stream, endian);
        SectionHeader typesSectionHeader = SectionHeader.Deserialize(stream, endian);
        SectionHeader dataSectionHeader = SectionHeader.Deserialize(stream, endian);

        ClassNamesSection classNamesSection = ClassNamesSection.Deserialize(stream, endian, classNamesSectionHeader);
        TypesSection typesSection = TypesSection.Deserialize(stream, endian, typesSectionHeader);
        DataSection dataSection = DataSection.Deserialize(stream, endian, header, dataSectionHeader, classNamesSection);

        var rootClass = (RootLevelContainer?)dataSection.Classes
            .SingleOrDefault(@class => ((IClass)@class).GetHavokClassName() == RootClassName);

        if (rootClass is null)
        {
            throw new InvalidOperationException($"Root class '{RootClassName}' cannot be found");
        }

        var fileNamesHashes = rootClass
            .NamedVariants
            .ToLookup(variant => Fnv64.Hash(variant.Name), variant => variant.Name);

        if (fileNamesHashes.Contains(FileHash))
        {
            return $"{fileNamesHashes[FileHash].First()}.hkx";
        }

        if (rootClass.NamedVariants.Count == 0)
        {
            throw new InvalidOperationException("Cannot find filename hash");
        }

        return string.Empty;
    }
}