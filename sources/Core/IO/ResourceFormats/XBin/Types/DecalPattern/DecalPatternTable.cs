using Core.IO.ResourceFormats.Extensions;
using Core.IO.ResourceFormats.XBin.Enumerators;

namespace Core.IO.ResourceFormats.XBin.Types.DecalPattern
{
    public class DecalPatternTable : ITable
    {
        public uint Unk0 { get; init; }
        public List<DecalPatternItem> Items { get; init; } = null!;

        private DecalPatternTable()
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
            
            uint unk0 = reader.ReadUInt32();
            var items = new DecalPatternItem[count1];

            for (var i = 0; i < items.Length; i++)
            {
                long id = reader.ReadInt64();
                float uvMinX = reader.ReadSingle();
                float uvMinY = reader.ReadSingle();
                float uvMaxX = reader.ReadSingle();
                float uvMaxY = reader.ReadSingle();
                uint materialGuidPart0 = reader.ReadUInt32();
                uint materialGuidPart1 = reader.ReadUInt32();
                float minRadius = reader.ReadSingle();
                float maxRadius = reader.ReadSingle();
                var flags = (EDecalFlags)reader.ReadUInt32();
                float impact = reader.ReadSingle();
                int texCols = reader.ReadInt32();
                int texRows = reader.ReadInt32();
                int texStart = reader.ReadInt32();
                int texEnd = reader.ReadInt32();
                uint group = reader.ReadUInt32();
                int multiDecal = reader.ReadInt32();
                float blendTime = reader.ReadSingle();
                int footStep = reader.ReadInt32();
                string notes = reader.ReadStringBuffer(32);
                
                items[i] = new DecalPatternItem()
                {
                    Id = id,
                    UvMinX = uvMinX,
                    UvMinY = uvMinY,
                    UvMaxX = uvMaxX,
                    UvMaxY = uvMaxY,
                    MaterialGuidPart0 = materialGuidPart0,
                    MaterialGuidPart1 = materialGuidPart1,
                    MinRadius = minRadius,
                    MaxRadius = maxRadius,
                    Flags = flags,
                    Impact = impact,
                    TexCols = texCols,
                    TexRows = texRows,
                    TexStart = texStart,
                    TexEnd = texEnd,
                    Group = group,
                    MultiDecal = multiDecal,
                    BlendTime = blendTime,
                    FootStep = footStep,
                    Notes = notes
                };
            }

            return new DecalPatternTable()
            {
                Unk0 = unk0,
                Items = items.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            int itemsCount = Items.Count;
            
            writer.Write(itemsCount);
            writer.Write(itemsCount);
            writer.Write(Unk0);

            for (var i = 0; i < itemsCount; i++)
            {
                DecalPatternItem item = Items[i];
                writer.Write(item.Id);
                writer.Write(item.UvMinX);
                writer.Write(item.UvMinY);
                writer.Write(item.UvMaxX);
                writer.Write(item.UvMaxY);
                writer.Write(item.MaterialGuidPart0);
                writer.Write(item.MaterialGuidPart1);
                writer.Write(item.MinRadius);
                writer.Write(item.MaxRadius);
                writer.Write((uint)item.Flags);
                writer.Write(item.Impact);
                writer.Write(item.TexCols);
                writer.Write(item.TexRows);
                writer.Write(item.TexStart);
                writer.Write(item.TexEnd);
                writer.Write(item.Group);
                writer.Write(item.MultiDecal);
                writer.Write(item.BlendTime);
                writer.Write(item.FootStep);
                writer.WriteStringBuffer(32, item.Notes);
            }
        }
    }
}
