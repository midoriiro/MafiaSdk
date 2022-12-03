using Core.IO.FileFormats.SDS.Archive;

namespace Core.IO.FileFormats.SDS.Resource.Results;

public class EntrySerializeResult
{
    public ResourceEntry ResourceEntry { get; init; } = null!;
    public string DataDescriptor { get; init; } = null!;

    public override string ToString()
    {
        return ResourceEntry.ToString();
    }
}