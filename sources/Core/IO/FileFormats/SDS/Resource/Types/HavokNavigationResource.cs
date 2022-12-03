using Core.IO.FileFormats.SDS.Resource.Manifest.Attributes;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

// We do not actually use this yet; We just use it to get the names.
public class HavokNavigationResource : IResourceType<HavokNavigationResource>
{
    public uint Unk0 { get; set; }
    public uint Unk1 { get; set; }
    public uint Unk2 { get; set; }
    public uint Unk3 { get; set; }
    public uint Unk4 { get; set; }
    [IgnoreFieldDescriptor]
    public byte[] Data { get; set; } = null!;

    private HavokNavigationResource()
    {
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteValueU32(Unk0, endian);
        stream.WriteValueU32(Unk1, endian);
        stream.WriteValueU32(Unk2, endian);
        stream.WriteValueU32(Unk3, endian);
        stream.WriteValueU32(Unk4, endian);
        stream.WriteBytes(Data);
    }

    public static HavokNavigationResource Deserialize(ushort version, Stream stream, Endian endian)
    {
        uint unk0 = stream.ReadValueU32(endian);
        uint unk1 = stream.ReadValueU32(endian);
        uint unk2 = stream.ReadValueU32(endian);
        uint unk3 = stream.ReadValueU32(endian);
        uint unk4 = stream.ReadValueU32(endian);
        byte[] data = stream.ReadBytes((int)(stream.Length - stream.Position));

        return new HavokNavigationResource
        {
            Unk0 = unk0,
            Unk1 = unk1,
            Unk2 = unk2,
            Unk3 = unk3,
            Unk4 = unk4,
            Data = data 
        };
    }
}