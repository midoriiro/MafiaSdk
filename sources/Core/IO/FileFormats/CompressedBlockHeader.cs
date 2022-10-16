using Core.IO.Streams;

namespace Core.IO.FileFormats;

public struct CompressedBlockHeader
{
    public uint UncompressedSize;
    public uint HeaderSize;
    public short ChunkSize;
    public short ChunkCount;
    public uint CompressedSize;
    public uint Unknown0C;
    public ushort[] Chunks;

    public static CompressedBlockHeader Read(Stream input, Endian endian)
    {
        CompressedBlockHeader instance;
        instance.UncompressedSize = input.ReadValueU32(endian);
        instance.HeaderSize = input.ReadValueU32(endian);
        instance.ChunkSize = input.ReadValueS16(endian);
        instance.ChunkCount = input.ReadValueS16(endian);
        instance.Unknown0C = input.ReadValueU32(endian);
        instance.Chunks = new ushort[8];
        instance.CompressedSize = 0;
            
        for (var i = 0; i < 8; ++i)
        {
            instance.Chunks[i] = input.ReadValueU16(endian);
            instance.CompressedSize += instance.Chunks[i];
        }

        return instance;
    }

    public override string ToString()
    {
        return $"{UncompressedSize} {HeaderSize} {ChunkSize} {ChunkCount} {Unknown0C} {CompressedSize}";
    }

    public void SetZlibPreset()
    {
        HeaderSize = 32;
        ChunkCount = 1;
        ChunkSize = 0;
        Chunks = new ushort[8];
    }

    public void SetOodlePreset()
    {
        HeaderSize = 128;
        ChunkCount = 1;
        ChunkSize = 1;
        Chunks = new ushort[8];
    }

    public void Write(Stream output, Endian endian)
    {
        output.WriteValueU32(UncompressedSize, endian);
        output.WriteValueU32(HeaderSize, endian);
        output.WriteValueS16(ChunkSize, endian);
        output.WriteValueS16(ChunkCount, endian);
        output.WriteValueU32(Unknown0C, endian);

        for (var i = 0; i < 8; i++)
        {
            output.WriteValueU16(Chunks[i], endian);
        }
    }
}