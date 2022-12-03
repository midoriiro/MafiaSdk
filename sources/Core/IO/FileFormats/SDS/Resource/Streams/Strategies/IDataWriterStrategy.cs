using Core.IO.FileFormats.SDS.Resource.Results;

namespace Core.IO.FileFormats.SDS.Resource.Streams.Strategies;

public interface IDataWriterStrategy
{
    public bool IsWriteTriggered(Queue<DataDescriptor> queue);
}