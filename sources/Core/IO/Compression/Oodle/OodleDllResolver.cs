using System.Reflection;
using System.Runtime.InteropServices;

namespace Core.IO.Compression.Oodle;

public class OodleDllResolver
{
    private const string OodleDllName = "oo2core_8_win64.dll";

    private static string _libraryFullPath = null!;
    private static bool _isInitialized;

    public static bool TryResolveFrom(string libraryPath)
    {
        if (_isInitialized)
        {
            return true;
        }

        _libraryFullPath = Path.Join(libraryPath, OodleDllName);

        if (!File.Exists(_libraryFullPath))
        {
            return false;
        }

        NativeLibrary.SetDllImportResolver(typeof(Oodle).Assembly, OodleResolver);
        _isInitialized = true;

        return true;
    }

    private static IntPtr OodleResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        IntPtr libraryHandle = IntPtr.Zero;

        if (libraryName == OodleDllName)
        {
            NativeLibrary.TryLoad(_libraryFullPath, out libraryHandle);
        }
            
        return libraryHandle;
    }
}