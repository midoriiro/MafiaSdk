using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.Streams;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS;

internal class ArchivePatchFile
{
    public const uint Signature = 0xD010F0F;
    public const uint Signature2 = 0xF0F0010D;
        
    private int _unkTotal; //UnkCount1 and UnkCount2 added together.
    private ResourceType[] Types { get; set; } = null!;
    private FileInfo File { get; init; }
        
    public ResourceEntry[] Resources { get; set; } = null!;
        
    private int UnkCount1 { get; set; }
    public int[] UnkInts1 { get; set; } = null!;
    private int UnkCount2 { get; set; }
    public int[] UnkInts2 { get; set; } = null!;

    internal ArchivePatchFile(FileInfo file)
    {
        File = file;
    }

    public void Deserialize(Stream reader, Endian endian)
    {
        int magic = reader.ReadValueS32(endian);
            
        if (magic != Signature)
        {
            reader.Position -= 4;
            magic = reader.ReadValueS32(endian == Endian.Big ? Endian.Little : Endian.Big);

            if (magic != Signature)
            {
                return;
            }
                
            endian = endian == Endian.Big ? Endian.Little : Endian.Big;
        }

        int version = reader.ReadValueS32(endian);
        //if (version > 1)
        //    return;

        uint magic2 = reader.ReadValueU32(endian);
            
        if (magic2 != Signature2)
        {
            return;
        }

        int typesCount = reader.ReadValueS32(endian);
        Types = new ResourceType[typesCount];
            
        for(var i = 0; i < typesCount; i++)
        {
            Types[i] = ResourceType.Read(reader, endian);
        }

        var indexes = new List<string> { "UnkSet0:" };

        UnkCount1 = reader.ReadValueS32(endian);
        UnkInts1 = new int[UnkCount1];
            
        for (var i = 0; i != UnkCount1; i++)
        {
            UnkInts1[i] = reader.ReadValueS32(endian);
            indexes.Add(UnkInts1[i].ToString());
        }
            
        indexes.Add("/nUnkSet1:");
            
        UnkCount2 = reader.ReadValueS32(endian);
        UnkInts2 = new int[UnkCount2];
            
        for (var i = 0; i != UnkCount2; i++)
        {
            UnkInts2[i] = reader.ReadValueS32(endian);
            indexes.Add(UnkInts2[i].ToString());
        }

        _unkTotal = reader.ReadValueS32(endian);

        if (UnkCount1 + UnkCount2 != _unkTotal)
        {
            throw new FormatException();
        }

        if (_unkTotal == 0)          
            return;

        var position = (int)reader.Position;

        BlockReaderStream blockStream = BlockReaderStream.FromStream(reader, endian);
        reader.Position = position;

        Resources = new ResourceEntry[_unkTotal];
            
        for (uint i = 0; i < Resources.Length; i++)
        {
            ResourceHeader resourceHeader;

            using (MemoryStream data = blockStream.ReadToMemoryStream(26))
            {
                resourceHeader = ResourceHeader.Read(data, endian, 19);
            }
                
            blockStream.ReadBytes(4); //checksum ?
                
            if (resourceHeader.Size < 30)
            {
                throw new FormatException();
            }
                
            Resources[i] = new ResourceEntry()
            {
                TypeId = (int)resourceHeader.TypeId,
                Version = resourceHeader.Version,
                Data = blockStream.ReadBytes((int)resourceHeader.Size - 30),
                SlotRamRequired = resourceHeader.SlotRamRequired,
                SlotVramRequired = resourceHeader.SlotVramRequired,
                OtherRamRequired = resourceHeader.OtherRamRequired,
                OtherVramRequired = resourceHeader.OtherVramRequired
            };
        }
    }
}