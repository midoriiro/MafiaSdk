namespace Core.IO.FileFormats.Streams;

internal abstract class Block
{
    public long Offset { get; }
    public uint Size { get; }

    protected Block(long offset, uint size)
    {
        Offset = offset;
        Size = size;
    }

    public bool IsValidOffset(long offset)
    {
        return offset >= Offset &&
               offset < Offset + Size;
    }

    public abstract bool Load(Stream input);
    public abstract void FreeLoadedData();
    public abstract int Read(Stream input, long baseOffset, byte[] buffer, int offset, int count);
}