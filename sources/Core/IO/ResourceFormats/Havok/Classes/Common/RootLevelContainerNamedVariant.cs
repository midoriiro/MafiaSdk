using Core.IO.ResourceFormats.Havok.Extensions;
using Core.IO.ResourceFormats.Havok.Headers;
using Core.IO.ResourceFormats.Havok.Utils;
using Core.IO.Streams;
using Object = Core.IO.ResourceFormats.Havok.Utils.Object;

namespace Core.IO.ResourceFormats.Havok.Classes.Common;

public class RootLevelContainerNamedVariant
{
    public string Name { get; init; } = null!;
    public string ClassName { get; init; } = null!;
    public IClass Variant { get; init; } = null!;

    private RootLevelContainerNamedVariant()
    {
    }

    public static RootLevelContainerNamedVariant Deserialize(
        Stream stream, 
        Endian endian, 
        Header header, 
        Object @object
    )
    {
        long nameOffset = stream.AssertPointer(endian, header.PointerSize);
        long classNameOffset = stream.AssertPointer(endian, header.PointerSize);
        long variantOffset = stream.AssertPointer(endian, header.PointerSize);

        string name = null!;
        string className = null!;

        foreach (DataPointer dataPointer in @object.DataPointers)
        {
            stream.StepIn(dataPointer.Destination);

            if (dataPointer.Source == nameOffset)
            {
                name = stream.ReadStringZ();
            }
            else if (dataPointer.Source == classNameOffset)
            {
                className = stream.ReadStringZ();
            }
            
            stream.StepOut();
        }

        IClass variant = null!;

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var index = 0; index < @object.ClassMapReferences.Count; index++)
        {
            ClassMapReference classMapReference = @object.ClassMapReferences[index];
            if (classMapReference.SourceRelativeOffset != variantOffset)
            {
                continue;
            }

            using var variantStream = new MemoryStream(classMapReference.Destination.Data);
            variant = ClassFactory.Deserialize(
                classMapReference.Destination.Class.Name,
                variantStream,
                endian,
                header,
                classMapReference.Destination
            );

            classMapReference.Destination.RemoveFromRepository();
        }

        return new RootLevelContainerNamedVariant
        {
            Name = name,
            ClassName = className,
            Variant = variant
        };
    }
}