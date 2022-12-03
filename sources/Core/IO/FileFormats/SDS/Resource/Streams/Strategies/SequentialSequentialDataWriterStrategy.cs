using Core.IO.FileFormats.SDS.Resource.Results;

namespace Core.IO.FileFormats.SDS.Resource.Streams.Strategies;

public class SequentialDataWriterStrategy : IDataWriterStrategy
{
    public bool IsWriteTriggered(Queue<DataDescriptor> queue)
    {
        return true;
    }
}