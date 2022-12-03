namespace Core.IO.ResourceFormats.Havok.Utils;

public class ClassMapReference
{
    public Object Source { get; init; } = null!;
    public Object Destination { get; init; } = null!;

    public uint SourceRelativeOffset { get; init; }
    public uint DestinationRelativeOffset { get; init; }
    public uint DestinationSectionId { get; init; }
    
    private ClassMapReference()
    {
    }

    public static ClassMapReference Create(ClassMap classMap, Object sourceObject, Object destinationObject)
    {
        return new ClassMapReference
        {
            Source = sourceObject,
            SourceRelativeOffset = classMap.Source - sourceObject.Offset,
            Destination = destinationObject,
            DestinationRelativeOffset = classMap.Destination - destinationObject.Offset
        };
    }
}