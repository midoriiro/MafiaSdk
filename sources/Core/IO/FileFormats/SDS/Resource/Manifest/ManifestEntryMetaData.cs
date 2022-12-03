using Core.IO.FileFormats.SDS.Archive;

namespace Core.IO.FileFormats.SDS.Resource.Manifest;

[Serializable]
public class ManifestEntryMetaData
{
    public ManifestEntryMetaDataResourceType Type { get; init; } = null!;
    public ushort Version { get; init; }
    public ulong FileHash { get; init; }
    public uint SlotRamRequired { get; init; }
    public uint SlotVramRequired { get; init; }
    public uint OtherRamRequired { get; init; }
    public uint OtherVramRequired { get; init; }

    public static ManifestEntryMetaData From(ResourceEntry entry, ResourceType? type)
    {
        ManifestEntryMetaDataResourceType resourceType;
            
        if (type is null)
        {
            resourceType = new ManifestEntryMetaDataResourceType
            {
                Id = -1,
                Name = "UnknownType",
                Parent = -1
            };
        }
        else
        {
            resourceType = new ManifestEntryMetaDataResourceType
            {
                Id = (int)type.Value.Id,
                Name = type.Value.Name,
                Parent = (int)type.Value.Parent
            };
        }
        
        return new ManifestEntryMetaData
        {
            Type = resourceType,
            Version = entry.Version,
            FileHash = entry.FileHash,
            SlotRamRequired = entry.SlotRamRequired,
            SlotVramRequired = entry.SlotVramRequired,
            OtherRamRequired = entry.OtherRamRequired,
            OtherVramRequired = entry.OtherVramRequired
        };
    }
}