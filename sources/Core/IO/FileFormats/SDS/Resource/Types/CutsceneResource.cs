using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

// NOTE: Only supports M1: DE and M3 Cutscene formats.
// This IS NOT for Mafia II. However it would be ideal if we could also support Mafia II.
public class CutsceneResource : IResourceType<CutsceneResource>
{
    public class GcrResource
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
        
        internal GcrResource()
        {
            Name = null!;
            Content = null!;
        }
            
        internal GcrResource(string name, byte[] content)
        {
            Name = name;
            Content = content;
        }

        public override string ToString()
        {
            return $"Name: {Name} Size: {Content.Length}";
        }
    }
    
    public GcrResource[] GcrEntityRecords { get; internal init; } = null!;

    internal CutsceneResource()
    {
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteValueU32(0, endian);
        stream.WriteValueU32((uint)GcrEntityRecords.Length, endian);

        foreach(GcrResource record in GcrEntityRecords)
        {
            string name = record.Name.Replace(".gcr", string.Empty);
            stream.WriteStringU16(name, endian);
            stream.WriteBytes(record.Content);
        }
    }

    public static CutsceneResource Deserialize(ushort version, Stream stream, Endian endian)
    {
        stream.ReadValueU32(endian); // padding
        uint index = stream.ReadValueU32(endian);

        var records = new GcrResource[index];

        for (var i = 0; i < index; i++)
        {
            // deserialize name
            string name = stream.ReadStringU16(endian);
            name += ".gcr";

            // get size and move back 8 bytes
            stream.ReadValueU32(endian); // Unk01
            int size = stream.ReadValueS32(endian); // Size includes these 4 bytes and Unk01

            stream.Position -= 8;
            byte[] content = stream.ReadBytes(size);

            // Construct and store Entity record
            var resource = new GcrResource(name, content);
            records[i] = resource;
        }

        return new CutsceneResource()
        {
            GcrEntityRecords = records
        };
    }
}