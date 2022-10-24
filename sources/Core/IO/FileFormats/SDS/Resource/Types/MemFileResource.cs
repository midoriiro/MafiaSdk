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

//THIS VERSION IS MODIFIED.
//SEE ORIGINAL CODE HERE::
//https://github.com/gibbed/Gibbed.Illusion

using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

public class MemFileResource : IResourceType<MemFileResource>
{
    public uint Unk0V4 { get; set; }
    public string Name { get; set; } = null!;
    public uint Unk1 { get; set; }
    public uint Unk2V4 { get; set; }
    public byte[] Data { get; set; } = null!;

    internal MemFileResource()
    {
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        if(version == 4)
        {
            stream.WriteValueU32(Unk0V4);
        }
        
        stream.WriteStringU32(Name, endian);
        stream.WriteValueU32(Unk1, endian);
        
        if (version == 4)
        {
            stream.WriteValueU32(Unk2V4);
        }
        
        stream.WriteValueS32(Data.Length, endian);
        stream.WriteBytes(Data);
    }

    public static MemFileResource Deserialize(ushort version, Stream stream, Endian endian)
    {
        uint unk0V4 = 0;
        
        if (version == 4)
        {
            unk0V4 = stream.ReadValueU32(endian);
        }
        
        string name = stream.ReadStringU32(endian);
        uint unk1 = stream.ReadValueU32(endian);
        
        if (unk1 != 1)
        {
            throw new InvalidOperationException();
        }

        uint unk2V4 = 0;
        
        if (version == 4)
        {
            unk2V4 = stream.ReadValueU32(endian);
        }
        
        uint size = stream.ReadValueU32(endian);
        byte[] data = stream.ReadBytes((int)size);

        return new MemFileResource()
        {
            Unk0V4 = unk0V4,
            Unk2V4 = unk2V4,
            Unk1 = unk1,
            Name = name,
            Data = data
        };
    }
}