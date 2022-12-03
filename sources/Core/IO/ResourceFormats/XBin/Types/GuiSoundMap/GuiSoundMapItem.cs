using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.GuiSoundMap;

public class GuiSoundMapItem
{
    public int Id { get; init; }
    public ESoundEvent SoundEvent { get; init; }
    public uint WwiseEvent { get; init; }

    internal GuiSoundMapItem()
    {
    }

    public override string ToString()
    {
        return $"ID = {Id}";
    }
}