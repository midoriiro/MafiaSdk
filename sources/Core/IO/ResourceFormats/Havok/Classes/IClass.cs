using Core.IO.ResourceFormats.Havok.Headers;
using Core.IO.Streams;
using Object = Core.IO.ResourceFormats.Havok.Utils.Object;

namespace Core.IO.ResourceFormats.Havok.Classes;

public interface IClass
{
    static abstract IClass Deserialize(Stream stream, Endian endian, Header header, Object @object);

    string GetHavokClassName()
    {
        return $"hk{GetType().Name}";
    }
}