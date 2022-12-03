using Core.IO.ResourceFormats.XBin.CoreTypes;
using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.HumanMaterials;

public class HumanMaterialsItem
{
    public uint Id { get; init; }
    public string MaterialName { get; init; } = null!;
    public EHumanMaterialsTableItemFlags Flags { get; init; }
    public XBinHash32Name SoundMaterialSwitch { get; init; } = null!;
    public uint StepParticles { get; init; }

    internal HumanMaterialsItem()
    {
    }

    public override string ToString()
    {
        return $"{Id} {MaterialName} {Flags}";
    }
}