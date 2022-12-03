

//THIS ISN'T GIBBED. BUT STILL USES GIBBED STUFF :)

using Core.IO.FileFormats.SDS.Resource.Manifest.Attributes;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

public class SoundResource : IResourceType<SoundResource>
{
    public string Name { get; set; } = null!;
    public int FileSize { get; set; }
    [IgnoreFieldDescriptor] 
    public byte[] Data { get; set; } = null!;

    // TODO check constructors resources are private/internal ?
    private SoundResource()
    {
    }
    
    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteValueU8((byte)Name.Length);
        stream.WriteString(Name);
        stream.WriteValueS32(FileSize);
        stream.WriteBytes(Data);
    }

    public static SoundResource Deserialize(ushort version, Stream stream, Endian endian)
    {
        byte length = stream.ReadValueU8();
        string name = stream.ReadString(length);
        int fileSize = stream.ReadValueS32(endian);
        byte[] data = stream.ReadBytes(fileSize);

        return new SoundResource
        {
            Name = name,
            FileSize = fileSize,
            Data = data
        };
    }
}