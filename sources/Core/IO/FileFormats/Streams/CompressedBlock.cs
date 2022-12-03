using System.IO.Compression;
using Core.Games;
using Core.IO.Compression.Oodle;
using Core.IO.Streams;
using ZLibStream = Core.IO.Compression.Zlib.Streams.ZLibStream;

namespace Core.IO.FileFormats.Streams;

internal class CompressedBlock : Block
{
    private readonly long _dataOffset;
    private readonly uint _dataCompressedSize;
    private bool _isLoaded;
    private readonly bool _isOodle;
    private byte[]? _data;

    public CompressedBlock(long virtualOffset, uint virtualSize, long dataOffset, uint dataCompressedSize, bool isOodle)
        : base(virtualOffset, virtualSize)
    {
        _dataOffset = dataOffset;
        _dataCompressedSize = dataCompressedSize;
        _isOodle = isOodle;
        _isLoaded = false;
        _data = null;

        if (!_isOodle)
        {
            return;
        }

        string libraryLocation = GameWorkSpace.Instance().SelectedGame.InstallationPath;
        bool isDllResolved = OodleDllResolver.TryResolveFrom(libraryLocation);

        if (!isDllResolved)
        {
            throw new InvalidOperationException($"Cannot resolve Oodle DLL from path '{libraryLocation}'");
        }
    }

    public override void FreeLoadedData()
    {
        _isLoaded = false;
        _data = null;
    }

    public override bool Load(Stream input)
    {
        if (_isLoaded)
        {
            return true;
        }
        
        if (_isOodle)
        {
            input.Seek(_dataOffset + 96, SeekOrigin.Begin);
            byte[] compressedData = input.ReadBytes((int)_dataCompressedSize);
            _data = Oodle.Decompress(compressedData, (int)_dataCompressedSize, (int)Size);
        }
        else
        {
            input.Seek(_dataOffset, SeekOrigin.Begin);
            _data = new byte[Size];
            using var stream = new ZLibStream(input, CompressionMode.Decompress, true);
            int length = stream.Read(_data, 0, _data.Length);
            
            if (length != _data.Length)
            {
                throw new InvalidOperationException();
            }
        }

        return _isLoaded = true;
    }

    public override int Read(Stream input, long baseOffset, byte[] buffer, int offset, int count)
    {
        if (baseOffset >= Offset + Size)
        {
            return 0;
        }

        Load(input);

        var relativeOffset = (int)(baseOffset - Offset);
        var read = (int)Math.Min(Size - relativeOffset, count);
        Array.ConstrainedCopy(
            _data!, 
            relativeOffset, 
            buffer, 
            offset, 
            read
        );
        return read;
    }
}