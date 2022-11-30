namespace TvMaze.Indexer.Exceptions;

/// <summary>
/// Raised when a particular TV show has not yet been indexed within our database
/// </summary>
public class TvShowNotIndexedException : Exception
{
    public TvShowNotIndexedException(int id)
        : base($"TV Show with ID {id} has not been indexed")
    {
    }
}