using Core.IO.ResourceFormats.XBin.Enumerators;
using Core.IO.ResourceFormats.XBin.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands
{
    public class LoadSds : ICommand
    {
        public uint Magic { get; } = 0x22663242;
        public int Size { get; } = 16;

        public ESlotType SlotType { get; init; }
        public string SdsName { get; init; } = null!;
        public string QuotaId { get; init; } = null!;
        public uint LoadFlags { get; init; }

        private LoadSds()
        {
        }

        public static ICommand ReadFromFile(BinaryReader reader)
        {
            var slotType = (ESlotType)reader.ReadUInt32();
            var sdsName = reader.ReadStringPointerWithOffset();
            var quotaId = reader.ReadStringPointerWithOffset();
            var loadFlags = reader.ReadUInt32();

            return new LoadSds()
            {
                SlotType = slotType,
                SdsName = sdsName,
                QuotaId = quotaId,
                LoadFlags = loadFlags
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            writer.Write((uint)SlotType);
            writer.PushStringPointer(SdsName);
            writer.PushStringPointer(QuotaId);
            writer.Write(LoadFlags);
        }
        
        public void FixUpStringPointer(XBinWriter writer)
        {
            writer.FixUpStringPointer(SdsName);
            writer.FixUpStringPointer(QuotaId);
        }
    }
}
