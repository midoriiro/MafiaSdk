

//THIS ISN'T GIBBED. BUT STILL USES GIBBED STUFF :)

using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

public class SoundResource : IResourceType<SoundResource>
{
    private byte[]? _data;

    public string Name { get; set; } = null!;
    public int FileSize { get; set; }
    public byte[] Data {
        get => _data!;
        set {
            _data = value;
            FileSize = value.Length;
        }
    }

    internal SoundResource()
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

        return new SoundResource()
        {
            Name = name,
            FileSize = fileSize,
            Data = data
        };
    }
}