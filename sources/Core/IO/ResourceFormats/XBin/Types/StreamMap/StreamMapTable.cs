using Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands;
using Core.IO.ResourceFormats.XBin.Types.StreamMap.Extensions;

namespace Core.IO.ResourceFormats.XBin.Types.StreamMap
{
    public class StreamMapTable : ITable
    {
        public uint Unk0 { get; init; }
        public List<StreamMapLine> Lines { get; init; } = null!;

        private StreamMapTable()
        {
        }

        public static ITable ReadFromFile(BinaryReader reader)
        {
            uint unk0 = reader.ReadUInt32();
            uint count0 = reader.ReadUInt32();
            uint count1 = reader.ReadUInt32();
            
            if (count0 != count1)
            {
                throw new InvalidDataException(
                    $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
                );
            }
            
            var lines = new StreamMapLine[count0];

            for (var i = 0; i < lines.Length; i++)
            {
                lines[i] = StreamMapLine.ReadFromFile(reader);
            }
            
            foreach (StreamMapLine line in lines)
            {
                line.ReadCommands(reader);
            }

            return new StreamMapTable()
            {
                Unk0 = unk0,
                Lines = lines.ToList()
            };
        }

        public void WriteToFile(XBinWriter writer)
        {
            int linesCount = Lines.Count;
            
            writer.Write(Unk0);
            writer.Write(linesCount);
            writer.Write(linesCount);

            for(var i = 0; i < linesCount; i++)
            {
                StreamMapLine line = Lines[i];
                writer.Write((uint)line.LineType);
                writer.PushStringPointer(line.GameId);
                writer.PushStringPointer(line.MissionId);
                writer.PushStringPointer(line.PartId);
                writer.PushObjectPointer($"TableOffset_{i}");
                writer.Write(line.TableCommands.Length);
                writer.Write(line.TableCommands.Length);
                writer.Write(line.IsAsync);
            }

            for (var i = 0; i < linesCount; i++)
            {
                StreamMapLine line = Lines[i];
                
                long tableOffset = writer.GetObjectPointerOffset($"TableOffset_{i}");
                var tableCommandsOffsetDebug = (int)(line.TableCommands.Length > 0 ? writer.BaseStream.Position : tableOffset);

                if (line.TableCommandsOffsetDebug != tableCommandsOffsetDebug)
                {
                    // TODO check if offset is the same when object has not been updated, if not throw error
                    // Should not throw error if object has been updated
                }

                if (line.TableCommands.Length > 0)
                {
                    writer.FixUpObjectPointer($"TableOffset_{i}");
                }
                else
                {
                    // Since table commands array is empty,
                    // we need to write value to zero at position when object pointer was pushed
                    writer.FixUpObjectPointerWithValue($"TableOffset_{i}", 0);
                }

                for (var x = 0; x < line.TableCommands.Length; x++)
                {
                    writer.PushObjectPointer($"TableCommandsOffset_{x}");
                    writer.Write(line.TableCommands[x].Magic);
                }

                for (var x = 0; x < line.TableCommands.Length; x++)
                {
                    ICommand command = line.TableCommands[x];
                    bool hasPreviousCommand = x > 0;

                    long tableCommandOffset = writer.GetObjectPointerOffset($"TableCommandsOffset_{x}");
                    long actualOffset = writer.BaseStream.Position - tableCommandOffset;

                    if (hasPreviousCommand)
                    {
                        actualOffset -= writer.FixUpOffset(command, line.TableCommands[x - 1]);
                    }

                    writer.FixUpObjectPointerWithValue($"TableCommandsOffset_{x}", actualOffset);
                    command.WriteToFile(writer);
                    
                    if (hasPreviousCommand)
                    {
                        writer.FixUpPosition(command, line.TableCommands[x - 1]);   
                    }
                }
            }
            
            for(var i = 0; i < linesCount; i++)
            {
                StreamMapLine line = Lines[i];
                writer.FixUpStringPointer(line.GameId);
                writer.FixUpStringPointer(line.MissionId);
                writer.FixUpStringPointer(line.PartId);

                foreach (ICommand command in line.TableCommands)
                {
                    command.FixUpStringPointer(writer);
                }
            }

            writer.FixUpStringPointers();
        }
    }
}
