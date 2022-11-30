using System.Threading.Channels;
using TvMaze.Indexer.Processing.Commands;

namespace TvMaze.Indexer.Processing;

public class InMemoryWorkQueue : IWorkQueue
{
    private readonly Channel<IIndexingTask> _internalQueue;

    public InMemoryWorkQueue()
    {
        _internalQueue = Channel.CreateBounded<IIndexingTask>(new BoundedChannelOptions(20)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
    }
    
    public async ValueTask EnqueueAsync(IIndexingTask task)
        => await _internalQueue.Writer.WriteAsync(task);

    public async ValueTask<IIndexingTask> DequeueAsync(CancellationToken cancellationToken = default)
        => await _internalQueue.Reader.ReadAsync(cancellationToken);
}