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

using Core.IO.FileFormats.SDS.Resource.Types.Table;
using Core.IO.FileFormats.SDS.Resource.Types.Table.Types;
using Core.IO.Streams;

namespace Core.IO.FileFormats.SDS.Resource.Types;

public class TableData : IResourceType<TableData>
{
    public ulong NameHash { get; set; }
    public string Name { get; set; } = null!;
    public uint Unk1 { get; set; }
    public uint Unk2 { get; set; }
    public byte[] Data { get; set; } = null!;

    public List<Row> Rows { get; set; } = null!;
    public List<Column> Columns { get; set; } = null!;

    internal TableData()
    {
    }

    public void Serialize(ushort version, Stream stream, Endian endian)
    {
        stream.WriteValueU64(NameHash, endian);
        stream.WriteStringU16(Name, endian);

        if(version >= 2)
        {
            stream.WriteBytes(new byte[10]);
            stream.WriteValueS32(-1);
            stream.WriteValueS32(0);
        }

        stream.WriteValueU16((ushort)Columns.Count, endian);
        stream.WriteValueU32(Unk1, endian);
        stream.WriteValueU32(Unk2, endian);
        stream.WriteValueU32((uint)(Data.Length / Rows.Count));
        stream.WriteValueU32((uint)Rows.Count);

        foreach (Row row in Rows)
        {
            for (var index = 0; index < Columns.Count; index++)
            {
                row.Values[index].Serialize(stream, endian);
            }
        }

        foreach (Column column in Columns)
        {
            column.Serialize(stream, endian);
        }
    }
    
    public static TableData Deserialize(ushort version, Stream input, Endian endian)
    {
        ulong nameHash = input.ReadValueU64(endian); 
        string name = input.ReadStringU16(endian);

        if(version >= 2)
        {
            input.ReadBytes(18);
        }

        ushort columnCount = input.ReadValueU16(endian);

        uint unk1 = input.ReadValueU32(endian);
        uint unk2 = input.ReadValueU32(endian);
        uint rowSize = input.ReadValueU32(endian);
        uint rowCount = input.ReadValueU32(endian);
        MemoryStream stream = input.ReadToMemoryStream((int)(rowSize * rowCount));
        byte[] data = stream.ReadBytes((int)stream.Length);

        var columns = new List<Column>();
        
        for (uint i = 0; i < columnCount; i++)
        {
            columns.Add(new Column()
            {
                NameHash = input.ReadValueU32(endian),
                Type = (ColumnType)input.ReadValueU8(),
                Unknown2 = input.ReadValueU8(),
                Unknown3 = input.ReadValueU16(endian),
            });
        }

        var rows = new List<Row>();
        
        for (uint i = 0; i < rowCount; i++)
        {
            var row = new Row();

            stream.Seek(i * rowSize, SeekOrigin.Begin);
            
            foreach (Column column in columns)
            {
                if ((byte)column.Type > 163)
                {
                    throw new FormatException();
                }

                object deserializedObject = column.DeserializeType(stream, endian);
                
                row.Values.Add((ISerializableTableData)deserializedObject);
            }

            rows.Add(row);
        }

        return new TableData()
        {
            Unk1 = unk1,
            Unk2 = unk2,
            Name = name,
            NameHash = nameHash,
            Data = data,
            Rows = rows,
            Columns = columns
        };
    }

    public override string ToString()
    {
        return Name;
    }
}