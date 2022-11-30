using TvMaze.Indexer.Processing.Commands;

namespace TvMaze.Indexer.Processing;

public interface IWorkQueue
{
    ValueTask EnqueueAsync(IIndexingTask task);

    ValueTask<IIndexingTask> DequeueAsync(CancellationToken cancellationToken = default);
}