namespace TvMaze.Indexer.Processing.Commands;

public class FetchTvShowCastMembers : IIndexingTask
{
    public int TvShowId { get; set; }
}