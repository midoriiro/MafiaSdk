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

using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

public class ScriptData : IResourceType<ScriptData>
{
    public ulong NameHash { get; set; }
    public ulong DataHash { get; set; }
    public string Name { get; set; } = null!;
    public byte[] Data { get; set; } = null!;

    internal ScriptData()
    {
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        if (version >= 2)
        {
            NameHash = FileFormats.Hashing.FNV64.Hash(Name);
            DataHash = FileFormats.Hashing.FNV64.Hash(Data, 0, Data.Length);
            stream.WriteValueU64(NameHash, endian);
            stream.WriteValueU64(DataHash, endian);
        }

        stream.WriteStringU16(Name, endian);
        stream.WriteValueS32(Data.Length, endian);
        stream.WriteBytes(Data);
    }

    public static ScriptData Deserialize(ushort version, Stream stream, Endian endian)
    {
        ulong nameHash = 0;
        ulong dataHash = 0;
        
        if (version >= 2)
        {
            nameHash = stream.ReadValueU64(endian);
            dataHash = stream.ReadValueU64(endian);
        }

        string name = stream.ReadStringU16(endian);
        uint size = stream.ReadValueU32(endian);
        byte[] data = stream.ReadBytes((int)size);

        return new ScriptData()
        {
            NameHash = nameHash,
            DataHash = dataHash,
            Name = name,
            Data = data
        };
    }

    public override string ToString()
    {
        return Name;
    }
}