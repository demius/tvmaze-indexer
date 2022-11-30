namespace TvMaze.Indexer.Processing.Tasks;

/// <summary>
/// Describes a specific indexing task that needs to be performed
/// </summary>
public interface IIndexingTask
{
    /// <summary>
    /// Gets or sets the unique TV show identifier
    /// </summary>
    int TvShowId { get; set; }
    
    /// <summary>
    /// Gets or sets the date/time at which the task was dispatched for processing
    /// </summary>
    DateTime DispatchTimestamp { get; set; }
}