using Core.IO.ResourceFormats.XBin.CoreTypes;

namespace Core.IO.ResourceFormats.XBin.Types.Subtitle;

public class SubtitleTableItem
{
    public XBinHash32Name SubtitleId { get; init; } = null!;
    public XBinHash32Name SoundId { get; init; } = null!;
    public XBinHash64Name FacialAnimationName { get; init; } = null!;
    public XBinHash64Name LongStringId { get; init; } = null!;
    public XBinHash64Name ShortStringId { get; init; } = null!;
    public uint SoundPreset { get; init; }
    public XBinHash32Name VoicePresetOverride { get; init; } = null!;
    public uint SubtitlePriorityOverride { get; init; }
    public uint Unk0 { get; init; }
    public XBinHash64Name SubtitleCharacter { get; init; } = null!;

    internal SubtitleTableItem()
    {
    }

    public override string ToString()
    {
        return LongStringId.ToString();
    }
}