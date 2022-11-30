using TvMaze.Indexer.Processing.Tasks;

namespace TvMaze.Indexer.Processing;

/// <summary>
/// Describes a simple queue for handling producer-consumer workloads required for TV show indexing
/// </summary>
public interface IWorkQueue
{
    /// <summary>
    /// Enqueues an indexing task
    /// </summary>
    ValueTask EnqueueAsync(IIndexingTask task);

    /// <summary>
    /// Dequeues the next available indexing task for processing
    /// </summary>
    ValueTask<IIndexingTask> DequeueAsync(CancellationToken cancellationToken = default);
}