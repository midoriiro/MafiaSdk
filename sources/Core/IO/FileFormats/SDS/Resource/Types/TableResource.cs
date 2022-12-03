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

public class TableResource : IResourceType<TableResource>
{
    [IgnoreFieldDescriptor]
    public int TablesCount => Tables.Length;

    public TableData[] Tables { get; set; } = null!;

    private TableResource()
    {
    }

    public uint GetDataLength()
    {
        return (uint)Tables.Sum(table => table.Data.Length);
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteValueU32((uint)Tables.Length);
        
        foreach (TableData table in Tables)
        {
            table.Serialize(version, stream, endian);
        }
    }

    public static TableResource Deserialize(ushort version, Stream stream, Endian endian)
    {
        uint count = stream.ReadValueU32(endian);

        var tables = new TableData[count];
        
        for (uint index = 0; index < count; index++)
        {
            tables[index] = TableData.Deserialize(version, stream, endian);
        }

        if (stream.Position != stream.Length)
        {
            throw new InvalidOperationException("There remain bytes to read"); // TODO find more accurate exception type and message
        }

        return new TableResource
        {
            Tables = tables
        };
    }
}