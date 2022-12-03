using System.Text;

namespace Core.IO.ResourceFormats.Extensions;

public static class BinaryWriterExtensions
{
    public static void WriteString8(this BinaryWriter writer, string text)
    {
        writer.Write((byte)text.Length);
        writer.Write(text.ToCharArray());
    }
    
    public static void WriteString16(this BinaryWriter writer, string text)
    {
        writer.Write((ushort)text.Length);
        writer.Write(text.ToCharArray());
    }
    
    public static void WriteString32(this BinaryWriter writer, string text)
    {
        writer.Write(text.Length);
        writer.Write(text.ToCharArray());
    }
    
    public static void WriteString(this BinaryWriter writer, string text, bool trail = true)
    {
        writer.Write(text.ToCharArray());

        if (trail)
        {
            writer.Write('\0');
        }
    }
    
    public static void WriteStringBuffer(
        this BinaryWriter writer, 
        int size, 
        string text, 
        char trim = ' ', 
        Encoding? encoding = null
    )
    {
        bool addTrim = trim != ' ';
        int padding = size - text.Length;
        byte[] data = encoding is null ? Encoding.UTF8.GetBytes(text) : encoding.GetBytes(text);
        
        writer.Write(data);

        if (addTrim)
        {
            writer.Write('\0');
            padding -= 1;
        }

        writer.Write(new byte[padding]);
    }

    public static void WriteUnicodeNullTerminatedString(this BinaryWriter writer, string str)
    {
        writer.Write(Encoding.Unicode.GetBytes(str.ToCharArray()));
        writer.Write((short)0);
    }
}