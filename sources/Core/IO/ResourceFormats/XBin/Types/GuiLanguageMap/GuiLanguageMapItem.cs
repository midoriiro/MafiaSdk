namespace Core.IO.ResourceFormats.XBin.Types.GuiLanguageMap;

public class GuiLanguageMapItem
{
    public string LanguageCode { get; init; } = null!;
    public uint DisplayNameOffset { get; init; }
    public string DisplayName { get; internal set; } = null!; // utf-8 ?
    public int HasAudioLayer { get; init; } // bool ?

    internal GuiLanguageMapItem()
    {
    }

    public override string ToString()
    {
        return $"{DisplayName}";
    }
}