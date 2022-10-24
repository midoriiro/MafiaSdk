﻿/* Copyright (c) 2017 Rick (rick 'at' gibbed 'dot' us)
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

public class TableResource : IResourceType<TableResource>
{
    public List<TableData> Tables { get; set; } = null!;

    internal TableResource()
    {
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteValueU32((uint)Tables.Count);
        
        foreach (TableData table in Tables)
        {
            table.Serialize(version, stream, endian);
        }
    }

    public static TableResource Deserialize(ushort version, Stream stream, Endian endian)
    {
        uint count = stream.ReadValueU32(endian);

        var tables = new List<TableData>();
        
        for (uint i = 0; i < count; i++)
        {
            TableData table = TableData.Deserialize(version, stream, endian);
            tables.Add(table);
        }

        return new TableResource()
        {
            Tables = tables
        };
    }
}