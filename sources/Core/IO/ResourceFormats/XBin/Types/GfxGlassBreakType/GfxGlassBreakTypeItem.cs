namespace Core.IO.ResourceFormats.XBin.Types.GfxGlassBreakType;

public class GfxGlassBreakTypeItem
{
    public uint Id { get; init; }
    public string TypeName { get; init; } = null!;
    public string MixedTex { get; init; } = null!;
    public string SpiderTex { get; init; } = null!;
    public uint MaterialGuidPart0 { get; init; }
    public uint MaterialGuidPart1 { get; init; }
    public int Defence { get; init; }
    public int OptimalScale { get; init; }
    public int WorstScale { get; init; }
    public int DynamicBreakPower { get; init; }
    public int DynamicCrackPower { get; init; }
    public int FragmentDisappearLimit { get; init; }
    public int SpiderRnd { get; init; }
    public int SpidersLimitMin { get; init; }
    public int SpidersLimitMax { get; init; }
    public float SpiderSize { get; init; }
    public int FragmentConnectionLimit { get; init; }
    public int CracksLimitMin { get; init; }
    public int CracksLimitMax { get; init; }
    public int CracksLimitPerHit { get; init; }
    public int CracksDamagePerPiece { get; init; }
    public int CrackCreateRnd { get; init; }
    public int SndSpider { get; init; }
    public int SndSpiderCategory { get; init; }
    public int SndDestruct { get; init; }
    public int SndDestructCategory { get; init; }
    public int SndLargeDestruct { get; init; }
    public int SndLargeDestructCategory { get; init; }
    public int PtcSpider { get; init; }
    public int PtcFragment { get; init; }
    public int PtcMultiGlass { get; init; }
    public bool CanDropShards { get; init; }
    public bool GenHumanHole { get; init; }
    public bool Unknown1 { get; init; }
    public bool Unknown2 { get; init; }
    public float ManHoleHeight { get; init; }
    public float ManHoleWidth { get; init; }
    public float DmgForDestruction { get; init; }

    internal GfxGlassBreakTypeItem()
    {
    }

    public override string ToString()
    {
        return $"ID = {Id}";
    }
}