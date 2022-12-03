namespace Core.IO.ResourceFormats.XBin.Types.GenericSpeechSituations;

public class GenericSpeechSituationsItem
{
    public ulong SituationId { get; init; }
    public uint Cooldown { get; init; }
    public uint PerActorCooldownMin { get; init; }
    public uint PerActorCooldownMax { get; init; }
    public uint PerTeamCooldownMin { get; init; }
    public uint PerTeamCooldownMax { get; init; }
    public uint IsForeground { get; init; } //bool
    public ulong InitHash { get; init; }

    internal GenericSpeechSituationsItem()
    {
    }

    public override string ToString()
    {
        return $"Situation = {SituationId}";
    }
}