using Core.IO.FileFormats.SDS.Resource.Streams.Monitoring;

namespace Core.IO.FileFormats.SDS.Resource.Streams.Logging;

public interface IDataWriterLogger
{
    DataWriterMonitor Monitor { get; }

    void Log();
}