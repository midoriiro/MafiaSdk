using Core.IO.ResourceFormats.Table.Types;
using Core.IO.Streams;
using Boolean = Core.IO.ResourceFormats.Table.Types.Boolean;

namespace Core.IO.ResourceFormats.Table;

public class Column
    {
        public uint NameHash { get; set; }
        public ColumnType Type { get; set; }
        public byte Unk0 { get; set; }
        public ushort Unk1 { get; set; }

        public void Serialize(Stream stream, Endian endian)
        {
            stream.WriteValueU32(NameHash, endian);
            stream.WriteValueU8((byte)Type);
            stream.WriteValueU8(Unk0);
            stream.WriteValueU16(Unk1, endian);
        }

        public static Column Deserialize(Stream stream, Endian endian)
        {
            uint nameHash = stream.ReadValueU32(endian);
            var type = (ColumnType)stream.ReadValueU8();
            byte unk0 = stream.ReadValueU8();
            ushort unk1 = stream.ReadValueU16(endian);

            return new Column
            {
                NameHash = nameHash,
                Type = type,
                Unk0 = unk0,
                Unk1 = unk1
            };
        }

        public object DeserializeType(Stream stream, Endian endian)
        {
            return Type switch
            {
                ColumnType.Boolean => Boolean.Deserialize(stream, endian),
                ColumnType.Float32 => Float32.Deserialize(stream, endian),
                ColumnType.Signed32 => Signed32.Deserialize(stream, endian),
                ColumnType.Unsigned32 => Unsigned32.Deserialize(stream, endian),
                ColumnType.Flags32 => Flags32.Deserialize(stream, endian),
                ColumnType.Hash64 => Hash64.Deserialize(stream, endian),
                ColumnType.String8 => String8.Deserialize(stream, endian),
                ColumnType.String16 => String16.Deserialize(stream, endian),
                ColumnType.String32 => String32.Deserialize(stream, endian),
                ColumnType.String64 => String64.Deserialize(stream, endian),
                ColumnType.Color => Color.Deserialize(stream, endian),
                ColumnType.Hash64AndString32 => Hash64AndString32.Deserialize(stream, endian),
                _ => throw new ArgumentOutOfRangeException() // TODO add message
            };
        }

        public override string ToString()
        {
            return $"{NameHash:X8} : {Type} ({Unk0}, {Unk1})";
        }
    }