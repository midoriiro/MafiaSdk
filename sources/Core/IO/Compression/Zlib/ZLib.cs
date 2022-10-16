using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Core.IO.Compression.Zlib;

internal static class ZLib
{
	internal const string ZLibVersion = "1.2.8";
	internal const int MaxWbits = 15; /* 32K LZ77 window */
	internal const int DefMemLevel = 8;
	internal const int ZDefaultStrategy = 0;
	internal const uint VersionMadeBy = 0;

	private const int ZDeflated = 8; // The deflate compression method (the only one supported in this version)

	static ZLib()
	{
		DllLoader.Load();
	}


	[DllImport(ZLibDll.Name32, EntryPoint = "inflateInit2_", ExactSpelling = true, CharSet = CharSet.Ansi)]
	private static extern int inflateInit2_32(
		ref ZStream stream, 
		int windowBits, 
		string version, 
		int streamSize
	);
		
	[DllImport(ZLibDll.Name64, EntryPoint = "inflateInit2_", ExactSpelling = true, CharSet = CharSet.Ansi)]
	private static extern int inflateInit2_64(
		ref ZStream stream, 
		int windowBits, 
		string version, 
		int streamSize
	);

	internal static int InflateInit(ref ZStream stream, ZLibOpenType windowBits)
	{
		return ZLibDll.Is64 ? 
			inflateInit2_64(ref stream, (int)windowBits, ZLibVersion, Marshal.SizeOf(typeof(ZStream))) : 
			inflateInit2_32(ref stream, (int)windowBits, ZLibVersion, Marshal.SizeOf(typeof(ZStream)));
	}

	[DllImport(ZLibDll.Name32, EntryPoint = "deflateInit2_", ExactSpelling = true, CharSet = CharSet.Ansi)]
	private static extern int deflateInit2_32(
		ref ZStream stream, 
		int level, 
		int method, 
		int windowBits,
		int memLevel, 
		int strategy, 
		string version, 
		int streamSize
	);
		
	[DllImport(ZLibDll.Name64, EntryPoint = "deflateInit2_", ExactSpelling = true, CharSet = CharSet.Ansi)]
	private static extern int deflateInit2_64(
		ref ZStream stream, 
		int level, 
		int method, 
		int windowBits,
		int memLevel, 
		int strategy, 
		string version, 
		int streamSize
	);

	internal static int DeflateInit(ref ZStream stream, CompressionLevel level, ZLibWriteType windowBits)
	{
		if (ZLibDll.Is64)
		{
			return deflateInit2_64(
				ref stream, 
				(int)level, 
				ZDeflated, 
				(int)windowBits, 
				DefMemLevel,
				ZDefaultStrategy, 
				ZLibVersion, 
				Marshal.SizeOf(typeof(ZStream))
			);
		}
			
		return deflateInit2_32(
			ref stream, 
			(int)level, 
			ZDeflated, 
			(int)windowBits, 
			DefMemLevel,
			ZDefaultStrategy, 
			ZLibVersion, 
			Marshal.SizeOf(typeof(ZStream))
		);
	}

	[DllImport(ZLibDll.Name32, EntryPoint = "inflate", ExactSpelling = true)]
	private static extern int inflate_32(ref ZStream stream, ZLibFlush flush);
		
	[DllImport(ZLibDll.Name64, EntryPoint = "inflate", ExactSpelling = true)]
	private static extern int inflate_64(ref ZStream stream, ZLibFlush flush);

	internal static int Inflate(ref ZStream stream, ZLibFlush flush)
	{
		return ZLibDll.Is64 ? inflate_64(ref stream, flush) : inflate_32(ref stream, flush);
	}

	[DllImport(ZLibDll.Name32, EntryPoint = "deflate", ExactSpelling = true)]
	private static extern int deflate_32(ref ZStream stream, ZLibFlush flush);
		
	[DllImport(ZLibDll.Name64, EntryPoint = "deflate", ExactSpelling = true)]
	private static extern int deflate_64(ref ZStream stream, ZLibFlush flush);

	internal static int Deflate(ref ZStream stream, ZLibFlush flush)
	{
		return ZLibDll.Is64 ? deflate_64(ref stream, flush) : deflate_32(ref stream, flush);
	}

	[DllImport(ZLibDll.Name32, EntryPoint = "inflateEnd", ExactSpelling = true)]
	private static extern int inflateEnd_32(ref ZStream stream);
	[DllImport(ZLibDll.Name64, EntryPoint = "inflateEnd", ExactSpelling = true)]
	private static extern int inflateEnd_64(ref ZStream stream);

	internal static int InflateEnd(ref ZStream stream)
	{
		return ZLibDll.Is64 ? inflateEnd_64(ref stream) : inflateEnd_32(ref stream);
	}

	[DllImport(ZLibDll.Name32, EntryPoint = "deflateEnd", ExactSpelling = true)]
	private static extern int deflateEnd_32(ref ZStream stream);
	[DllImport(ZLibDll.Name64, EntryPoint = "deflateEnd", ExactSpelling = true)]
	private static extern int deflateEnd_64(ref ZStream stream);

	internal static int DeflateEnd(ref ZStream stream)
	{
		return ZLibDll.Is64 ? deflateEnd_64(ref stream) : deflateEnd_32(ref stream);
	}

