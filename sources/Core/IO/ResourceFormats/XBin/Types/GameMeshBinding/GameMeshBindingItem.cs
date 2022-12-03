namespace Core.IO.ResourceFormats.XBin.Types.GameMeshBinding;

public class GameMeshBindingItem
{
    public ulong NameHash { get; init; }
    public ulong SingleMeshIndex { get; init; }
    public ulong HavokIndex { get; init; }

    internal GameMeshBindingItem()
    {
    }
    
    public override string ToString()
    {
        return $"SingleMeshIndex = {SingleMeshIndex} HavokIndex = {HavokIndex}";
    }
}