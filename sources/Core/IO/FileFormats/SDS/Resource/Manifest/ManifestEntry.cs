namespace Core.IO.FileFormats.SDS.Resource.Manifest;

public class ManifestEntry
{
    public ManifestEntryMetaData MetaData { get; init; } = null!;
    public ManifestEntryDescriptors Descriptors { get; init; } = null!;
}