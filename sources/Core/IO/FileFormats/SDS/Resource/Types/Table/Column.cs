using Core.IO.FileFormats.SDS.Resource.Types.Table.Types;
using Core.IO.Streams;
using Boolean = Core.IO.FileFormats.SDS.Resource.Types.Table.Types.Boolean;

namespace Core.IO.FileFormats.SDS.Resource.Types.Table;

public class Column
    {
        public uint NameHash;
        public ColumnType Type;
        public byte Unknown2;
        public ushort Unknown3;

        public void Serialize(Stream input, Endian endian)
        {
            input.WriteValueU32(NameHash);
            input.WriteValueU8((byte)Type);
            input.WriteValueU8(Unknown2);
            input.WriteValueU16(Unknown3);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(NameHash);
            writer.Write((byte)Type);
            writer.Write(Unknown2);
            writer.Write(Unknown3);
        }

        public object DeserializeType(MemoryStream stream, Endian endian)
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
                _ => throw new FormatException()
            };
        }

        public override string ToString()
        {
            return $"{NameHash:X8} : {Type} ({Unknown2}, {Unknown3})";
        }
    }