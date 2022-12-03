namespace Core.IO.FileFormats.SDS.Resource.Results;

public class DeserializeResult
{
    public Manifest.Manifest Manifest { get; init; } = null!;
    public List<DataDescriptor> DataDescriptors { get; init; } = null!;
}