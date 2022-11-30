namespace TvMaze.Indexer.Exceptions;

public class TvShowNotIndexedException : Exception
{
    public TvShowNotIndexedException(int id)
        : base($"TV Show with ID {id} has not been indexed")
    {
    }
}