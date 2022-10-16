using System.IO.Compression;

namespace Core.IO.Compression.Zlib.Streams;

/// <summary>
/// hdr(?) + adler32 et end.
/// wraps a deflate stream
/// </summary>
public sealed class ZLibStream : DeflateStream
{
    public ZLibStream(Stream stream, CompressionMode mode)
        : base(stream, mode)
    {
    }

    public ZLibStream(Stream stream, CompressionMode mode, bool leaveOpen) :
        base(stream, mode, leaveOpen)
    {
    }

    public ZLibStream(Stream stream, CompressionMode mode, CompressionLevel level) :
        base(stream, mode, level)
    {
    }

    public ZLibStream(Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen) :
        base(stream, mode, level, leaveOpen)
    {
    }

    protected override ZLibOpenType OpenType => ZLibOpenType.ZLib;

    protected override ZLibWriteType WriteType => ZLibWriteType.ZLib;
}