using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.GuiInputMap;

public class GuiInputMapItem
{
    public int Id { get; init; }
    public EInputControlFlags ControlMode { get; init; }
    public EKeyModifiers KeyModifiers { get; init; }
    public EInputDeviceType DeviceType { get; init; }
    public EInputControlUnified Control { get; init; }
    public EControllerType ControlPriority { get; init; }
    public EMenuAction MenuAction { get; init; }

    internal GuiInputMapItem()
    {
    }

    public override string ToString()
    {
        return $"ID = {Id}";
    }
}