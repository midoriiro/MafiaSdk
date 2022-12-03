using Core.IO.ResourceFormats.Extensions;
using Core.IO.ResourceFormats.XBin.CoreTypes;

namespace Core.IO.ResourceFormats.XBin.Types.HealthSystem
{
    public class HealthSystemItem
    {
        public HealthSegment[] HealthSegments { get; init; } = null!;
        public XBinVector2[] DamageLimiterGraph { get; init; } = null!;
        public XBinVector2[] DangerBarDistanceMultipliers { get; init; } = null!;
        public float DefaultRegeneration { get; init; }
        public float DefaultCriticalSize { get; init; }
        public float DefaultDamageTimeout { get; init; }
        public float DefaultSize { get; init; }
        public bool DefaultDamageGoesOver { get; init; }
        public float DefaultNoDamageTimeAfterSegmentDown { get; init; }
        public string Name { get; init; } = null!;
        public float RegenerationOnTakedownAmount { get; init; }
        public float RegenerationOnTakedownTime { get; init; }
        public bool RegenerationOnTakedownExceedSegment { get; init; }
        public ushort SegmentsCountToChangeColor { get; init; }
        public XBinHash64Name DefaultEffectName { get; init; } = null!;
        public float DefaultEffectMaxOpacity { get; init; }
        public float DangerBarSize { get; init; }
        public float DangerBarRegenerationDelay { get; init; }
        public float DangerBarRegenerationTime { get; init; }
        public float LethalDamageClampMultiplier { get; init; }
        public float AfterClampNoDamageTime { get; init; }

        internal HealthSystemItem()
        {
        }

        public static HealthSystemItem ReadFromFile(BinaryReader reader)
        {
            reader.ReadUInt32(); // offset
            uint healthSegmentsCount0 = reader.ReadUInt32();
            uint healthSegmentsCount1 = reader.ReadUInt32();
            
            if (healthSegmentsCount0 != healthSegmentsCount1)
            {
                throw new InvalidDataException(
                    $"Numbers should be identical: Count0 = {healthSegmentsCount0}, Count1 = {healthSegmentsCount1}"
                );
            }
            
            var healthSegments = new HealthSegment[healthSegmentsCount0];

            reader.ReadUInt32(); // offset
            uint damageLimiterGraphCount0 = reader.ReadUInt32();
            uint damageLimiterGraphCount1 = reader.ReadUInt32();
            
            if (damageLimiterGraphCount0 != damageLimiterGraphCount1)
            {
                throw new InvalidDataException(
                    $"Numbers should be identical: Count0 = {damageLimiterGraphCount0}, Count1 = {damageLimiterGraphCount1}"
                );
            }
            
            var damageLimiterGraph = new XBinVector2[damageLimiterGraphCount0];

            reader.ReadUInt32(); // offset
            uint dangerBarDistanceCount0 = reader.ReadUInt32();
            uint dangerBarDistanceCount1 = reader.ReadUInt32();
            
            if (dangerBarDistanceCount0 != dangerBarDistanceCount1)
            {
                throw new InvalidDataException(
                    $"Numbers should be identical: Count0 = {dangerBarDistanceCount0}, Count1 = {dangerBarDistanceCount1}"
                );
            }
            
            var dangerBarDistanceMultipliers = new XBinVector2[dangerBarDistanceCount0];

            float defaultRegeneration = reader.ReadSingle();
            float defaultCriticalSize = reader.ReadSingle();
            float defaultDamageTimeout = reader.ReadSingle();
            float defaultSize = reader.ReadSingle();
            var defaultDamageGoesOver = Convert.ToBoolean(reader.ReadInt32());
            float defaultNoDamageTimeAfterSegmentDown = reader.ReadSingle();
            string name = reader.ReadStringBuffer(32).TrimEnd('\0');
            float regenerationOnTakedownAmount = reader.ReadSingle();
            float regenerationOnTakedownTime = reader.ReadSingle();
            var regenerationOnTakedownExceedSegment = Convert.ToBoolean(reader.ReadInt16());
            ushort segmentsCountToChangeColor = reader.ReadUInt16();
            XBinHash64Name defaultEffectName = XBinHash64Name.ReadFromFile(reader);
            float defaultEffectMaxOpacity = reader.ReadSingle(); // TODO refactor all max/min to long shape
            float dangerBarSize = reader.ReadSingle();
            float dangerBarRegenerationDelay = reader.ReadSingle();
            float dangerBarRegenerationTime = reader.ReadSingle();
            float lethalDamageClampMultiplier = reader.ReadSingle();
            float afterClampNoDamageTime = reader.ReadSingle();

            return new HealthSystemItem()
            {
                HealthSegments = healthSegments,
                DamageLimiterGraph = damageLimiterGraph,
                DangerBarDistanceMultipliers = dangerBarDistanceMultipliers,
                DefaultRegeneration = defaultRegeneration,
                DefaultCriticalSize = defaultCriticalSize,
                DefaultDamageTimeout = defaultDamageTimeout,
                DefaultSize = defaultSize,
                DefaultDamageGoesOver = defaultDamageGoesOver,
                DefaultNoDamageTimeAfterSegmentDown = defaultNoDamageTimeAfterSegmentDown,
                Name = name,
                RegenerationOnTakedownAmount = regenerationOnTakedownAmount,
                RegenerationOnTakedownTime = regenerationOnTakedownTime,
                RegenerationOnTakedownExceedSegment = regenerationOnTakedownExceedSegment,
                SegmentsCountToChangeColor = segmentsCountToChangeColor,
                DefaultEffectName = defaultEffectName,
                DefaultEffectMaxOpacity = defaultEffectMaxOpacity,
                DangerBarSize = dangerBarSize,
                DangerBarRegenerationDelay = dangerBarRegenerationDelay,
                DangerBarRegenerationTime = dangerBarRegenerationTime,
                LethalDamageClampMultiplier = lethalDamageClampMultiplier,
                AfterClampNoDamageTime = afterClampNoDamageTime
            };
        }

        public void ReadHealthSegments(BinaryReader reader)
        {
            for (var i = 0; i < HealthSegments.Length; i++)
            {
                HealthSegments[i] = HealthSegment.ReadFromFile(reader);
            }

            foreach (HealthSegment healthSegment in HealthSegments)
            {
                healthSegment.ReadArrayData(reader);
            }

            for (var i = 0; i < DamageLimiterGraph.Length; i++)
            {
                DamageLimiterGraph[i] = XBinVector2.ReadFromFile(reader);
            }

            for (var i = 0; i < DangerBarDistanceMultipliers.Length; i++)
            {
                DangerBarDistanceMultipliers[i] = XBinVector2.ReadFromFile(reader);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}