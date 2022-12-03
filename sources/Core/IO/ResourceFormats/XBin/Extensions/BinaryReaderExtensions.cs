using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin.Extensions;

public static class BinaryReaderExtensions
{
    public static string ReadStringPointerWithOffset(this BinaryReader reader)
    {
        uint offset = reader.ReadUInt32();
        return ReadStringPointer(reader, offset);
    }

    public static string ReadStringPointer(this BinaryReader reader, uint offset)
    {
        long currentPosition = reader.BaseStream.Position;
        reader.BaseStream.Seek((currentPosition - 4) + offset, SeekOrigin.Begin);
        string data = reader.ReadStringEncoded();
        reader.BaseStream.Seek(currentPosition, SeekOrigin.Begin);
        return data;
    }

    public static void GotoPointerWithOffset(this BinaryReader reader)
    {
        uint offset = reader.ReadUInt32();
        GotoPointer(reader, offset);
    }

    public static void GotoPointer(this BinaryReader reader, uint offset)
    {
        long currentPosition = reader.BaseStream.Position;
        reader.BaseStream.Seek((currentPosition - 4) + offset, SeekOrigin.Begin);
    }
}