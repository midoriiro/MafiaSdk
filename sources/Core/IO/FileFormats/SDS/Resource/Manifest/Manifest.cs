using System.Text.Json;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.SDS.Resource.Manifest.Converters;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Manifest;

public class Manifest
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new ()
    {
        WriteIndented = true,
        Converters = { new EntryDescriptorsConverter() }
    };
    
    public Endian Endian { get; init; }
    public uint Version { get; init; }
    public Platform Platform { get; init; }
    public byte[]? Unknown20 { get; init; }
    public bool RequireResourceInfoXml { get; init; }
    public List<ResourceType> ResourceTypes { get; init; } = null!;
    public List<ManifestEntry> Entries { get; init; } = new();

    public static Manifest From(ArchiveFile archiveFile, List<ManifestEntry> entries)
    {
        return new Manifest
        {
            Endian = archiveFile.Endian,
            Version = archiveFile.Version,
            Platform = archiveFile.Platform,
            Unknown20 = archiveFile.Unknown20,
            RequireResourceInfoXml = archiveFile.ResourceInfoXml is not null,
            ResourceTypes = archiveFile.ResourceTypes,
            Entries = entries
        };
    }

    public string Serialize()
    {
        return JsonSerializer.Serialize(this, JsonSerializerOptions);
    }

    public static Manifest Deserialize(string manifest)
    {
        return JsonSerializer.Deserialize<Manifest>(manifest, JsonSerializerOptions)!;
    }
}