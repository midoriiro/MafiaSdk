using Core.IO.ResourceFormats.Havok.Headers;
using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Havok.Utils;

public class Object
{
    public uint Offset { get; init; }
    public byte[] Data { get; init; } = null!;
    public Class Class { get; init; } = null!;
    public List<DataPointer> DataPointers { get; } = new();
    public List<ClassMapReference> ClassMapReferences { get; } = new();
    public List<Object> Repository { get; internal set; } = null!;
    
    internal Object()
    {
    }

    public static Object Deserialize(
        Stream stream, 
        Endian endian, 
        SectionHeader header, 
        uint length, 
        Class @class
    )
    {
        long rawOffset = stream.Position - header.AbsoluteOffset;

        if (rawOffset < 0)
        {
            rawOffset = 0;
        }
        
        var offset = (uint)rawOffset;
        byte[] data = stream.ReadBytes((int)length);

        return new Object
        {
            Offset = offset,
            Data = data,
            Class = @class
        };
    }

    public void RemoveFromRepository()
    {
        Repository.Remove(this);
    }
}