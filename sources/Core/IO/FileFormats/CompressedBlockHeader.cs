using Core.IO.Streams;

namespace Core.IO.FileFormats;

public enum CompressedBlockHeaderPreset
{
    ZLib,
    Oodle
}

public readonly struct CompressedBlockHeader
{
    public uint UncompressedSize { get; init; }
    public uint HeaderSize { get; private init; }
    public ushort ChunkSize { get; init; }
    public ushort ChunkCount { get; private init; }
    public uint CompressedSize { get; init; }
    public uint Unknown0C { get; init; }
    public ushort[] Chunks { get; private init; } = null!;

    public CompressedBlockHeader(CompressedBlockHeaderPreset preset)
    {
        switch (preset)
        {
            case CompressedBlockHeaderPreset.ZLib:
                HeaderSize = 32;
                ChunkCount = 1;
                Chunks = new ushort[8];
                break;
            case CompressedBlockHeaderPreset.Oodle:
                HeaderSize = 128;
                ChunkCount = 1;
                Chunks = new ushort[8];
                break;
            default:
                // TODO refactor all switch to use this
                throw new ArgumentOutOfRangeException(nameof(preset), preset, null);
        }
    }

    public static CompressedBlockHeader Read(Stream stream, Endian endian)
    {
        uint uncompressedSize = stream.ReadValueU32(endian);
        uint headerSize = stream.ReadValueU32(endian);
        ushort chunkSize = stream.ReadValueU16(endian);
        ushort chunkCount = stream.ReadValueU16(endian);
        uint unknown0C = stream.ReadValueU32(endian);
        var chunks = new ushort[8];
        var compressedSize = 0;
            
        for (var index = 0; index < 8; ++index)
        {
            chunks[index] = stream.ReadValueU16(endian);
            compressedSize += chunks[index];
        }

        return new CompressedBlockHeader()
        {
           Chunks = chunks,
           ChunkCount = chunkCount,
           ChunkSize = chunkSize,
           CompressedSize = (uint)compressedSize,
           HeaderSize = headerSize,
           UncompressedSize = uncompressedSize,
           Unknown0C = unknown0C
        };
    }
    
    public void Write(Stream output, Endian endian) // TODO rename all paramater as 'stream'
    {
        output.WriteValueU32(UncompressedSize, endian);
        output.WriteValueU32(HeaderSize, endian);
        output.WriteValueU16(ChunkSize, endian);
        output.WriteValueU16(ChunkCount, endian);
        output.WriteValueU32(Unknown0C, endian);

        for (var i = 0; i < 8; i++)
        {
            output.WriteValueU16(Chunks[i], endian);
        }
    }

    public override string ToString()
    {
        return $"{UncompressedSize} {HeaderSize} {ChunkSize} {ChunkCount} {Unknown0C} {CompressedSize}";
    }
}