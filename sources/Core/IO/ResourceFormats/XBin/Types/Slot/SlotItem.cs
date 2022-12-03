using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.Slot;

public class SlotItem
{
    public int TypeId { get; init; }
    public ESlotType SlotType { get; init; }
    public int BaseNameOffset { get; init; }
    public string BaseName { get; internal set; } = null!;
    public uint RamWindows { get; init; }
    public uint VramWindows { get; init; }
    public uint RamXbox360 { get; init; }
    public uint VramXbox360 { get; init; }
    public uint RamPs3Devkit { get; init; }
    public uint VramPs3Devkit { get; init; }
    public uint RamPs3Testkit { get; init; }
    public uint VramPs3Testkit { get; init; }

    internal SlotItem()
    {
    }

    public override string ToString()
    {
        return $"{BaseName}";
    }
}