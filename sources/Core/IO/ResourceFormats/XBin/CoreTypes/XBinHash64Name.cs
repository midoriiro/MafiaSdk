using Core.IO.ResourceFormats.XBin.Caches;

namespace Core.IO.ResourceFormats.XBin.CoreTypes
{
    public class XBinHash64Name
    {
        public ulong Hash { get; init; }

        public string Name { get; private set; } = null!;

        private XBinHash64Name()
        {
        }
        
        public static XBinHash64Name ReadFromFile(BinaryReader reader)
        {
            ulong hash = reader.ReadUInt64();

            string name = XBinHashCache.GetNameFromHash64(hash, out bool _);

            return new XBinHash64Name()
            {
                Hash = hash,
                Name = name
            };
        }

        public void WriteToFile(BinaryWriter writer)
        {
            writer.Write(Hash);
        }

        public void ForceToCheckStorage()
        {
            string returnedName = XBinHashCache.GetNameFromHash64(Hash, out bool isSuccessful);

            if(isSuccessful)
            {
                Name = returnedName;
            }
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(Name) ? $"{Name} [{Hash:X}]" : Hash.ToString();
        }
    }
}
