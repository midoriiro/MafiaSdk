using Core.IO.ResourceFormats.XBin.Caches;

namespace Core.IO.ResourceFormats.XBin.CoreTypes
{
    public class XBinHash32Name
    {
        public uint Hash { get; set; }
        public string Name { get; private set; } = null!;

        private XBinHash32Name()
        {
        }

        public static XBinHash32Name ReadFromFile(BinaryReader reader)
        {
            uint hash = reader.ReadUInt32();

            string name = XBinHashCache.GetNameFromHash32(hash, out bool _);

            return new XBinHash32Name()
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
