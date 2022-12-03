using System.Text;

namespace Core.IO.ResourceFormats.Extensions;

public static class BinaryReaderExtensions
{
    private static readonly StringBuilder StringBuilder = new ();
    
    public static string ReadString8(this BinaryReader reader)
    {
        byte size = reader.ReadByte();
        return new string(reader.ReadChars(size));
    }

    public static string ReadString16(this BinaryReader reader)
    {
        short size = reader.ReadInt16();
        return new string(reader.ReadChars(size));
    }

    public static string ReadString32(this BinaryReader reader)
    {
        int size = reader.ReadInt32();
        return new string(reader.ReadChars(size));
    }

    public static string ReadString64(this BinaryReader reader)
    {
        long size = reader.ReadInt64();
        return new string(reader.ReadChars((int)size));
    }

    public static string ReadString(this BinaryReader reader)
    {
        StringBuilder.Clear();

        while (reader.PeekChar() != '\0')
        {
            StringBuilder.Append(reader.ReadChar());
        }

        reader.ReadByte();
        
        return StringBuilder.ToString();
    }

    public static string ReadStringEncoded(this BinaryReader reader)
    {
        var stringBytes = new List<byte>();

        while (reader.PeekChar() != '\0')
        {
            stringBytes.Add(reader.ReadByte());
        }

        reader.ReadByte();
        
        return Encoding.UTF8.GetString(stringBytes.ToArray());
    }

    public static string ReadStringBuffer(this BinaryReader reader, int size)
    {
        var result = new string(reader.ReadChars(size));
        return result.Trim('\0');
    }
}