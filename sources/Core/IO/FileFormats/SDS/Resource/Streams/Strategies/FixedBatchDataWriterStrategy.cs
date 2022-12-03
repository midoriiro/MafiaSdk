using Core.IO.FileFormats.SDS.Resource.Results;

namespace Core.IO.FileFormats.SDS.Resource.Streams.Strategies;

public class FixedBatchDataWriterStrategy : IDataWriterStrategy
{
    public uint Threshold { get; init; } // TODO check input in all strategies, use required keyword ?

    public bool IsWriteTriggered(Queue<DataDescriptor> queue)
    {
        return queue.Count >= Threshold;
    }
}