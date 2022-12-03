using Core.IO.FileFormats.SDS.Resource.Results;

namespace Core.IO.FileFormats.SDS.Resource.Streams.Writers;

public interface IDataWriter
{
    void Write(DataDescriptor dataDescriptor);
}