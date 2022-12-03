namespace Core.IO.FileFormats.SDS.Resource.Results;

public class DataDescriptor
{
    public string Path { get; set; }
    public byte[] Data { get; private set; }
    
    public DataDescriptor(string path, byte[] data)
    {
        Path = path;
        Data = data;
    }

    public void Clear()
    {
        Data = Array.Empty<byte>();
    }
}