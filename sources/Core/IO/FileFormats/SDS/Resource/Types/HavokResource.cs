using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

// We do not actually use this yet; We just use it to get the names.
public class HavokResource : IResourceType<HavokResource>
{
    // Serialized
    public uint Unk01 { get; set; }
    public ulong FileHash { get; set; }
    public uint Unk02 { get; set; }
    public byte[] Data { get; set; } = null!;

    internal HavokResource()
    {
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteValueU32(Unk01, endian);
        stream.WriteValueU64(FileHash, endian);
        stream.WriteValueU32(Unk02);
        stream.WriteBytes(Data);
    }

    public static HavokResource Deserialize(ushort version, Stream stream, Endian endian)
    {
        uint unk01 = stream.ReadValueU32(endian);
        ulong fileHash = stream.ReadValueU64(endian);
        uint unk02 = stream.ReadValueU32(endian);
        byte[] data = stream.ReadBytes((int)(stream.Length - stream.Position));

        return new HavokResource()
        {
            Unk01 = unk01,
            Unk02 = unk02,
            FileHash = fileHash,
            Data = data
        };
    }
}