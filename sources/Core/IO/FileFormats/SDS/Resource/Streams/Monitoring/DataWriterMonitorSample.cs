using Core.IO.FileFormats.SDS.Resource.Results;

namespace Core.IO.FileFormats.SDS.Resource.Streams.Monitoring;

public class DataWriterMonitorSample
{
    public long TimeStamp { get; private init; }
    public string Path { get; private init; } = null!;
    public long DataLength { get; private init; }

    private DataWriterMonitorSample()
    {
    }

    public static DataWriterMonitorSample FromDataDescriptor(DataDescriptor dataDescriptor, long timestamp)
    {
        return new DataWriterMonitorSample
        {
            TimeStamp = timestamp,
            Path = dataDescriptor.Path,
            DataLength = dataDescriptor.Data.LongLength
        };
    }
}