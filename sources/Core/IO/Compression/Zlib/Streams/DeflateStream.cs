using System.IO.Compression;

namespace Core.IO.Compression.Zlib.Streams;

/// <summary>Provides methods and properties used to compress and decompress streams.</summary>
public class DeflateStream : Stream
{
    private bool _isSuccess;
    
    private const int DataSize = 0x1000;
    private readonly byte[] _data = new byte[DataSize];
    private int _dataPosition;

    private Stream? _stream;
    private readonly CompressionMode _compressionMode;
    private ZStream _zStream;
    private readonly bool _leaveOpen;
    
    public long BytesWritten { get; private set; }
    public long BytesRead { get; private set; }

    protected virtual ZLibOpenType OpenType => ZLibOpenType.Deflate;
    protected virtual ZLibWriteType WriteType => ZLibWriteType.Deflate;
    
    // The compression ratio obtained (same for compression/decompression).
    public double CompressionRatio
    {
        get
        {
            if (_compressionMode == CompressionMode.Compress)
            {
                return BytesWritten == 0 ? 0.0 : 100.0 - BytesRead * 100.0 / BytesWritten;
            }
            
            return BytesRead == 0 ? 0.0 : 100.0 - BytesWritten * 100.0 / BytesRead;
        }
    }

    public DeflateStream(Stream stream, CompressionMode mode, bool leaveOpen) :
        this(stream, mode, CompressionLevel.Default, leaveOpen)
    {
    }

    public DeflateStream(Stream stream, CompressionMode mode, CompressionLevel level = CompressionLevel.Default) :
        this(stream, mode, level, false)
    {
    }

    public DeflateStream(Stream stream, CompressionMode compMode, CompressionLevel level, bool leaveOpen)
    {
        _leaveOpen = leaveOpen;
        _stream = stream;
        _compressionMode = compMode;

        int returnCode = _compressionMode == CompressionMode.Compress ? 
            ZLib.DeflateInit(ref _zStream, level, WriteType) : 
            ZLib.InflateInit(ref _zStream, OpenType);

        if (returnCode != ZLibReturnCode.Ok)
        {
            throw new ZLibException(returnCode, _zStream.LastErrorMessage);
        }

        _isSuccess = true;
    }

    /// <summary>GZipStream destructor. Cleans all allocated resources.</summary>
    ~DeflateStream()
    {
        Dispose(false);
    }

