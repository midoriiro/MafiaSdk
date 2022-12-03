using Core.IO.ResourceFormats.XBin.CoreTypes;

namespace Core.IO.ResourceFormats.XBin.Types.HealthSystem;

public class HealthSegment
{
    public float[] Regeneration { get; init; } = null!;
    public float[] DamageTimeout { get; init; } = null!;
    public float[] Size { get; init; } = null!;
    public bool[] DamageGoesOver { get; init; } = null!;
    public float[] NoDamageTimeAfterSegmentDown { get; init; } = null!;
    public XBinHash64Name[] EffectName { get; init; } = null!;
    public float[] EffectMaxOpacity { get; init; } = null!;
    public bool IsArmour { get; init; }

    internal HealthSegment()
    {
    }
    public static HealthSegment ReadFromFile(BinaryReader reader)
    {
        reader.ReadUInt32(); // offset
        uint regenerationCount0 = reader.ReadUInt32();
        uint regenerationCount1 = reader.ReadUInt32();
        
        if (regenerationCount0 != regenerationCount1)
        {
            throw new InvalidDataException(
                $"Numbers should be identical: Count0 = {regenerationCount0}, Count1 = {regenerationCount1}"
            );
        }
        
        var regeneration = new float[regenerationCount0];

        reader.ReadUInt32(); // offset
        uint damageTimeoutCount0 = reader.ReadUInt32();
        uint damageTimeoutCount1 = reader.ReadUInt32();
        
        if (damageTimeoutCount0 != damageTimeoutCount1)
        {
            throw new InvalidDataException(
                $"Numbers should be identical: Count0 = {damageTimeoutCount0}, Count1 = {damageTimeoutCount1}"
            );
        }
        
        var damageTimeout = new float[damageTimeoutCount0];

        reader.ReadUInt32(); // offset
        uint sizeCount0 = reader.ReadUInt32();
        uint sizeCount1 = reader.ReadUInt32();
        
        if (sizeCount0 != sizeCount1)
        {
            throw new InvalidDataException(
                $"Numbers should be identical: Count0 = {sizeCount0}, Count1 = {sizeCount1}"
            );
        }
        
        var size = new float[sizeCount0];

        reader.ReadUInt32(); // offset
        uint damageGoesOverCount0 = reader.ReadUInt32();
        uint damageGoesOverCount1 = reader.ReadUInt32();
        
        if (damageGoesOverCount0 != damageGoesOverCount1)
        {
            throw new InvalidDataException(
                $"Numbers should be identical: Count0 = {damageGoesOverCount0}, Count1 = {damageGoesOverCount1}"
            );
        }
        
        var damageGoesOver = new bool[damageGoesOverCount0];

        reader.ReadUInt32(); // offset
        uint noDamageTimeAfterSegmentDownCount0 = reader.ReadUInt32();
        uint noDamageTimeAfterSegmentDownCount1 = reader.ReadUInt32();
        
        if (noDamageTimeAfterSegmentDownCount0 != noDamageTimeAfterSegmentDownCount1)
        {
            throw new InvalidDataException(
                $"Numbers should be identical: Count0 = {noDamageTimeAfterSegmentDownCount0}, Count1 = {noDamageTimeAfterSegmentDownCount1}"
            );
        }
        
        var noDamageTimeAfterSegmentDown = new float[noDamageTimeAfterSegmentDownCount0];

        reader.ReadUInt32(); // offset
        uint effectNameCount0 = reader.ReadUInt32();
        uint effectNameCount1 = reader.ReadUInt32();
        
        if (effectNameCount0 != effectNameCount1)
        {
            throw new InvalidDataException(
                $"Numbers should be identical: Count0 = {effectNameCount0}, Count1 = {effectNameCount1}"
            );
        }
        
        var effectName = new XBinHash64Name[effectNameCount0];

        reader.ReadUInt32(); // offset
        uint effectMaxOpacityCount0 = reader.ReadUInt32();
        uint effectMaxOpacityCount1 = reader.ReadUInt32();
        
        if (effectMaxOpacityCount0 != effectMaxOpacityCount1)
        {
            throw new InvalidDataException(
                $"Numbers should be identical: Count0 = {effectMaxOpacityCount0}, Count1 = {effectMaxOpacityCount1}"
            );
        }
        
        var effectMaxOpacity = new float[effectMaxOpacityCount0];

        var isArmour = Convert.ToBoolean(reader.ReadUInt32());

        return new HealthSegment()
        {
            Regeneration = regeneration,
            DamageTimeout = damageTimeout,
            Size = size,
            DamageGoesOver = damageGoesOver,
            NoDamageTimeAfterSegmentDown = noDamageTimeAfterSegmentDown,
            EffectName = effectName,
            EffectMaxOpacity = effectMaxOpacity,
            IsArmour = isArmour
        };
    }

    public void ReadArrayData(BinaryReader reader)
    {
        // TODO read data from file
        for (var i = 0; i < Regeneration.Length; i++)
        {

        }

        for (var i = 0; i < DamageTimeout.Length; i++)
        {

        }

        for (var i = 0; i < Size.Length; i++)
        {
            Size[i] = reader.ReadSingle();
        }

        for (var i = 0; i < DamageGoesOver.Length; i++)
        {

        }

        for (var i = 0; i < NoDamageTimeAfterSegmentDown.Length; i++)
        {

        }

        for (var i = 0; i < EffectName.Length; i++)
        { ;
            EffectName[i] = XBinHash64Name.ReadFromFile(reader);
        }

        for (var i = 0; i < EffectMaxOpacity.Length; i++)
        {
            EffectMaxOpacity[i] = reader.ReadSingle();
        }
    }

    public override string ToString()
    {
        return "Health Segment";
    }
}