using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.GfxGlassBreakType
{
    public class GfxGlassBreakTypeTable : ITable
    {
        public List<GfxGlassBreakTypeItem> Items { get; init; } = null!;

        private GfxGlassBreakTypeTable()
        {
        }

        public static ITable ReadFromFile(BinaryReader reader)
        {
            uint count0 = reader.ReadUInt32();
            uint count1 = reader.ReadUInt32();
            
            if (count0 != count1)
            {
                throw new InvalidDataException(
                    $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
                );
            }
            
            var items = new GfxGlassBreakTypeItem[count1];

            for (var i = 0; i < items.Length; i++)
            {
                uint id = reader.ReadUInt32();
                string typeName = reader.ReadStringBuffer(32);
                string mixedTex = reader.ReadStringBuffer(32);
                string spiderTex = reader.ReadStringBuffer(32);
                uint materialGuidPart0 = reader.ReadUInt32();
                uint materialGuidPart1 = reader.ReadUInt32();
                int defence = reader.ReadInt32();
                int optimalScale = reader.ReadInt32();
                int worstScale = reader.ReadInt32();
                int dynamicBreakPower = reader.ReadInt32();
                int dynamicCrackPower = reader.ReadInt32();
                int fragmentDisappearLimit = reader.ReadInt32();
                int spiderRnd = reader.ReadInt32();
                int spidersLimitMin = reader.ReadInt32();
                int spidersLimitMax = reader.ReadInt32();
                float spiderSize = reader.ReadSingle();
                int fragmentConnectionLimit = reader.ReadInt32();
                int cracksLimitMin = reader.ReadInt32();
                int cracksLimitMax = reader.ReadInt32();
                int cracksLimitPerHit = reader.ReadInt32();
                int cracksDamagePerPiece = reader.ReadInt32();
                int crackCreateRnd = reader.ReadInt32();
                int sndSpider = reader.ReadInt32();
                int sndSpiderCategory = reader.ReadInt32();
                int sndDestruct = reader.ReadInt32();
                int sndDestructCategory = reader.ReadInt32();
                int sndLargeDestruct = reader.ReadInt32();
                int sndLargeDestructCategory = reader.ReadInt32();
                int ptcSpider = reader.ReadInt32();
                int ptcFragment = reader.ReadInt32();
                int ptcMultiGlass = reader.ReadInt32();
                bool canDropShards = reader.ReadBoolean();
                bool genHumanHole = reader.ReadBoolean();
                bool unknown1 = reader.ReadBoolean();
                bool unknown2 = reader.ReadBoolean();
                float manHoleHeight = reader.ReadSingle();
                float manHoleWidth = reader.ReadSingle();
                float dmgForDestruction = reader.ReadSingle();
                
                items[i] = new GfxGlassBreakTypeItem()
                {
                    Id = id,
                    TypeName = typeName,
                    MixedTex = mixedTex,
                    SpiderTex = spiderTex,
                    MaterialGuidPart0 = materialGuidPart0,
                    MaterialGuidPart1 = materialGuidPart1,
                    Defence = defence,
                    OptimalScale = optimalScale,
                    WorstScale = worstScale,
                    DynamicBreakPower = dynamicBreakPower,
                    DynamicCrackPower = dynamicCrackPower,
                    FragmentDisappearLimit = fragmentDisappearLimit,
                    SpiderRnd = spiderRnd,
                    SpidersLimitMin = spidersLimitMin,
                    SpidersLimitMax = spidersLimitMax,
                    SpiderSize = spiderSize,
                    FragmentConnectionLimit = fragmentConnectionLimit,
                    CracksLimitMin = cracksLimitMin,
                    CracksLimitMax = cracksLimitMax,
                    CracksLimitPerHit = cracksLimitPerHit,
                    CracksDamagePerPiece = cracksDamagePerPiece,
                    CrackCreateRnd = crackCreateRnd,
                    SndSpider = sndSpider,
                    SndSpiderCategory = sndSpiderCategory,
                    SndDestruct = sndDestruct,
                    SndDestructCategory = sndDestructCategory,
                    SndLargeDestruct = sndLargeDestruct,
                    SndLargeDestructCategory = sndLargeDestructCategory,
                    PtcSpider = ptcSpider,
                    PtcFragment = ptcFragment,
                    PtcMultiGlass = ptcMultiGlass,
                    CanDropShards = canDropShards,
                    GenHumanHole = genHumanHole,
                    Unknown1 = unknown1,
                    Unknown2 = unknown2,
                    ManHoleHeight = manHoleHeight,
                    ManHoleWidth = manHoleWidth,
                    DmgForDestruction = dmgForDestruction
                };
            }

            return new GfxGlassBreakTypeTable()
            {
                Items = items.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            int itemsCount = Items.Count;
            
            writer.Write(itemsCount);
            writer.Write(itemsCount);

            for (var i = 0; i < itemsCount; i++)
            {
                GfxGlassBreakTypeItem item = Items[i];
                writer.Write(item.Id);
                writer.WriteStringBuffer(32, item.TypeName);
                writer.WriteStringBuffer(32, item.MixedTex);
                writer.WriteStringBuffer(32, item.SpiderTex);
                writer.Write(item.MaterialGuidPart0);
                writer.Write(item.MaterialGuidPart1);
                writer.Write(item.Defence);
                writer.Write(item.OptimalScale);
                writer.Write(item.WorstScale);
                writer.Write(item.DynamicBreakPower);
                writer.Write(item.DynamicCrackPower);
                writer.Write(item.FragmentDisappearLimit);
                writer.Write(item.SpiderRnd);
                writer.Write(item.SpidersLimitMin);
                writer.Write(item.SpidersLimitMax);
                writer.Write(item.SpiderSize);
                writer.Write(item.FragmentConnectionLimit);
                writer.Write(item.CracksLimitMin);
                writer.Write(item.CracksLimitMax);
                writer.Write(item.CracksLimitPerHit);
                writer.Write(item.CracksDamagePerPiece);
                writer.Write(item.CrackCreateRnd);
                writer.Write(item.SndSpider);
                writer.Write(item.SndSpiderCategory);
                writer.Write(item.SndDestruct);
                writer.Write(item.SndDestructCategory);
                writer.Write(item.SndLargeDestruct);
                writer.Write(item.SndLargeDestructCategory);
                writer.Write(item.PtcSpider);
                writer.Write(item.PtcFragment);
                writer.Write(item.PtcMultiGlass);
                writer.Write(item.CanDropShards);
                writer.Write(item.GenHumanHole);
                writer.Write(item.Unknown1);
                writer.Write(item.Unknown2);
                writer.Write(item.ManHoleHeight);
                writer.Write(item.ManHoleWidth);
                writer.Write(item.DmgForDestruction);
            }
        }
    }
}
