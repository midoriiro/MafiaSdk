namespace Core.IO.FileFormats.Streams;

internal class UncompressedBlock : Block
{
    private readonly long _dataOffset;
    private bool _isLoaded;
    private byte[]? _data;

    public UncompressedBlock(long virtualOffset, uint virtualSize, long dataOffset)
        : base(virtualOffset, virtualSize)
    {
        _dataOffset = dataOffset;
        _isLoaded = false;
        _data = null;
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

        input.Seek(_dataOffset, SeekOrigin.Begin);
        _data = new byte[Size];
        
        if (input.Read(_data, 0,_data.Length) !=_data.Length)
        {
            throw new InvalidOperationException();
        }

        _isLoaded = true;
        return true;
    }

    public override int Read(Stream input, long baseOffset, byte[] buffer, int offset, int count)
    {
        if (baseOffset >=Offset +Size)
        {
            return 0;
        }

        Load(input);

        var relativeOffset = (int)(baseOffset -Offset);
        var read = (int)Math.Min(Size - relativeOffset, count);
        Array.ConstrainedCopy(_data!, relativeOffset, buffer, offset, read);
        return read;
    }
}