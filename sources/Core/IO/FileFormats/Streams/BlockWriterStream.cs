/* Copyright (c) 2017 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System.Diagnostics;
using System.IO.Compression;
using Core.IO.Compression.Oodle;
using Core.IO.Streams;
using CompressionLevel = Core.IO.Compression.Zlib.CompressionLevel;
using ZLibStream = Core.IO.Compression.Zlib.Streams.ZLibStream;

namespace Core.IO.FileFormats.Streams;

public class BlockWriterStream : Stream
{
    public const uint Signature = 0x6C7A4555; // 'zlEU'

    private readonly Endian _endian;
    private readonly uint _alignment;
    private readonly Stream _baseStream;
    private readonly byte[] _blockBytes;
    private int _blockOffset;
    private readonly bool _isCompressing;
    private readonly bool _useOodle;

    private BlockWriterStream(Stream baseStream, uint alignment, Endian endian, bool compress, bool useOodle)
    {
        _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
        _alignment = alignment;
        _endian = endian;
        _blockBytes = new byte[alignment];
        _blockOffset = 0;
        _isCompressing = compress;
        _alignment = alignment;
        if (useOodle)
        {
            _useOodle = true; // Parametrize this ToolkitSettings.bUseOodleCompression;
        }
    }
        
    public static BlockWriterStream ToStream(Stream baseStream, uint alignment, Endian endian, bool compress, bool bUseOodle)
    {
        var instance = new BlockWriterStream(baseStream, alignment, endian, compress, bUseOodle);
        baseStream.WriteValueU32(Signature, endian);          
        uint headerAlignment = instance._useOodle && compress ? alignment | 0x1000000 : alignment;
        baseStream.WriteValueU32(headerAlignment, endian);
        baseStream.WriteValueU8(4);
        return instance;
    }
        
    private static bool IsWithinCompressionRatio(int compressedSize, int blockLength)
    {
        float ratio = 0.9f * blockLength; // TODO: ToolkitSettings.CompressionRatio * blockLength;
        return !(compressedSize >= ratio);
    }

    #region Stream
    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override void Flush()
    {
        FlushBlock();
    }

    public override long Length => throw new NotSupportedException();

    public override long Position {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        long remaining = count;
            
        while (remaining > 0)
        {
            var write = (int)Math.Min(remaining,_alignment -_blockOffset);
            Array.Copy(
                buffer, 
                offset,
                _blockBytes,
                _blockOffset, 
                write
            );
            _blockOffset += write;
            remaining -= write;
            offset += write;

            if (_blockOffset >=_alignment)
            {
                FlushBlock();
            }
        }
    }
    #endregion

    private void FlushBlock()
    {
        if (_blockOffset == 0 || (_isCompressing && FlushCompressedBlock()))
        {
            return;
        }

        int blockLength = _blockOffset;
        _baseStream.WriteValueS32(blockLength,_endian);
        _baseStream.WriteValueU8(0);
        _baseStream.Write(_blockBytes, 0, blockLength);
        _blockOffset = 0;
    }

    private bool FlushCompressedBlock()
    {
        using var data = new MemoryStream();
        int blockLength =_blockOffset;

        return _useOodle ? 
            FlushOodleCompressedBlock(data, blockLength) : 
            FlushZlibCompressedBlock(data, blockLength);
    }

    private bool FlushZlibCompressedBlock(MemoryStream data, int blockLength)
    {
        var zlib = new ZLibStream(data, CompressionMode.Compress, CompressionLevel.Level9);
        zlib.Write(_blockBytes, 0, blockLength);
        zlib.Flush();

        // If it doesn't fit within the range of ratio, store as uncompressed.
        if (!IsWithinCompressionRatio((int)zlib.BytesWritten, blockLength))
        {
            return false;
        }

        var compressedLength = (int)data.Length;

        if (data.Length >= blockLength)
        {
            return false;
        }

        _baseStream.WriteValueS32(32 + compressedLength,_endian);
        _baseStream.WriteValueU8(1);
            
        var compressedBlockHeader = new CompressedBlockHeader
        {
            Chunks = new ushort[1]
        };
        compressedBlockHeader.SetZlibPreset();
        compressedBlockHeader.UncompressedSize = (uint)blockLength; //TODO: I think this should actually be alignment?
        compressedBlockHeader.CompressedSize = (uint)compressedLength;
        compressedBlockHeader.ChunkSize = (short)_alignment;
        compressedBlockHeader.Unknown0C = 135200769;
        compressedBlockHeader.Chunks[0] = (ushort)compressedBlockHeader.CompressedSize;
        compressedBlockHeader.Write(_baseStream,_endian);
            
        _baseStream.Write(data.GetBuffer(), 0, compressedLength);
        _blockOffset = 0;
            
        zlib.Close();
        zlib.Dispose();
            
        return true;
    }

    private bool FlushOodleCompressedBlock(MemoryStream data, int blockLength)
    {
        byte[] compressed = Oodle.Compress(
            _blockBytes, 
            blockLength, 
            OodleFormat.Kraken, 
            OodleCompressionLevel.Normal
        );
            
        Debug.Assert(compressed.Length != 0, "Compressed Block should not be empty");
            
        data.WriteBytes(compressed);

        // If it doesn't fit within the range of ratio, store as uncompressed.
        if (!IsWithinCompressionRatio(compressed.Length, blockLength))
        {
            return false;
        }

        var compressedLength = (int)data.Length;

        if (data.Length >= blockLength)
        {
            return false;
        }

        _baseStream.WriteValueS32(128 + compressedLength,_endian);
        _baseStream.WriteValueU8(1);
            
        var compressedBlockHeader = new CompressedBlockHeader
        {
            Chunks = new ushort[1]
        };
        compressedBlockHeader.SetOodlePreset();
        compressedBlockHeader.UncompressedSize = (uint)blockLength;
        compressedBlockHeader.CompressedSize = (uint)compressedLength;
        compressedBlockHeader.ChunkSize = 1;
        compressedBlockHeader.Unknown0C = (uint)blockLength;
        compressedBlockHeader.Chunks[0] = (ushort)compressedBlockHeader.CompressedSize;

        compressedBlockHeader.Write(_baseStream,_endian);
        _baseStream.Write(new byte[96], 0, 96); // Empty padding.
        _baseStream.Write(data.GetBuffer(), 0, compressedLength);
        _blockOffset = 0;
            
        return true;
    }

    public void Finish()
    {
        _baseStream.WriteValueS32(0,_endian);
        _baseStream.WriteValueU8(0);
    }
}