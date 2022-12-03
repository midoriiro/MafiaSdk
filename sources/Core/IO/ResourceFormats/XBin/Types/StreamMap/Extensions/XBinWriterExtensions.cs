using System.Collections.Immutable;
using Core.IO.ResourceFormats.XBin.Types.StreamMap.Commands;

namespace Core.IO.ResourceFormats.XBin.Types.StreamMap.Extensions;

public static class XBinWriterExtensions
{
    private static readonly ImmutableDictionary<Type, Type[]> TypesToFix = new Dictionary<Type, Type[]>()
    {
        { 
            typeof(Suspend), 
            new []
            {
                typeof(Suspend),
                typeof(WaitForMovie),
                typeof(EndIf)
            }
        }
    }
    .ToImmutableDictionary();

    public static long FixUpOffset(this XBinWriter writer, ICommand current, ICommand previous)
    {
        bool isCommandNeedFixUp = TypesToFix.Any(pair => pair.Key == current.GetType() && pair.Value.Contains(previous.GetType()));
        bool isBytesToWriteIsDifferentCommandSize = previous.Size != previous.BytesToWrite;
        
        if (!isCommandNeedFixUp || !isBytesToWriteIsDifferentCommandSize)
        {
            return 0;
        }

        long fixedOffset = current.Size - previous.BytesToWrite;
        long positionToBackward = writer.BaseStream.Position - (current.Size - previous.BytesToWrite);
        writer.Seek((int)positionToBackward, SeekOrigin.Begin);
        return fixedOffset;
    }

    public static void FixUpPosition(this XBinWriter writer, ICommand current, ICommand previous)
    {
        bool isCommandNeedFixUp = TypesToFix.Any(pair => pair.Key == current.GetType() && pair.Value.Contains(previous.GetType()));
        bool isBytesToWriteIsDifferentCommandSize = previous.Size != previous.BytesToWrite;
        
        if (!isCommandNeedFixUp || !isBytesToWriteIsDifferentCommandSize)
        {
            return;
        }
        
        int offset = current.Size - (current.Size - previous.BytesToWrite);
        long positionToForward = writer.BaseStream.Position - offset;
        writer.Seek((int)positionToForward, SeekOrigin.Begin); 
    }
}