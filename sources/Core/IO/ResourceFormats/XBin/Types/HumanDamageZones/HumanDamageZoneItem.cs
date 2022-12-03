using Core.IO.ResourceFormats.Extensions;
using Core.IO.ResourceFormats.XBin.CoreTypes;

namespace Core.IO.ResourceFormats.XBin.Types.HumanDamageZones;

public class HumanDamageZoneItem
{
    // ordered id - probably needs to be ordered 
    // or game may crash
    public uint ItemId { get; init; }

    // actual data
    public StunGroup[] StunGroups { get; init; } = null!;
    public string DamageZoneName { get; init; } = null!;
    public int Health { get; init; }
    public float DamageMultiplexBody { get; init; }
    public float ArmorBodyValue { get; init; }
    public float SneakDamageBodyMultiplier { get; init; }
    public float SneakDamageLowerBodyMultiplier { get; init; }
    public float DamageMultiplexHead { get; init; }
    public float ArmorHeadValue { get; init; }
    public float SneakDamageHeadMultiplier { get; init; }
    public float DamageMultiplexHands { get; init; }
    public float ArmorHandsValue { get; init; }
    public float SneakDamageHandsMultiplier { get; init; }
    public float DamageMultiplexLegs { get; init; }
    public float ArmorLegsValue { get; init; }
    public float SneakDmgLegsMultiplier { get; init; }
    public float EachHitStunHealthLevelThreshold { get; init; }
    public float SingleHitStunHealthThreshold { get; init; }
    public XBinHash64Name WeaponImpactGroupName { get; init; } = null!;

    internal HumanDamageZoneItem()
    {
    }

    public static HumanDamageZoneItem ReadFromFile(BinaryReader reader)
    {
        uint itemId = reader.ReadUInt32();
                
        reader.ReadUInt32(); // offset
        uint count0 = reader.ReadUInt32();
        uint count1 = reader.ReadUInt32();
        
        if (count0 != count1)
        {
            throw new InvalidDataException(
                $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
            );
        }

        var stunGroups = new StunGroup[count0];
        string damageZoneName = reader.ReadStringBuffer(32).Trim('\0');
        int health = reader.ReadInt32();
        float damageMultiplexBody = reader.ReadSingle();
        float armorBodyValue = reader.ReadSingle();
        float sneakDamageBodyMultiplier = reader.ReadSingle();
        float sneakDamageLowerBodyMultiplier = reader.ReadSingle();
        float damageMultiplexHead = reader.ReadSingle();
        float armorHeadValue = reader.ReadSingle();
        float sneakDamageHeadMultiplier = reader.ReadSingle();
        float armorHandsValue = reader.ReadSingle();
        float sneakDmgHandsMultiplier = reader.ReadSingle();
        float damageMultiplexLegs = reader.ReadSingle();
        float armorLegsValue = reader.ReadSingle();
        float sneakDmgLegsMultiplier = reader.ReadSingle();
        float damageMultiplexHands = reader.ReadSingle();
        float eachHitStunHealthLevelThreshold = reader.ReadSingle();
        float singleHitStunHealthThreshold = reader.ReadSingle();
        XBinHash64Name weaponImpactGroupName = XBinHash64Name.ReadFromFile(reader);
        
        return new HumanDamageZoneItem()
        {
            ItemId = itemId,
            
            StunGroups = stunGroups,
            DamageZoneName = damageZoneName,
            Health = health,
            DamageMultiplexBody = damageMultiplexBody,
            ArmorBodyValue = armorBodyValue,
            SneakDamageBodyMultiplier = sneakDamageBodyMultiplier,
            SneakDamageLowerBodyMultiplier = sneakDamageLowerBodyMultiplier,
            DamageMultiplexHead = damageMultiplexHead,
            ArmorHeadValue = armorHeadValue,
            SneakDamageHeadMultiplier = sneakDamageHeadMultiplier, 
            ArmorHandsValue = armorHandsValue,
            SneakDamageHandsMultiplier = sneakDmgHandsMultiplier,
            DamageMultiplexLegs = damageMultiplexLegs,
            ArmorLegsValue = armorLegsValue,
            SneakDmgLegsMultiplier = sneakDmgLegsMultiplier,
            DamageMultiplexHands = damageMultiplexHands,
            EachHitStunHealthLevelThreshold = eachHitStunHealthLevelThreshold,
            SingleHitStunHealthThreshold = singleHitStunHealthThreshold,
            WeaponImpactGroupName = weaponImpactGroupName
        };
    }
}