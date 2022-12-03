using Core.IO.ResourceFormats.Havok.Headers;
using Core.IO.ResourceFormats.Havok.Utils;
using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Havok.Sections;

public class ClassNamesSection
{
    private static readonly byte[] EndOfStream3 = { 0xFF, 0xFF, 0xFF };
    private static readonly byte[] EndOfStream4 = { 0xFF, 0xFF, 0xFF, 0xFF };
    public List<Class> Items { get; init; } = null!;
    
    private ClassNamesSection()
    {
    }

    public static ClassNamesSection Deserialize(Stream stream, Endian endian, SectionHeader header)
    {
        var items = new List<Class>();
        long readUntil = header.AbsoluteOffset + header.EndOfFileOffset;

        while (stream.Position < readUntil)
        {
            items.Add(Class.Deserialize(stream, endian, header));
            
            if (stream.Peek(4).SequenceEqual(EndOfStream4))
            {
                stream.Seek(4, SeekOrigin.Current);
                break;
            }
            // ReSharper disable once InvertIf
            if (stream.Peek(3).SequenceEqual(EndOfStream3))
            {
                stream.Seek(3, SeekOrigin.Current);
                break;
            }
        }

        return new ClassNamesSection
        {
            Items = items
        };
    }
}