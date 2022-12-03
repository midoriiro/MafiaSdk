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

using Core.IO.Streams;

namespace Core.IO.FileFormats.Streams;

public class BlockReaderStream : Stream
{
    private const uint Signature = 0x6C7A4555; // 'zlEU'
        
    private readonly Stream _baseStream;
    private readonly List<Block> _blocks;
    private Block? _currentBlock;
    private long _currentPosition;

    private BlockReaderStream(Stream baseStream)
    {
        _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
        _blocks = new List<Block>();
        _currentBlock = null;
        _currentPosition = 0;
    }
        
    public static BlockReaderStream FromStream(Stream baseStream, Endian endian)
    {
        var instance = new BlockReaderStream(baseStream);

        uint magic = baseStream.ReadValueU32(endian);
        uint alignment = baseStream.ReadValueU32(endian);
        byte flags = baseStream.ReadValueU8();

        // We can check if this particular block uses Oodle compression.
        bool usesOodleCompression = (alignment & 0x1000000) != 0;

        if (magic != Signature || flags != 4)
        {
            throw new InvalidOperationException();
        }

        long virtualOffset = 0;

        while (true)
        {
            uint size = baseStream.ReadValueU32(endian);
            bool isCompressed = baseStream.ReadValueU8() != 0;

            if (size == 0)
            {
                break;
            }

            if (isCompressed)
            {
                // TODO: Consider if we can check if this is still a valid header.
                CompressedBlockHeader compressedBlockHeader = CompressedBlockHeader.Read(baseStream, endian);
                var headerSize = (int)compressedBlockHeader.HeaderSize;

                if (headerSize is not (32 or 128))
                {
                    throw new InvalidOperationException("The compressed header is not equal to 32 or 128.");
                }

                // Make sure the Size equals CompressedSize on the block header.
                if (size - headerSize != compressedBlockHeader.CompressedSize)
                {
                    throw new InvalidOperationException();
                }

                bool isUsingOodleCompression = headerSize == 128 && usesOodleCompression;
                long compressedPosition = baseStream.Position;
                uint remainingUncompressedSize = compressedBlockHeader.UncompressedSize;

                for (var i = 0; i < compressedBlockHeader.ChunkCount; ++i)
                {
                    uint uncompressedSize = Math.Min(alignment, remainingUncompressedSize);
                    instance.AddCompressedBlock(
                        virtualOffset,
                        uncompressedSize,
                        compressedPosition,
                        compressedBlockHeader.Chunks[i],
                        isUsingOodleCompression
                    );
                    compressedPosition += compressedBlockHeader.Chunks[i];
                    virtualOffset += uncompressedSize;
                    remainingUncompressedSize -= alignment;
                }

                // TODO: Consider if there is a better option for 
                int seekStride = compressedBlockHeader.HeaderSize == 128 ? 96 : 0;
                baseStream.Seek(compressedBlockHeader.CompressedSize + seekStride, SeekOrigin.Current);
            }
            else
            {
                instance.AddUncompressedBlock(virtualOffset, size, baseStream.Position);
                baseStream.Seek(size, SeekOrigin.Current);
                virtualOffset += size;
            }            
        }

        return instance;
    }

    private void AddUncompressedBlock(long virtualOffset, uint virtualSize, long dataOffset)
    {
        _blocks.Add(new UncompressedBlock(virtualOffset, virtualSize, dataOffset));
    }

    private void AddCompressedBlock(
        long virtualOffset, 
        uint virtualSize, 
        long dataOffset, 
        uint dataCompressedSize, 
        bool isOodle
    )
    {
        _blocks.Add(new CompressedBlock(virtualOffset, virtualSize, dataOffset, dataCompressedSize, isOodle));
    }

    private bool LoadBlock(long offset)
    {
        if (_currentBlock is not null && _currentBlock.IsValidOffset(offset))
        {
            return _currentBlock.Load(_baseStream);
        }

        Block? block = _blocks.SingleOrDefault(candidate => candidate.IsValidOffset(offset));
                
        if (block is null)
        {
            _currentBlock = null;
            return false;
        }
                
        _currentBlock = block;

        return _currentBlock.Load(_baseStream);
    }

    #region Stream
    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override void Flush()
    {
        _baseStream.Flush();
    }

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => _currentPosition;
        set => throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var totalRead = 0;

        while (totalRead < count)
        {
            if (LoadBlock(_currentPosition) == false)
            {
                throw new InvalidOperationException();
            }

            int read = _currentBlock!.Read(
                _baseStream,
                _currentPosition,
                buffer,
                offset + totalRead,
                count - totalRead
            );

            totalRead += read;
            _currentPosition += read;
        }

        return totalRead;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (origin == SeekOrigin.End)
        {
            throw new NotSupportedException();
        }

        if (origin == SeekOrigin.Current)
        {
            if (offset == 0)
            {
                return _currentPosition;
            }

            offset = _currentPosition + offset;
        }
        
        if (LoadBlock(offset) == false)
        {
            throw new InvalidOperationException();
        }

        _currentPosition = offset;
        return _currentPosition;
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }
    #endregion
}