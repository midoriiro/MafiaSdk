/* Copyright (c) 2017 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System.Text;
using Core.IO.FileFormats.SDS.Archive;
using Core.IO.FileFormats.Streams;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS
{
    public class ArchiveFile
    {
        public const uint Signature = 0x53445300; // 'SDS\0'

        private Endian Endian { get; set; }
        private uint Version { get; set; }
        private Platform Platform { get; set; }
        private uint SlotRamRequired { get; set; }
        private uint SlotVramRequired { get; set; }
        private uint OtherRamRequired { get; set; }
        private uint OtherVramRequired { get; set; }
        private byte[]? Unknown20 { get; set; }
        private string? ResourceInfoXml { get; set; }
        private List<ResourceType> ResourceTypes { get; }
        private List<ResourceEntry> ResourceEntries { get; }
        private List<string> ResourceNames { get; }
        
        public ArchiveFile()
        {
            ResourceTypes = new List<ResourceType>();
            ResourceEntries = new List<ResourceEntry>();
            ResourceNames = new List<string>();
            Unknown20 = new byte[16];
        }

        public void Serialize(Stream output, ArchiveSerializeOptions options)
        {
            // If ratio isn't 0.0f, we'll try to compress.
            // If the user sets it to 0.0f, then we should skip any attempt to compress.
            bool compress = (options & ArchiveSerializeOptions.Compress) > 0;
            compress &= 0.9f != 0.0f; // TODO parametrize 0.9 compression ratio

            long basePosition = output.Position;
            Endian endian = Endian;

            using (var data = new MemoryStream(12))
            {
                data.WriteValueU32(Signature, Endian.Big);
                data.WriteValueU32(Version, endian);
                data.WriteValueU32((uint)Platform, Endian.Big);
                data.Flush();
                output.WriteFromMemoryStreamSafe(data, endian);
            }

            long headerPosition = output.Position;

            FileHeader fileHeader;
            output.Seek(56, SeekOrigin.Current);

            fileHeader.ResourceTypeTableOffset = (uint)(output.Position - basePosition);
            output.WriteValueS32(ResourceTypes.Count, endian);
            
            foreach (ResourceType resourceType in ResourceTypes)
            {
                resourceType.Write(output, endian);
            }
            
            var stride = (uint)(Version == 20 ? 38 : 30);
            var alignment = (uint)(Version == 20 ? 0x10000 : 0x4000);
            uint blockAlignment = (options & ArchiveSerializeOptions.OneBlock) != ArchiveSerializeOptions.None ? 
                (uint)ResourceEntries.Sum(entry => stride + (entry.Data?.Length ?? 0)) : 
                alignment;
            fileHeader.BlockTableOffset = (uint)(output.Position - basePosition);
            fileHeader.ResourceCount = 0;
            BlockWriterStream blockStream = BlockWriterStream.ToStream(
                output, 
                blockAlignment, 
                endian, 
                compress, 
                false
            );           
            
            foreach (ResourceEntry resourceEntry in ResourceEntries)
            {
                ResourceHeader resourceHeader;
                resourceHeader.TypeId = (uint)resourceEntry.TypeId;
                resourceHeader.Size = stride + (uint)(resourceEntry.Data?.Length ?? 0);
                resourceHeader.Version = resourceEntry.Version;
                resourceHeader.FileHash = resourceEntry.FileHash;
                resourceHeader.SlotRamRequired = resourceEntry.SlotRamRequired;
                resourceHeader.SlotVramRequired = resourceEntry.SlotVramRequired;
                resourceHeader.OtherRamRequired = resourceEntry.OtherRamRequired;
                resourceHeader.OtherVramRequired = resourceEntry.OtherVramRequired;

                using (var data = new MemoryStream())
                {
                    resourceHeader.Write(data, endian, Version);
                    data.Flush();
                    blockStream.WriteFromMemoryStreamSafe(data, endian);
                }

                blockStream.WriteBytes(resourceEntry.Data!);
                fileHeader.ResourceCount++;
            }

            blockStream.Flush();
            blockStream.Finish();

            fileHeader.XmlOffset = (uint)(output.Position - basePosition);
            
            if (string.IsNullOrEmpty(ResourceInfoXml) == false)
            {
                output.WriteString(ResourceInfoXml, Encoding.UTF8);
            }

            fileHeader.SlotRamRequired = SlotRamRequired;
            fileHeader.SlotVramRequired = SlotVramRequired;
            fileHeader.OtherRamRequired = OtherRamRequired;
            fileHeader.OtherVramRequired = OtherVramRequired;
            fileHeader.Flags = 1;
            fileHeader.Unknown20 = Unknown20 ?? new byte[16];

            output.Position = headerPosition;
            
            using (var data = new MemoryStream())
            {
                fileHeader.Write(data, endian);
                data.Flush();
                output.WriteFromMemoryStreamSafe(data, endian);
            }
        }

        public void Deserialize(Stream input)
        {
            long basePosition = input.Position;

            // Check Magic, should be SDS.
            uint magic = input.ReadValueU32(Endian.Big);
            
            if (magic != Signature)
            {
                throw new FormatException($"Unsupported Archive Signature: {magic}");
            }

            input.Seek(8, SeekOrigin.Begin);
            // Check Platform. There may be values for XboxOne and PS4, but that is unknown.
            var platform = (Platform)input.ReadValueU32(Endian.Big);
            if (platform != Platform.PC && platform != Platform.Xbox360 && platform != Platform.PS3)
            {
                throw new FormatException($"Unsupported Archive Platform: {platform}");
            }

            Endian endian = platform == Platform.PC ? Endian.Little : Endian.Big;

            input.Seek(4, SeekOrigin.Begin);
            // Check Version, should be 19 (Mafia: II) or 20 (Mafia III).
            var version = input.ReadValueU32(endian);
            
            if (version != 19 && version != 20)
            {
                throw new FormatException($"Unsupported Archive Version: {version}");
            }

            input.Seek(12, SeekOrigin.Begin);
            input.Position = basePosition;

            using (MemoryStream data = input.ReadToMemoryStreamSafe(12, endian))
            {
                data.Position += 4; // skip magic
                Version = data.ReadValueU32(endian);
                data.Position += 4; // skip platform
            }

            if (Version != 19 && Version != 20)
            {
                throw new FormatException($"Unsupported Archive Version: {version}");
            }

            FileHeader fileHeader;
            
            using (MemoryStream data = input.ReadToMemoryStreamSafe(52, endian))
            {
                fileHeader = FileHeader.Read(data, endian);
            }

            input.Position = basePosition + fileHeader.ResourceTypeTableOffset;
            uint resourceTypeCount = input.ReadValueU32(endian);
            var resourceTypes = new ResourceType[resourceTypeCount];
            
            for (uint i = 0; i < resourceTypeCount; i++)
            {
                resourceTypes[i] = ResourceType.Read(input, endian);
            }

            input.Position = basePosition + fileHeader.BlockTableOffset;
            BlockReaderStream blockStream = BlockReaderStream.FromStream(input, endian);
            var resources = new ResourceEntry[fileHeader.ResourceCount];

            for (uint i = 0; i < fileHeader.ResourceCount; i++)
            {
                ResourceHeader resourceHeader;
                int size = (Version == 20 ? 34 : 26);
                
                using (MemoryStream data = blockStream.ReadToMemoryStreamSafe(size, endian))
                {
                    resourceHeader = ResourceHeader.Read(data, endian, Version);

                }

                if (resourceHeader.Size < 30)
                {
                    throw new FormatException();
                }

                resources[i] = new ResourceEntry()
                {
                    TypeId = (int)resourceHeader.TypeId,
                    Version = resourceHeader.Version,
                    Data = blockStream.ReadBytes((int)resourceHeader.Size - (size + 4)),
                    FileHash = resourceHeader.FileHash,
                    SlotRamRequired = resourceHeader.SlotRamRequired,
                    SlotVramRequired = resourceHeader.SlotVramRequired,
                    OtherRamRequired = resourceHeader.OtherRamRequired,
                    OtherVramRequired = resourceHeader.OtherVramRequired,
                };
            }
            
            if (fileHeader.XmlOffset != 0)
            {
                input.Position = basePosition + fileHeader.XmlOffset;
                string xml = input.ReadString((int)(input.Length - input.Position), Encoding.UTF8);
                ResourceInfoXml = xml;
            }

            ResourceTypes.Clear();
            ResourceEntries.Clear();

            Endian = endian;
            Platform = platform;
            SlotRamRequired = fileHeader.SlotRamRequired;
            SlotVramRequired = fileHeader.SlotVramRequired;
            OtherRamRequired = fileHeader.OtherRamRequired;
            OtherVramRequired = fileHeader.OtherVramRequired;
            Unknown20 = (byte[])fileHeader.Unknown20.Clone();
            ResourceTypes.AddRange(resourceTypes);
            ResourceEntries.AddRange(resources);
        }
    }
}
