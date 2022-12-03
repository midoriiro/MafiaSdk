namespace Core.IO.FileFormats.SDS.Resource.Streams.Helpers;

public static class MemorySize
{
    public static long From(uint value, MemoryUnit unit)
    {
        return unit switch
        {
            MemoryUnit.Byte => value,
            MemoryUnit.Kilo => value * 1024,
            MemoryUnit.Mega => value * 1024 * 1024,
            MemoryUnit.Giga => value * 1024 * 1024 * 1024,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };
    }
}