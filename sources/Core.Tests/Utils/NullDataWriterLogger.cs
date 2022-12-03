using Core.IO.FileFormats.SDS.Resource.Streams.Logging;
using Core.IO.FileFormats.SDS.Resource.Streams.Monitoring;

namespace Core.Tests.Utils;

public class NullDataWriterLogger : IDataWriterLogger
{
    private long _dataLength;
    public DataWriterMonitor Monitor { get; }
    
    public NullDataWriterLogger(DataWriterMonitor monitor)
    {
        Monitor = monitor;
    }

    public void Log()
    {
        var samples = Monitor.Pull();

        if (samples.Length == 0)
        {
            return;
        }

        var maximumTimestamp = samples
            .MaxBy(sample => sample.TimeStamp)!
            .TimeStamp;
        var minimumTimestamp = samples
            .MinBy(sample => sample.TimeStamp)!
            .TimeStamp;
        var duration = maximumTimestamp - minimumTimestamp;
        var dataLength = samples
            .Sum(sample => sample.DataLength);
        _dataLength += dataLength;
        long writeSpeed = 0;

        if (duration < 1000)
        {
            writeSpeed = (long)((duration / 1000f) * dataLength);
        }
        else
        {
            writeSpeed = dataLength / (duration / 1000);
        }
        
        // TODO create an auto convert bytes to proper unit
        Console.Out.WriteLine($"{_dataLength / 1024 / 1024} MB at {writeSpeed / 1024 / 1024} MB/s");
    }
}