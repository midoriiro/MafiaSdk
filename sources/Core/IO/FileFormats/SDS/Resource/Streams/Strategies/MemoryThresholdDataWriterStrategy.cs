using Core.IO.FileFormats.SDS.Resource.Results;

namespace Core.IO.FileFormats.SDS.Resource.Streams.Strategies;

public class MemoryThresholdDataWriterStrategy : IDataWriterStrategy
{
    public long TriggerAt { get; init; }

    public bool IsWriteTriggered(Queue<DataDescriptor> queue)
    {
        long dataInMemory = queue.Sum(dataDescriptor => (long)dataDescriptor.Data.Length);
        return dataInMemory >= TriggerAt;
    }
}