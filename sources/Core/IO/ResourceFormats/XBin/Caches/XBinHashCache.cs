using Core.IO.FileFormats.Hashing;

namespace Core.IO.ResourceFormats.XBin.Caches
{
    public static class XBinHashCache
    {
        private static bool _isLoaded;
        private static string[] _stringTable = null!;
        private static Dictionary<ulong, uint> _fnv64Storage = new ();
        private static Dictionary<uint, uint> _fnv32Storage = new ();

        public static void Load()
        {
            if (_isLoaded)
            {
                return;
            }
            
            // Load string table for XBins.
            string[] loadedLines = File.ReadAllLines("Resources/Caches/XBin_Hashes.txt");

            // Create all arrays
            _stringTable = new string[loadedLines.Length];

            // iterate through all lines and build our storage.
            for(uint index = 0; index < loadedLines.Length; index++)
            {
                string line = loadedLines[index];
                _stringTable[index] = line;

                uint hash32 = Fnv32.Hash(line);
                ulong hash64 = Fnv64.Hash(line);

                _fnv32Storage.TryAdd(hash32, index);
                _fnv64Storage.TryAdd(hash64, index);
            }

            _isLoaded = true;
        }

        public static string GetNameFromHash64(ulong hash, out bool isSuccessful)
        {
            isSuccessful = _fnv64Storage.TryGetValue(hash, out uint index);

            return GetFromTable(index);
        }

        public static string GetNameFromHash32(uint hash, out bool isSuccessful)
        {
            isSuccessful = _fnv32Storage.TryGetValue(hash, out uint index);

            return GetFromTable(index);
        }

        private static string GetFromTable(uint index)
        {
            return index == uint.MaxValue ? string.Empty : _stringTable[index];
        }
    }
}
