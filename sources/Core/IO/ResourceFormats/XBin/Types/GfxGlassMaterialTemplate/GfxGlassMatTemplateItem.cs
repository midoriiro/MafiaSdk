namespace Core.IO.ResourceFormats.XBin.Types.GfxGlassMaterialTemplate;

public class GfxGlassMatTemplateItem
{
    public uint Id { get; init; }
    public uint OriginalTemplatePart0 { get; init; }
    public uint OriginalTemplatePart1 { get; init; }
    public uint DamagedTemplatePart0 { get; init; }
    public uint DamagedTemplatePart1 { get; init; }
    public int Type { get; init; }
    public uint GlassBreakType { get; init; }
    public string Description { get; init; } = null!;

    internal GfxGlassMatTemplateItem()
    {
    }

    public override string ToString()
    {
        return $"ID = {Id}";
    }
}