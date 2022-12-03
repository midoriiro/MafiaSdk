using Core.IO.ResourceFormats.Havok.Headers;
using Core.IO.ResourceFormats.Havok.Utils;
using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Havok.Sections;

public class Section
{
    private const int EndOfStream = 0xFF;

    public List<DataPointer> DataPointers { get; private init; } = null!;
    public List<ClassMap> ClassMaps { get; private init; } = null!;
    public List<LinkedEntry> LinkedEntries { get; private init; } = null!;

    private Section()
    {
    }

    public static Section Deserialize(Stream stream, Endian endian, SectionHeader header)
    {
        uint readUntil = 0;
        uint bytesToRead = 0;
        
        // Read data pointers
        stream.StepIn( header.AbsoluteOffset + header.DataPointerOffset);

        readUntil = header.AbsoluteOffset + header.ClassMapOffset;
        bytesToRead = header.ClassMapOffset - header.DataPointerOffset;
        var dataPointers = new List<DataPointer>();
        
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach(int _ in Enumerable.Range(0, (int)bytesToRead))
        {
            if (stream.Position == readUntil || stream.Peek() == EndOfStream)
            {
                break;
            }
            
            dataPointers.Add(DataPointer.Deserialize(stream, endian));
        }
        
        // Verify we had read all data pointers. DataPointer is composed by two uint so we need to divide the expected
        // bytes to read by 8 (2 * 4 bytes representing the two uint).
        // In that way we get the expected data pointers count and we compared with the actual data pointers count.
        //
        // We also subtract '1' to the expected count because in some data pointer section, end section is filled
        // with 0xFF so we can ignore it.
        //
        // In other hand, some data pointer section don't end with termination byte pattern, we test both cases and if
        // expected data pointer count is not reached then it should fail.
        if (bytesToRead > 0 && bytesToRead / 8 - 1 != dataPointers.Count && bytesToRead / 8 != dataPointers.Count)
        {
            throw new InvalidOperationException(
                $"Data pointers expected count '{bytesToRead / 8}' differ from actual count '{dataPointers.Count}'"
            );
        }
        
        stream.StepOut();

        // Read class maps
        stream.StepIn(header.AbsoluteOffset + header.ClassMapOffset);
        
        readUntil = header.AbsoluteOffset + header.LinkedEntryOffset;
        bytesToRead = header.LinkedEntryOffset - header.ClassMapOffset;
        var classMaps = new List<ClassMap>();
        
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach(int _ in Enumerable.Range(0, (int)bytesToRead))
        {
            if (stream.Position == readUntil || stream.Peek() == EndOfStream)
            {
                break;
            }

            classMaps.Add(ClassMap.Deserialize(stream, endian));
        }
        
        // Same as above, except this time ClassMap size is 12 bytes
        if (bytesToRead > 0 && bytesToRead / 12 - 1 != classMaps.Count && bytesToRead / 12 != classMaps.Count)
        {
            throw new InvalidOperationException(
                $"Class maps expected count '{bytesToRead / 8}' differ from actual count '{classMaps.Count}'"
            );
        }
        
        stream.StepOut();
        
        // Read linked entries
        stream.StepIn(header.AbsoluteOffset + header.LinkedEntryOffset);

        readUntil = header.AbsoluteOffset + header.EndOfFileOffset;
        bytesToRead = header.ExportsOffset - header.LinkedEntryOffset;
        var linkedEntries = new List<LinkedEntry>();
        
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach(int _ in Enumerable.Range(0, (int)bytesToRead))
        {
            if (stream.Position == readUntil || stream.Peek() == EndOfStream)
            {
                break;
            }

            linkedEntries.Add(LinkedEntry.Deserialize(stream, endian));
        }
        
        // Same as above, except this time LinkedEntry size is 12 bytes
        if (bytesToRead > 0 && bytesToRead / 12 - 1 != linkedEntries.Count && bytesToRead / 12 != linkedEntries.Count)
        {
            throw new InvalidOperationException(
                $"Linked entries expected count '{bytesToRead / 8}' differ from actual count '{linkedEntries.Count}'"
            );
        }
        
        stream.StepOut();

        return new Section
        {
            DataPointers = dataPointers,
            ClassMaps = classMaps,
            LinkedEntries = linkedEntries
        };
    }
}