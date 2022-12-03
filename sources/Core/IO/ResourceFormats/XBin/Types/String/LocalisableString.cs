using Core.IO.ResourceFormats.XBin.CoreTypes;

namespace Core.IO.ResourceFormats.XBin.Types.String;

public class LocalisableString
{
    public uint Unk0 { get; internal init; } // I'm guessing this used to be a pointer to the stringID.
    public XBinHash64Name StringId { get; init; } = null!; // Hash of the StringID which no longer exists. *cry ever tim*
    public string Content { get; init; } = null!; // Text

    internal LocalisableString()
    {
    }

    public override string ToString()
    {
        return Content;
    }
}