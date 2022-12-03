using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Havok.Extensions;

public static class StreamExtensions
{
    public static long AssertPointer(this Stream stream, Endian endian, int pointerSize)
    {
        long offset = stream.Position;

        switch (pointerSize)
        {
            case 4:
                stream.ReadValueU32(endian).AssertValue(0U);
                break;
            case 8:
                stream.ReadValueU64(endian).AssertValue(0UL);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(pointerSize), 
                    "Cannot determine pointer size"
                );
        }

        return offset;
    }

    public static uint ReadCounter(this Stream stream, Endian endian)
    {
        uint count = stream.ReadValueU32(endian);
        stream.ReadValueU32();
        return count;
    }
}