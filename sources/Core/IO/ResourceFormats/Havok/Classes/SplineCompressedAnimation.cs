using Core.IO.ResourceFormats.Havok.Headers;
using Core.IO.Streams;
using Object = Core.IO.ResourceFormats.Havok.Utils.Object;

namespace Core.IO.ResourceFormats.Havok.Classes;

public class SplineCompressedAnimation : IClass
{
    public static IClass Deserialize(Stream stream, Endian endian, Header header, Object @object)
    {
        return new SplineCompressedAnimation();
    }
}