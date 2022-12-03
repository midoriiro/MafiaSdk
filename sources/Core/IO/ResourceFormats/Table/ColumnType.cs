namespace Core.IO.ResourceFormats.Table;

public enum ColumnType : byte
{
    Boolean = 1,
    Float32 = 2,
    Signed32 = 3,
    Unsigned32 = 4,
    Flags32 = 5,
    Hash64 = 6,
    String8 = 8,
    String16 = 16,
    String32 = 32,
    String64 = 64,
    Color = 66,
    Hash64AndString32 = 132
}