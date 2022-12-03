using Core.IO.FileFormats.SDS.Resource.Manifest;

namespace Core.IO.FileFormats.SDS.Resource.Results;

public class EntryDeserializeResult
{
    public ManifestEntryDescriptors ManifestEntryDescriptors { get; init; } = null!;
    public DataDescriptor[] DataDescriptors { get; init; } = null!;
}