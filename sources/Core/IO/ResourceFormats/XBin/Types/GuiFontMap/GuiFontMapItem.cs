using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.GuiFontMap;

public class GuiFontMapItem
{
    public int Id { get; init; }
    public string Alias { get; init; } = null!;
    public string Name { get; init; } = null!;
    public EFontMapFlags Flags { get; init; }
    public float Scale { get; init; }
    public float OffsetX { get; init; }
    public float OffsetY { get; init; }
    public EFontMapPlatform Platform { get; init; }
    public EFontMapLanguage Language { get; init; }

    internal GuiFontMapItem()
    {
    }

    public override string ToString()
    {
        return $"ID = {Id}";
    }
}