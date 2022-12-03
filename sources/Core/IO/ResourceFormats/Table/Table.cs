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

using Core.IO.ResourceFormats.Table.Types;
using Core.IO.Streams;

namespace Core.IO.ResourceFormats.Table;

public class Table
{
    public List<Row> Rows { get; init; } = null!;
    public List<Column> Columns { get; init; } = null!;

    private Table()
    {
    }
    
    private uint CalculateRowSize()
    {
        var rowSize = 0U;

        foreach (Column column in Columns)
        {
            switch (column.Type)
            {
                case ColumnType.Boolean:
                case ColumnType.Float32:
                case ColumnType.Signed32:
                case ColumnType.Unsigned32:
                case ColumnType.Flags32:
                    rowSize += 4;
                    break;
                case ColumnType.Hash64:
                    rowSize += 8;
                    break;
                case ColumnType.String8:
                    rowSize += 8;
                    break;
                case ColumnType.Color:
                    rowSize += 12;
                    break;
                case ColumnType.String16:
                    rowSize += 16;
                    break;
                case ColumnType.String32:
                    rowSize += 32;
                    break;
                case ColumnType.Hash64AndString32:
                    rowSize += 40;
                    break;
                case ColumnType.String64:
                    rowSize += 64;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(); // TODO add message
            }
        }
        
        return rowSize;
    }

    public static Table Read(Stream stream, Endian endian)
    {
        ushort columnsCount = stream.ReadValueU16(endian);
        uint rowsSize = stream.ReadValueU32(endian);
        uint rowsCount = stream.ReadValueU32(endian);

        var columnsOffset = (uint)(stream.Position + rowsSize * rowsCount);
        var rowsOffset = (uint)stream.Position;
        
        stream.Seek(columnsOffset, SeekOrigin.Begin);
        
        var columns = new List<Column>();
        
        for (uint index = 0; index < columnsCount; index++)
        {
            columns.Add(Column.Deserialize(stream, endian));
        }

        var rows = new List<Row>();
        
        for (uint index = 0; index < rowsCount; index++)
        {
            var row = new Row();

            stream.Seek(rowsOffset + index * rowsSize, SeekOrigin.Begin);
            
            foreach (Column column in columns)
            {
                if ((byte)column.Type > 163)
                {
                    throw new FormatException(); // TODO check if exceptions are message set and proper type 
                }

                object deserializedObject = column.DeserializeType(stream, endian);
                
                row.Values.Add((ISerializableTableData)deserializedObject);
            }

            rows.Add(row);
        }

        return new Table
        {
            Columns = columns,
            Rows = rows
        };
    }
    
    public void Write(Stream stream, Endian endian)
    {
        stream.WriteValueU16((ushort)Columns.Count, endian);
        stream.WriteValueU32(CalculateRowSize(), endian);
        stream.WriteValueU32((uint)Rows.Count, endian);

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
}