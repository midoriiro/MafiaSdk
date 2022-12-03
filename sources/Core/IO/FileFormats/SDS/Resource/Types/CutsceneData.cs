using Core.IO.FileFormats.SDS.Resource.Manifest.Attributes;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

public class CutsceneData : IResourceType<CutsceneData>
{
    public string Name { get; set; } = null!;
    public uint Unk0 { get; set; }
    [IgnoreFieldDescriptor] 
    public byte[] Data { get; set; } = null!;
        
    private CutsceneData()
    {
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        string name = Name.Replace(".gcr", string.Empty);
        stream.WriteStringU16(name, endian);
        stream.WriteValueU32(Unk0);
        stream.WriteValueS32(Data.Length);
        stream.Position -= 8;
        stream.WriteBytes(Data);
    }

    public static CutsceneData Deserialize(ushort version, Stream stream, Endian endian)
    {
        string name = stream.ReadStringU16(endian);
        name += ".gcr";
            
        uint unk0 = stream.ReadValueU32(endian); 
        int size = stream.ReadValueS32(endian); // TODO check if its an int ?
        stream.Position -= 8;
        byte[] data = stream.ReadBytes(size);

        return new CutsceneData
        {
            Name = name,
            Unk0 = unk0,
            Data = data
        };
    }
        
    public override string ToString()
    {
        return $"Name: {Name} Size: {Data.Length}";
    }
}