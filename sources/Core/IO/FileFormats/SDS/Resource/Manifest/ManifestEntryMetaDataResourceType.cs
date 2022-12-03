namespace Core.IO.FileFormats.SDS.Resource.Manifest;

public class ManifestEntryMetaDataResourceType
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public int Parent { get; init; }
}