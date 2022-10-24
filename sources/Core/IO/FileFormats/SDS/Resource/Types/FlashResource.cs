using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

public class FlashResource : IResourceType<FlashResource>
{
    public string FileName { get; set; } = null!;
    public ulong Hash { get; set; }
    public string Name { get; set; } = null!;
    public uint Size { get; set; }
    public byte[] Data { get; set; } = null!;

    internal FlashResource()
    {
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteStringU16(FileName, endian);
        stream.WriteValueU64(FileFormats.Hashing.FNV64.Hash(Name), endian);
        stream.WriteStringU16(Name, endian);
        stream.WriteValueS32(Data.Length, endian);
        stream.WriteBytes(Data);
    }
        
    public static FlashResource Deserialize(ushort version, Stream stream, Endian endian)
    {
        string fileName = stream.ReadStringU16(endian);
        ulong hash = stream.ReadValueU64(endian);
        string name = stream.ReadStringU16(endian);
        uint size = stream.ReadValueU32(endian);
        byte[] data = stream.ReadBytes((int)size);

        return new FlashResource()
        {
            FileName = fileName,
            Hash = hash,
            Name = name,
            Size = size,
            Data = data
        };
    }
}