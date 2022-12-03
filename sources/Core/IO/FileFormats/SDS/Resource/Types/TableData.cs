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

using Core.IO.FileFormats.SDS.Resource.Manifest.Attributes;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

public class TableData : IResourceType<TableData>
{
    public ulong NameHash { get; set; }
    public string Name { get; set; } = null!;
    public byte[]? UnkPatch0 { get; set; }
    public string? UnkPatch1 { get; set; }
    public byte[]? UnkPatch2 { get; set; }
    public byte[]? Unk0 { get; set; }
    public uint Unk1 { get; set; }
    public uint Unk2 { get; set; }
    [IgnoreFieldDescriptor]
    public byte[] Data { get; set; } = null!;

    private TableData()
    {
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteValueU64(NameHash, endian);
        stream.WriteStringU16(Name, endian);

        if(version >= 2)
        {
            // TODO is all patched tables need this ? Maybe this need refactor or a better implementation
            if (Name is "/tables/patch_ja_Pinups.tbl" or "/tables/patch_ja_Pinups_galleries.tbl" or "/tables/patch_ja_vehicles.tbl")
            {
                stream.WriteBytes(UnkPatch0!);
                stream.WriteStringU16(UnkPatch1!, endian);
                stream.WriteBytes(UnkPatch2!);
            }
            else
            {
                stream.WriteBytes(Unk0!);
            }
        }

        using var dataStream = new MemoryStream(Data);
        ushort columnsCount = dataStream.ReadValueU16(endian);
        uint rowsSize = dataStream.ReadValueU32(endian);
        uint rowsCount = dataStream.ReadValueU32(endian);
        byte[] data = dataStream.ReadBytes((int)(dataStream.Length - dataStream.Position));

        stream.WriteValueU16(columnsCount, endian);
        stream.WriteValueU32(Unk1, endian);
        stream.WriteValueU32(Unk2, endian);
        stream.WriteValueU32(rowsSize, endian);
        stream.WriteValueU32(rowsCount, endian);
        stream.WriteBytes(data);
    }
    
    public static TableData Deserialize(ushort version, Stream stream, Endian endian)
    {
        ulong nameHash = stream.ReadValueU64(endian); 
        string name = stream.ReadStringU16(endian);

        byte[]? unkPatch0 = null;
        string? unkPatch1 = null;
        byte[]? unkPatch2 = null;
        byte[]? unk0 = null;

        if(version >= 2)
        {
            // TODO is all patched tables need this ? Maybe this need refactor or a better implementation
            if (name is "/tables/patch_ja_Pinups.tbl" or "/tables/patch_ja_Pinups_galleries.tbl" or "/tables/patch_ja_vehicles.tbl")
            {
                unkPatch0 = stream.ReadBytes(8); // unknown
                unkPatch1 = stream.ReadStringU16(endian); // an other name
                unkPatch2 = stream.ReadBytes(8); // padding ?
            }
            else
            {
                unk0 = stream.ReadBytes(18);
            }
        }

        ushort columnsCount = stream.ReadValueU16(endian);

        uint unk1 = stream.ReadValueU32(endian);
        uint unk2 = stream.ReadValueU32(endian);
        uint rowsSize = stream.ReadValueU32(endian);
        uint rowsCount = stream.ReadValueU32(endian);

        var columnsOffset = (uint)(stream.Position + rowsSize * rowsCount);
        long dataEndOffset = columnsOffset + columnsCount * 8; // Column size is 8 bytes * columns count
        long dataBytesToRead = dataEndOffset - stream.Position;
        byte[] data = stream.ReadBytes((int)dataBytesToRead);

        using var dataStream = new MemoryStream();
        dataStream.WriteValueU16(columnsCount, endian);
        dataStream.WriteValueU32(rowsSize, endian); // TODO check all methods endian
        dataStream.WriteValueU32(rowsCount, endian);
        dataStream.WriteBytes(data);

        return new TableData
        {
            NameHash = nameHash,
            Name = name,
            UnkPatch0 = unkPatch0,
            UnkPatch1 = unkPatch1,
            UnkPatch2 = unkPatch2,
            Unk0 = unk0,
            Unk1 = unk1,
            Unk2 = unk2,
            Data = dataStream.ToArray()
        };
    }

    public override string ToString()
    {
        return Name;
    }
}