using Core.IO.ResourceFormats.XBin.Enumerators;
using Core.IO.ResourceFormats.XBin.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands
{
    public class If : ICommand
    {
        public uint Magic { get; } = 0x1EFE290F;
        public int Size { get; } = 12;

        public string Variable { get; init; } = null!;
        public ECommandIfOperator Operator { get; init; }
        public string Value { get; init; } = null!;

        private If()
        {
        }

        public static ICommand ReadFromFile(BinaryReader reader)
        {
            string variable = reader.ReadStringPointerWithOffset();
            var @operator = (ECommandIfOperator)reader.ReadInt32();
            string value = reader.ReadStringPointerWithOffset();

            return new If()
            {
                Variable = variable,
                Operator = @operator,
                Value = value
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            writer.PushStringPointer(Variable);
            writer.Write((uint)Operator);
            writer.PushStringPointer(Value);
        }
        
        public void FixUpStringPointer(XBinWriter writer)
        {
            writer.FixUpStringPointer(Variable);
            writer.FixUpStringPointer(Value);
        }
    }
}
