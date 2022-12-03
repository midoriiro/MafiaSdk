using Core.IO.ResourceFormats.Havok.Classes;
using Core.IO.ResourceFormats.Havok.Headers;
using Core.IO.ResourceFormats.Havok.Utils;
using Core.IO.Streams;
using Object = Core.IO.ResourceFormats.Havok.Utils.Object;

namespace Core.IO.ResourceFormats.Havok.Sections;

public class DataSection
{
    // TODO use IClass instead of object type
    public List<object> Classes { get; init; } = null!;
    
    private static Object GetObjectFromValue(IReadOnlyList<Object> objects, uint value)
    {
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var index = 0; index < objects.Count; index++)
        {
            Object @object = objects[index];
            
            if (@object.Offset == value)
            {
                return @object;
            }
        }

        return new Object();
    }
    
    public static DataSection Deserialize(
        Stream stream, 
        Endian endian, 
        Header header,
        SectionHeader sectionHeader, 
        ClassNamesSection classNamesSection
    )
    {
        Section section = Section.Deserialize(stream, endian, sectionHeader);
        
        // Since types section is not used we can use data section header to get absolute offset
        stream.Position = sectionHeader.AbsoluteOffset;

        var objects = new List<Object>();

        for (var index = 0; index < section.LinkedEntries.Count; index++)
        {
            LinkedEntry linkedEntry = section.LinkedEntries[index];
            Class @class = classNamesSection
                .Items
                .Single(item => item.Offset == linkedEntry.Destination);

            uint length;

            if (index + 1 < section.LinkedEntries.Count)
            {
                length = section.LinkedEntries[index + 1].Source - linkedEntry.Source;
            }
            else
            {
                length = sectionHeader.DataPointerOffset - linkedEntry.Source;
            }

            Object @object = Object.Deserialize(stream, endian, sectionHeader, length, @class);
            @object.Repository = objects;
            objects.Add(@object);
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var x = 0; x < objects.Count; x++)
        {
            Object @object = objects[x];
            
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var y = 0; y < section.DataPointers.Count; y++)
            {
                DataPointer dataPointer = section.DataPointers[y];
                
                bool isSourceWithinLimit = @object.Offset <= dataPointer.Source &&
                                           dataPointer.Source < @object.Offset + @object.Data.Length;

                bool isDestinationWithinLimit = @object.Offset <= dataPointer.Destination &&
                                                dataPointer.Source < @object.Offset + @object.Data.Length;

                if (isSourceWithinLimit && isDestinationWithinLimit)
                {
                    @object.DataPointers.Add(dataPointer - @object.Offset);
                }
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var y = 0; y < section.ClassMaps.Count; y++)
            {
                ClassMap classMap = section.ClassMaps[y];

                bool isSourceWithinLimit = @object.Offset <= classMap.Source &&
                                           classMap.Source < @object.Offset + @object.Data.Length;

                if (!isSourceWithinLimit)
                {
                    continue;
                }

                var classMapReference = ClassMapReference.Create(
                    classMap,
                    @object,
                    GetObjectFromValue(objects, classMap.Destination)
                );

                @object.ClassMapReferences.Add(classMapReference);
            }

            // Seek to the end of file to check for additional embedded hk files
            stream.Seek(sectionHeader.AbsoluteOffset + sectionHeader.EndOfFileOffset, SeekOrigin.Begin);
        }

        var classes = new List<object>();

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var index = 0; index < objects.Count; index++)
        {
            Object @object = objects[index];
            string className = @object.Class.Name;

            using var classStream = new MemoryStream(@object.Data);
            IClass @class = ClassFactory.Deserialize(className, classStream, endian, header, @object);
            classes.Add(@class);
        }

        return new DataSection
        {
            Classes = classes
        };
    }
}