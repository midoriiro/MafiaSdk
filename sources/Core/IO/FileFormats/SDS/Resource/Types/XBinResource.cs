using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

//also known as SystemObjectDatabase
public class XBinResource : IResourceType<XBinResource>
{
    //header
    public ulong Unk01 { get; set; }
    public int Size { get; set; }
    public byte[] Data { get; set; }

    //near the end
    public uint Unk02 { get; set; }
    public ulong Unk03 { get; set; }
    public string Unk04 { get; set; }
    public ulong Hash { get; set; }
    public string Name { get; set; }
    public byte[]? XmlData { get; set; }

    internal XBinResource()
    {
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteValueU64(Unk01, endian);
        stream.WriteValueS32(Size, endian);
        stream.WriteBytes(Data);
        stream.WriteValueU32(Unk02, endian);
        stream.WriteValueU64(Unk03, endian);
        stream.WriteStringU32(Unk04, endian);
        stream.WriteValueU64(Hash, endian);
        stream.WriteStringU32(Name, endian);
    }
    
    public static XBinResource Deserialize(ushort version, Stream stream, Endian endian)
    {
        ulong unk01 = stream.ReadValueU64(endian);
        int size = stream.ReadValueS32(endian);
        byte[] data = stream.ReadBytes(size);
        uint unk02 = stream.ReadValueU32(endian);
        ulong unk03 = stream.ReadValueU64(endian);
        string unk04 = stream.ReadStringU32(endian);
        ulong hash = stream.ReadValueU64(endian);
        string name = stream.ReadStringU32(endian);

        byte[]? xmlData = default;

        if(stream.Position != stream.Length)
        {
            ulong xmlSize = stream.ReadValueU64(endian);
            xmlData = stream.ReadBytes((int)xmlSize);
        }

        return new XBinResource()
        {
            Unk01 = unk01,
            Size = size,
            Data = data,
            Unk02 = unk02,
            Unk03 = unk03,
            Unk04 = unk04,
            Hash = hash,
            Name = name,
            XmlData = xmlData
        };
    }
}