    /// <summary>
    /// Stream.Close() ->   Dispose(true); + GC.SuppressFinalize(this);
    /// Stream.Dispose() ->  Close();
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        try
        {
            try
            {
                if (!disposing || _stream is null)
                {
                    return;
                }
                
                //managed stuff
                if (_compressionMode == CompressionMode.Compress && _isSuccess)
                {
                    Flush();
                }

                if (!_leaveOpen)
                {
                    _stream.Close();
                }

                _stream = null;
            }
            finally
            {
                //unmanaged stuff
                FreeUnmanagedResources();
            }
        }
        finally
        {
            base.Dispose(disposing);
        }
    }

    // Finished, free the resources used.
    private void FreeUnmanagedResources()
    {
        if (_compressionMode == CompressionMode.Compress)
        {
            ZLib.DeflateEnd(ref _zStream);
        }
        else
        {
            ZLib.InflateEnd(ref _zStream);
        }
    }


    /// <summary>Reads a number of decompressed bytes into the specified byte array.</summary>
    /// <param name="buffer">The array used to store decompressed bytes.</param>
    /// <param name="offset">The location in the array to begin reading.</param>
    /// <param name="count">The number of bytes decompressed.</param>
    /// <returns>The number of bytes that were decompressed into the byte array. If the end of the stream has been reached, zero or the number of bytes read is returned.</returns>
    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_compressionMode == CompressionMode.Compress)
        {
            throw new NotSupportedException("Can't read on a compress stream!");
        }

        var readLength = 0;

        if (_dataPosition == -1)
        {
            return readLength;
        }

        using var workDataPtr = new FixedArray(_data);
        using var bufferPtr = new FixedArray(buffer);

        _zStream.next_in = workDataPtr[_dataPosition];
        _zStream.next_out = bufferPtr[offset];
        _zStream.avail_out = (uint)count;

        while (_zStream.avail_out != 0)
        {
            if (_zStream.avail_in == 0)
            {
                _dataPosition = 0;
                _zStream.next_in = workDataPtr;
                _zStream.avail_in = (uint)_stream!.Read(_data, 0, DataSize);
                BytesWritten += _zStream.avail_in;
            }

            uint inCount = _zStream.avail_in;
            uint outCount = _zStream.avail_out;

            int zlibError =
                ZLib.Inflate(ref _zStream, ZLibFlush.NoFlush); // flush method for inflate has no effect

            _dataPosition += (int)(inCount - _zStream.avail_in);
            readLength += (int)(outCount - _zStream.avail_out);

            if (zlibError == ZLibReturnCode.StreamEnd)
            {
                _dataPosition = -1; // magic for StreamEnd
                break;
            }

            if (zlibError == ZLibReturnCode.Ok)
            {
                continue;
            }

            _isSuccess = false;

            throw new ZLibException(zlibError, _zStream.LastErrorMessage);
        }

        BytesRead += readLength;

        return readLength;
    }


    /// <summary>This property is not supported and always throws a NotSupportedException.</summary>
    /// <param name="buffer">The array used to store compressed bytes.</param>
    /// <param name="offset">The location in the array to begin reading.</param>
    /// <param name="count">The number of bytes compressed.</param>
    public override void Write(byte[] buffer, int offset, int count)
    {
        if (_compressionMode == CompressionMode.Decompress)
        {
            throw new NotSupportedException("Can't write on a decompression stream!");
        }

        BytesWritten += count;

        using var writePtr = new FixedArray(_data);
        using var bufferPtr = new FixedArray(buffer);
        
        _zStream.next_in = bufferPtr[offset];
        _zStream.avail_in = (uint)count;
        _zStream.next_out = writePtr[_dataPosition];
        _zStream.avail_out = (uint)(DataSize - _dataPosition);

        while (_zStream.avail_in != 0)
        {
            if (_zStream.avail_out == 0)
            {
                //rar logikk, men det betyr vel bare at den kun skriver hvis buffer ble fyllt helt,
                //dvs halvfyllt buffer vil kun skrives ved flush
                _stream!.Write(_data, 0, DataSize);
                BytesRead += DataSize;
                _dataPosition = 0;
                _zStream.next_out = writePtr;
                _zStream.avail_out = DataSize;
            }

            uint outCount = _zStream.avail_out;

            int zlibError = ZLib.Deflate(ref _zStream, ZLibFlush.NoFlush);

            _dataPosition += (int)(outCount - _zStream.avail_out);

            if (zlibError != ZLibReturnCode.Ok)
            {
                _isSuccess = false;
                throw new ZLibException(zlibError, _zStream.LastErrorMessage);
            }
        }
    }

    /// <summary>Flushes the contents of the internal buffer of the current GZipStream object to the underlying stream.</summary>
    public override void Flush()
    {
        if (_compressionMode == CompressionMode.Decompress)
        {
            throw new NotSupportedException("Can't flush a decompression stream.");
        }

        using var workDataPtr = new FixedArray(_data);
        
        _zStream.next_in = IntPtr.Zero;
        _zStream.avail_in = 0;
        _zStream.next_out = workDataPtr[_dataPosition];
        _zStream.avail_out = (uint)(DataSize - _dataPosition);

        int zlibError = ZLibReturnCode.Ok;
        
        while (zlibError != ZLibReturnCode.StreamEnd)
        {
            if (_zStream.avail_out != 0)
            {
                uint outCount = _zStream.avail_out;
                zlibError = ZLib.Deflate(ref _zStream, ZLibFlush.Finish);

                _dataPosition += (int)(outCount - _zStream.avail_out);
                if (zlibError == ZLibReturnCode.StreamEnd)
                {
                    //ok. will break loop
                }
                else if (zlibError != ZLibReturnCode.Ok)
                {
                    _isSuccess = false;
                    throw new ZLibException(zlibError, _zStream.LastErrorMessage);
                }
            }

            _stream!.Write(_data, 0, _dataPosition);
            BytesRead += _dataPosition;
            _dataPosition = 0;
            _zStream.next_out = workDataPtr;
            _zStream.avail_out = DataSize;
        }

        _stream!.Flush();
    }

    /// <summary>Gets a value indicating whether the stream supports reading while decompressing a file.</summary>
    public override bool CanRead => _compressionMode == CompressionMode.Decompress && _stream!.CanRead;

    /// <summary>Gets a value indicating whether the stream supports writing.</summary>
    public override bool CanWrite => _compressionMode == CompressionMode.Compress && _stream!.CanWrite;

    /// <summary>Gets a value indicating whether the stream supports seeking.</summary>
    public override bool CanSeek => false;

    /// <summary>Gets a reference to the underlying stream.</summary>
    public Stream BaseStream => _stream!;

    /// <summary>This property is not supported and always throws a NotSupportedException.</summary>
    /// <param name="offset">The location in the stream.</param>
    /// <param name="origin">One of the SeekOrigin values.</param>
    /// <returns>A long value.</returns>
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException("Seek not supported");
    }

    /// <summary>This property is not supported and always throws a NotSupportedException.</summary>
    /// <param name="value">The length of the stream.</param>
    public override void SetLength(long value)
    {
        throw new NotSupportedException("SetLength not supported");
    }

    /// <summary>This property is not supported and always throws a NotSupportedException.</summary>
    public override long Length => throw new NotSupportedException("Length not supported.");

    /// <summary>This property is not supported and always throws a NotSupportedException.</summary>
    public override long Position
    {
        get => throw new NotSupportedException("Position not supported.");
        set => throw new NotSupportedException("Position not supported.");
    }
}