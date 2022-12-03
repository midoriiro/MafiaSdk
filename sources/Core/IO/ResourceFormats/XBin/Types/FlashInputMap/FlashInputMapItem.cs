using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.FlashInputMap;

public class FlashInputMapItem
{
    public int Id { get; init; }
    public EKeyModifiers KeyModifiers { get; init; }
    public EMenuAction MenuAction { get; init; }
    public EFlashControlType FlashControlType { get; init; }
    public EInputControlUnified Control { get; init; }
    public EAxisMode AxisMode { get; init; }
    public EFlashDeviceIndex DeviceIndex { get; init; }

    internal FlashInputMapItem()
    {
    }

    public override string ToString()
    {
        return $"ID = {Id}";
    }
}