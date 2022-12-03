using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.HumanDamageZones;

public class StunGroup
{
    // actual data
    public EBodyPartType[] BodyPartList { get; internal set; } = null!;
    public float DamageToStun { get; init; }
    public float DamageTimeWindow { get; init; }

    internal StunGroup()
    {
    }

    public static StunGroup ReadFromFile(BinaryReader reader)
    {
        reader.ReadUInt32(); // offset
        uint count0 = reader.ReadUInt32();
        uint count1 = reader.ReadUInt32();
        
        if (count0 != count1)
        {
            throw new InvalidDataException(
                $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
            );
        }

        var bodyPartList = new EBodyPartType[count0];
        float damageToStun = reader.ReadSingle();
        float damageTimeWindow = reader.ReadSingle();
                    
        return new StunGroup()
        {
            BodyPartList = bodyPartList,
            DamageToStun = damageToStun,
            DamageTimeWindow = damageTimeWindow
        };
    }
}