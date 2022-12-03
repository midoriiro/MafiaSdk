namespace Core.IO.FileFormats.SDS.Resource.Streams.Monitoring;

public class DataWriterMonitor
{
    private readonly Queue<DataWriterMonitorSample> _samples = new();

    public void Push(DataWriterMonitorSample sample)
    {
        _samples.Enqueue(sample);
    }

    public DataWriterMonitorSample[] Pull()
    {
        return Enumerable
            .Range(0, _samples.Count)
            .Select(_ => _samples.Dequeue())
            .ToArray();
    }
}