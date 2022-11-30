using System.Threading.Channels;
using TvMaze.Indexer.Processing.Tasks;

namespace TvMaze.Indexer.Processing;

/// <summary>
/// A simple thread-safe in-memory queue abstraction
/// </summary>
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
    
    /// <inheritdoc cref="IWorkQueue"/>
    public async ValueTask EnqueueAsync(IIndexingTask task)
        => await _internalQueue.Writer.WriteAsync(task);

    /// <inheritdoc cref="IWorkQueue"/>
    public async ValueTask<IIndexingTask> DequeueAsync(CancellationToken cancellationToken = default)
        => await _internalQueue.Reader.ReadAsync(cancellationToken);
}