	[DllImport(ZLibDll.Name32, EntryPoint = "crc32", ExactSpelling = true)]
	private static extern uint crc32_32(uint crc, IntPtr buffer, uint len);
	[DllImport(ZLibDll.Name64, EntryPoint = "crc32", ExactSpelling = true)]
	private static extern uint crc32_64(uint crc, IntPtr buffer, uint len);

	internal static uint Crc32(uint crc, IntPtr buffer, uint len)
	{
		return ZLibDll.Is64 ? crc32_64(crc, buffer, len) : crc32_32(crc, buffer, len);
	}
}

internal enum ZLibFlush
{
	NoFlush = 0, //Z_NO_FLUSH
	PartialFlush = 1,
	SyncFlush = 2,
	FullFlush = 3,
	Finish = 4 // Z_FINISH
}

internal enum ZLibCompressionStrategy
{
	Filtered = 1,
	HuffmanOnly = 2,
	DefaultStrategy = 0
}

//enum ZLibCompressionMethod
//{
//    Delated = 8
//}

internal enum ZLibDataType
{
	Binary = 0,
	Ascii = 1,
	Unknown = 2
}

public enum ZLibOpenType
{
	//If a compressed stream with a larger window
	//size is given as input, inflate() will return with the error code
	//Z_DATA_ERROR instead of trying to allocate a larger window.
	Deflate = -15, // -8..-15
	ZLib = 15, // 8..15, 0 = use the window size in the zlib header of the compressed stream.
	GZip = 15 + 16,
	BothZLibGZip = 15 + 32
}

public enum ZLibWriteType
{
	//If a compressed stream with a larger window
	//size is given as input, inflate() will return with the error code
	//Z_DATA_ERROR instead of trying to allocate a larger window.
	Deflate = -15, // -8..-15
	ZLib = 15, // 8..15, 0 = use the window size in the zlib header of the compressed stream.
	GZip = 15 + 16
	//		Both = 15 + 32,
}

public enum CompressionLevel
{
	NoCompression = 0,
	BestSpeed = 1,
	BestCompression = 9,
	// The "real" default is -1. Currently, zlib interpret -1 as 6, but they are free to change the interpretation.
	// The reason for overriding the default and using 5 is I want this library to match DynaZip's default
	// compression ratio and speed, and 5 was the best match (6 was somewhat slower than dynazip default).
	Default = 5,
	Level0 = 0,
	Level1 = 1,
	Level2 = 2,
	Level3 = 3,
	Level4 = 4,
	Level5 = 5,
	Level6 = 6,
	Level7 = 7,
	Level8 = 8,
	Level9 = 9
}

[StructLayout(LayoutKind.Sequential)]
internal struct ZStream
{
	public IntPtr next_in;  /* next input byte */
	public uint avail_in;  /* number of bytes available at next_in */
	public uint total_in;  /* total nb of input bytes read so far */

	public IntPtr next_out; /* next output byte should be put there */
	public uint avail_out; /* remaining free space at next_out */
	public uint total_out; /* total nb of bytes output so far */

	private IntPtr message;      /* last error message, NULL if no error */

	private IntPtr state; /* not visible by applications */

	private IntPtr zalloc;  /* used to allocate the internal state */
	private IntPtr zfree;   /* used to free the internal state */
	private IntPtr opaque;  /* private data object passed to zalloc and zfree */

	public ZLibDataType data_type;  /* best guess about the data type: ascii or binary */
	public uint adler;      /* adler32 value of the uncompressed data */
	private uint reserved;   /* reserved for future use */

	public string? LastErrorMessage => Marshal.PtrToStringAnsi(message);
}

internal static class ZLibReturnCode
{
	public const int Ok = 0;
	public const int StreamEnd = 1; //positive = no error
	public const int NeedDictionary = 2; //positive = no error?
	public const int Errno = -1;
	public const int StreamError = -2;
	public const int DataError = -3; //CRC
	public const int MemoryError = -4;
	public const int BufferError = -5;
	public const int VersionError = -6;

	public static string GetMessage(int returnCode)
	{
		return returnCode switch
		{
			Ok => "No error",
			StreamEnd => "End of stream raced",
			NeedDictionary => "A preset dictionary is needed",
			Errno => //consult error code
				"Unknown error " + Marshal.GetLastWin32Error(),
			StreamError => "Stream error",
			DataError => "Data was corrupted",
			MemoryError => "Out of memory",
			BufferError => "Not enough room in provided buffer",
			VersionError => "Incompatible zlib library version",
			_ => "Unknown error"
		};
	}
}


[Serializable]
public class ZLibException : ApplicationException
{
	public ZLibException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public ZLibException(int errorCode)
		: base(GetMsg(errorCode, null))
	{

	}

	public ZLibException(int errorCode, string? lastStreamError)
		: base(GetMsg(errorCode, lastStreamError))
	{
	}

	private static string GetMsg(int errorCode, string? lastStreamError)
	{
		string message = "ZLib error " + errorCode + ": " + ZLibReturnCode.GetMessage(errorCode);
			
		if (lastStreamError is not null && lastStreamError.Length > 0)
		{
			message += " (" + lastStreamError + ")";
		}

		return message;
	}
}