using System.Text;
using Core.IO.ResourceFormats.XBin.Caches;
using Core.IO.ResourceFormats.XBin.Types;

namespace Core.IO.ResourceFormats.XBin
{
    public class XBin
    {
        private ulong Hash { get; init; }
        private int Version { get; init; }
        private int TableCount { get; init; }
        private int Offset { get; init; }
        public ITable Table { get; private init; } = null!;

        private XBin()
        {
        }
        
        // TODO replace BinaryReader by stream and pass endian through file (first value or manifest field value)
        // TODO refactor method name to Read/Write ?
        public static XBin ReadFromFile(BinaryReader reader)
        {
            XBinHashCache.Load();

            ulong hash = reader.ReadUInt64(); // I *think* this is to UInt32's molded together.
            int version = reader.ReadInt32();
            int tableCount = reader.ReadInt32();
            int offset = reader.ReadInt32();
            ITable table = XBinFactory.ReadXBin(reader, hash);
            
            return new XBin()
            {
                Hash = hash,
                Version = version,
                TableCount = tableCount,
                Offset = offset,
                Table = table
            };
        }

        public void WriteToFile(FileInfo fileInfo)
        {
            FileStream file = File.Open(fileInfo.FullName, FileMode.Create);
            WriteToStream(file);
        }

        public void WriteToStream(Stream stream, bool leaveOpen = false)
        {
            using var writer = new XBinWriter(stream, Encoding.UTF8, leaveOpen);
            writer.Write(Hash);
            writer.Write(Version);
            writer.Write(TableCount);
            writer.Write(Offset);
            Table.WriteToFile(writer);
        }
    }
}
