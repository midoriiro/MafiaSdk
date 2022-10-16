using System.Reflection;
using System.Runtime.InteropServices;

namespace Core.IO.Compression.Zlib;

internal class FixedArray : IDisposable
{
	private GCHandle _pHandle;
	private readonly Array _pArray;

	public FixedArray(Array array)
	{
		_pArray = array;
		_pHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
	}

	~FixedArray()
	{
		_pHandle.Free();
	}

	#region IDisposable Members

	public void Dispose()
	{
		_pHandle.Free();
		GC.SuppressFinalize(this);
	}

	public IntPtr this[int idx] => Marshal.UnsafeAddrOfPinnedArrayElement(_pArray, idx);

	public static implicit operator IntPtr(FixedArray fixedArray)
	{
		return fixedArray[0];
	}
	#endregion
}

public static class ListHelper
{
	public static void Add<T>(this List<T> list, params T[] items)
	{
		list.AddRange(items);
	}
	public static void AddRange<T>(this List<T> list, IEnumerable<T> items)
	{
		list.AddRange(items);
	}
}


internal static class BitFlag
{
	internal static bool IsSet(int bits, int flag)
	{
		return (bits & flag) == flag;
	}
	internal static bool IsSet(uint bits, uint flag)
	{
		return (bits & flag) == flag;
	}
	//internal static uint Set(uint bits, uint flag)
	//{
	//    return bits | flag;
	//}
	//internal static int Set(int bits, int flag)
	//{
	//    return bits | flag;
	//}
}
	
internal static class ZLibDll
{
	internal const string Name32 = "zlib32.dll";
	internal const string Name64 = "zlib64.dll";

	internal static readonly bool Is64 = IntPtr.Size == 8;

	internal const string ZLibDllFileVersion = "1.2.8.1";

	internal static string GetDllName()
	{
		return Is64 ? Name64 : Name32;
	}
	
}

public static class DllLoader
{
		
	[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
	private static extern IntPtr LoadLibrary(string lpFileName);

	// http://stackoverflow.com/questions/666799/embedding-unmanaged-dll-into-a-managed-c-sharp-dll
	public static void Load()
	{
		var executingAssembly = Assembly.GetExecutingAssembly();

		if (executingAssembly is null)
		{
			throw new Exception("Can't retrieve executing assembly");
		}

		// Get a temporary directory in which we can store the unmanaged DLL, with
		// this assembly's version number in the path in order to avoid version
		// conflicts in case two applications are running at once with different versions
		string directory = Path.Combine(Path.GetTempPath(), "zlibnet-zlib" + ZLibDll.ZLibDllFileVersion);

		try
		{
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
		}
		catch
		{
			// raced?
			if (!Directory.Exists(directory))
			{
				throw;
			}
		}

		string dllName = ZLibDll.GetDllName();
		string dllFullName = Path.Combine(directory, dllName);

		// Get the embedded resource stream that holds the Internal DLL in this assembly.
		// The name looks funny because it must be the default namespace of this project
		// (MyAssembly.) plus the name of the Properties subdirectory where the
		// embedded resource resides (Properties.) plus the name of the file.
		if (!File.Exists(dllFullName))
		{
			// Copy the assembly to the temporary file
			string temporaryFileName = Path.GetTempFileName();
			using (Stream? stream = executingAssembly.GetManifestResourceStream("ZLibNet." + dllName))
			{
				if (stream is null)
				{
					throw new Exception("Can't retrieve manifest resource from stream");
				}
					
				using (Stream outputFile = File.Create(temporaryFileName))
				{
					stream.CopyTo(outputFile);
				}
			}

			try
			{
				File.Move(temporaryFileName, dllFullName);
			}
			catch
			{
				// clean up temporary file
				try
				{
					File.Delete(temporaryFileName);
				}
				catch
				{
					// eat
				}

				// raced?
				if (!File.Exists(dllFullName))
				{
					throw;
				}
			}
		}

		// We must explicitly load the DLL here because the temporary directory is not in the PATH.
		// Once it is loaded, the DllImport directives that use the DLL will use the one that is already loaded into the process.
		IntPtr hFile = LoadLibrary(dllFullName);
			
		if (hFile == IntPtr.Zero)
		{
			throw new Exception("Can't load " + dllFullName);
		}
	}
}