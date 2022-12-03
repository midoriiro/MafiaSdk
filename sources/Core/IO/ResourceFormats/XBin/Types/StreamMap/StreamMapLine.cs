using Core.IO.ResourceFormats.XBin.Enumerators;
using Core.IO.ResourceFormats.XBin.Extensions;
using Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands;

namespace Core.IO.ResourceFormats.XBin.Types.StreamMap;

public class StreamMapLine
{
    public EStreamMapLineType LineType { get; init; }
    public string GameId { get; init; } = null!;
    public string MissionId { get; init; } = null!;
    public string PartId { get; init; } = null!;
    internal int TableCommandsOffsetDebug { get; init; }
    public ICommand[] TableCommands { get; init; } = null!;
    public int IsAsync { get; init; }

    private StreamMapLine()
    {
    }

    public static StreamMapLine ReadFromFile(BinaryReader reader)
    {
        var lineType = (EStreamMapLineType)reader.ReadInt32();
        string gameId = reader.ReadStringPointerWithOffset();
        string missionId = reader.ReadStringPointerWithOffset();
        string partId = reader.ReadStringPointerWithOffset();

        int offset = reader.ReadInt32();
        var tableCommandsOffsetDebug = (int)(reader.BaseStream.Position + offset - 4);
        int count0 = reader.ReadInt32();
        int count1 = reader.ReadInt32();
        
        if (count0 != count1)
        {
            throw new InvalidDataException(
                $"Numbers should be identical: Count0 = {count0}, Count1 = {count1}"
            );
        }
        
        int isAsync = reader.ReadInt32();
        var tableCommands = new ICommand[count0];

        return new StreamMapLine()
        {
            LineType = lineType,
            GameId = gameId,
            MissionId = missionId,
            PartId = partId,
            IsAsync = isAsync,
            TableCommands = tableCommands,
            TableCommandsOffsetDebug = tableCommandsOffsetDebug
        };
    }

    public void ReadCommands(BinaryReader reader)
    {
        // Debug here. Make sure we are actually at the same offset as the Tables offset in the line. 
        // If not, we will have big problems and undoubtedly fail. 
        reader.BaseStream.Seek(TableCommandsOffsetDebug, SeekOrigin.Begin);

        // We have to read the declarations
        var tableCommandOffsets = new uint[TableCommands.Length];
        var tableCommandOffsets2 = new uint[TableCommands.Length];
        var tableCommandMagics = new uint[TableCommands.Length];

        // Iterate and read them
        for (var z = 0; z < TableCommands.Length; z++)
        {
            tableCommandOffsets[z] = reader.ReadUInt32();
            tableCommandOffsets2[z] = tableCommandOffsets[z];
            var actualOffset = (uint)(reader.BaseStream.Position + tableCommandOffsets[z] - 4);
            tableCommandOffsets[z] = actualOffset;

            tableCommandMagics[z] = reader.ReadUInt32();
        }

        // Construct the Command.
        for (var z = 0; z < TableCommands.Length; z++)
        {
            reader.BaseStream.Seek(tableCommandOffsets[z], SeekOrigin.Begin);
            TableCommands[z] = CommandFactory.ReadCommand(reader, tableCommandMagics[z]);
        }
    }

    public override string ToString()
    {
        return $"{GameId} {MissionId} {PartId}";
    }
}