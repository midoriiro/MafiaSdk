using Core.IO.ResourceFormats.Havok.Classes.Common;
using Core.IO.ResourceFormats.Havok.Extensions;
using Core.IO.ResourceFormats.Havok.Headers;
using Core.IO.ResourceFormats.Havok.Utils;
using Core.IO.Streams;
using Object = Core.IO.ResourceFormats.Havok.Utils.Object;

namespace Core.IO.ResourceFormats.Havok.Classes;

public class RootLevelContainer : IClass
{
    public List<RootLevelContainerNamedVariant> NamedVariants { get; init; } = null!;
    
    private RootLevelContainer()
    {
    }

    public static IClass Deserialize(Stream stream, Endian endian, Header header, Object @object)
    {
        long offset = stream.AssertPointer(endian, header.PointerSize);
        uint count = stream.ReadCounter(endian);

        List<RootLevelContainerNamedVariant> namedVariants = new();

        foreach (DataPointer dataPointer in @object.DataPointers)
        {
            stream.StepIn(dataPointer.Destination);

            if (dataPointer.Source == offset)
            {
                for (var index = 0; index < count; index++)
                {
                    RootLevelContainerNamedVariant namedVariant = RootLevelContainerNamedVariant.Deserialize(
                        stream, 
                        endian, 
                        header, 
                        @object
                    );
                    namedVariants.Add(namedVariant);
                }
            }
            
            stream.StepOut();
        }

        return new RootLevelContainer
        {
            NamedVariants = namedVariants
        };
    }
}