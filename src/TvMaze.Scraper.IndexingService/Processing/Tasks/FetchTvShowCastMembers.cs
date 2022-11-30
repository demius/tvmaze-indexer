namespace TvMaze.Indexer.Processing.Tasks;

/// <summary>
/// A lightweight task/command which indicates that a TV show's cast information must be fetched
/// </summary>
public class FetchTvShowCastMembers : IIndexingTask
{
    /// <inheritdoc cref="IIndexingTask"/>
    public int TvShowId { get; set; }

    /// <inheritdoc cref="IIndexingTask"/>
    public DateTime DispatchTimestamp { get; set; }
}