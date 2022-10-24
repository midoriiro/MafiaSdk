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

public class TextureResource : IResourceType<TextureResource>
{
    public ulong NameHash { get; set; }
    public byte Unknown8 { get; set; }
    public byte? HasMipMap { get; set; }
    public byte[] Data { get; set; } = null!;
    public bool IsDirectX10 { get; set; }

    internal TextureResource()
    {
    }

    internal TextureResource(ulong hash, byte? hasMipMap, byte[] data)
    {
        NameHash = hash;
        Unknown8 = 0;
        HasMipMap = hasMipMap;
        Data = data;
        IsDirectX10 = false;
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteValueU64(NameHash, endian);
        stream.WriteValueU8(Unknown8);

        if (version == 2)
        {
            stream.WriteValueU8(HasMipMap!.Value);
        }

        stream.WriteBytes(Data);

        IsDirectX10 = DetermineDirectX10(Data);
    }

    public void SerializeMipMap(ushort version, Stream stream, Endian endian)
    {
        stream.WriteValueU64(NameHash, endian);
        stream.WriteValueU8(Unknown8);
        stream.WriteBytes(Data);
    }

    public static TextureResource Deserialize(ushort version, Stream stream, Endian endian)
    {
        ulong nameHash = stream.ReadValueU64(endian);
        byte unknown8 = stream.ReadValueU8();

        byte? hasMipMap = null;

        if (version == 2)
        {
            hasMipMap = stream.ReadValueU8();
        }

        byte[] data = stream.ReadBytes((int)(stream.Length - stream.Position));

        return new TextureResource()
        {
            NameHash = nameHash,
            Unknown8 = unknown8,
            HasMipMap = hasMipMap,
            Data = data,
            IsDirectX10 = DetermineDirectX10(data)
        };
    }

    public static TextureResource DeserializeMipMap(ushort version, Stream stream, Endian endian)
    {
        ulong nameHash = stream.ReadValueU64(endian);
        byte unknown8 = stream.ReadValueU8();
        byte[] data = stream.ReadBytes((int)(stream.Length - stream.Position));

        return new TextureResource()
        {
            NameHash = nameHash,
            Unknown8 = unknown8,
            Data = data
        };
    }

    private static bool DetermineDirectX10(byte[] data)
    {
        var magic = BitConverter.ToUInt32(data, 0x54);
        return magic == 0x30315844;
    }

    public override string ToString()
    {
        return $"Hash: {NameHash}, Unk1: {Unknown8}, HasMIP: {HasMipMap}, Size: {Data.Length}";
    }
}