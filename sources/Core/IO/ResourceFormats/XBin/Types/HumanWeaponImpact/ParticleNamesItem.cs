using Core.IO.ResourceFormats.XBin.CoreTypes;

namespace Core.IO.ResourceFormats.XBin.Types.HumanWeaponImpact;

public class ParticleNamesItem
{
    public XBinHash64Name InitialShot { get; init; } = null!;
    public XBinHash64Name Headshot { get; init; } = null!;
    public XBinHash64Name GrazingShot { get; init; } = null!;
    public XBinHash64Name KillShot { get; init; } = null!;
    public float SplashDiameter { get; init; }
    public float SplashHardness { get; init; }
    public float SplashStrength { get; init; }
    public float ShotDiameter { get; init; }
    public float ShotHardness { get; init; }
    public float ShotStrength { get; init; }

    internal ParticleNamesItem()
    {
    }
}