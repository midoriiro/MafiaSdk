using Core.IO.FileFormats.SDS.Resource.Results;
using Core.IO.FileFormats.SDS.Resource.Streams.Monitoring;
using Core.IO.FileFormats.SDS.Resource.Streams.Strategies;
using Core.IO.FileFormats.SDS.Resource.Streams.Writers;

namespace Core.IO.FileFormats.SDS.Resource.Streams;

// TODO check if classes need to be sealed
public sealed class DataWriterScheduler : IDisposable
{
    private Queue<DataDescriptor> Queue { get; } = new();
    private IDataWriterStrategy WriterStrategy { get; }
    private IDataWriter Writer { get; }
    private DataWriterMonitor Monitor { get; }

    public string BasePath { get; set; } = null!;

    public DataWriterScheduler(IDataWriterStrategy writerStrategy, IDataWriter writer, DataWriterMonitor monitor)
    {
        WriterStrategy = writerStrategy;
        Writer = writer;
        Monitor = monitor;
    }

    private void WriteAllData()
    {
        while (Queue.Count > 0)
        {
            DataDescriptor dataDescriptor = Queue.Dequeue();
            Writer.Write(dataDescriptor);
            Monitor.Push(DataWriterMonitorSample.FromDataDescriptor(
                dataDescriptor,
                DateTimeOffset.Now.ToUnixTimeMilliseconds()
            ));
        }
    }

    public void Push(List<DataDescriptor> dataDescriptors)
    {
        foreach (DataDescriptor dataDescriptor in dataDescriptors)
        {
            string pathToWrite = Path.Join(BasePath, dataDescriptor.Path);
            dataDescriptor.Path = pathToWrite;
            Queue.Enqueue(dataDescriptor);
        }
        
        bool isWriteTriggered = WriterStrategy.IsWriteTriggered(Queue);

        if (!isWriteTriggered)
        {
            return;
        }

        WriteAllData();
    }

    public void Flush()
    {
        WriteAllData();
    }

    public void Dispose()
    {
        Flush();
    }
}