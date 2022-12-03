namespace Core.IO.ResourceFormats.XBin.Types.CharacterCinematics;

public class CharacterCinematicsItem
{
    public ulong StringId { get; init; }
    public ulong CharacterId { get; init; }

    internal CharacterCinematicsItem()
    {
    }
    
    public override string ToString()
    {
        return $"StringId = {StringId}";
    }
}