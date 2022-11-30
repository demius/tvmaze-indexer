using Microsoft.EntityFrameworkCore;
using Quartz;
using TvMaze.Client;
using TvMaze.Client.Types;
using TvMaze.Indexer.Processing;
using TvMaze.Indexer.Processing.Tasks;
using TvMaze.Scraper.Data;
using TvMaze.Scraper.Data.Model;

namespace TvMaze.Indexer.Jobs;

/// <summary>
/// Represents a Quartz job which is responsible for refreshing the TV Maze data set
/// </summary>
[DisallowConcurrentExecution]
public class RefreshMovieIndex : IJob
{
    private readonly TvMazeDataContext _dbContext;
    private readonly ITvMazeClient _apiClient;
    private readonly IWorkQueue _enrichmentQueue;
    private readonly ILogger<RefreshMovieIndex> _logger;

    public RefreshMovieIndex(TvMazeDataContext dbContext, ITvMazeClient apiClient, 
        IWorkQueue enrichmentQueue, ILogger<RefreshMovieIndex> logger)
    {
        _dbContext = dbContext;
        _apiClient = apiClient;
        _enrichmentQueue = enrichmentQueue;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("TV show index update started at {Time}", context.FireTimeUtc);
        _logger.LogInformation("Initializing the database and applying migrations");
        
        await _dbContext.Database.MigrateAsync(context.CancellationToken);

        // get the existing set of unique show identifiers to prevent unnecessary re-fetch and resume
        var existing = _dbContext.TvShows.Select(_ => _.TvShowId).ToHashSet();
        
        // TV Maze's documentation states that the index is cached in blocks of 250 and identifiers are sequential.
        // As per their documentation, we can resume the fetch operation by dividing the max identifier by 250.
        var page = existing.Any() ? (int)Math.Floor((double)existing.Max() / 250) : 0;

        PagedShowResponse nextResponse;
        do
        {
            nextResponse = await _apiClient.GetShows(page, context.CancellationToken);
            
            // An empty result set indicates we've reached the end of the list and can safely exit the loop
            if (nextResponse.IsEmpty)
                break;

            _logger.LogInformation("{Count} show(s) retrieved with index range {Min}-{Max}",
                nextResponse.Count, nextResponse.Items.Min(_ => _.Id), nextResponse.Items.Max(_ => _.Id));

            // filter our working set to only include shows we've not yet indexed
            var mapped = nextResponse.Items
                .Select(Transform)
                .Where(_ => !existing.Contains(_.TvShowId))
                .ToArray();
            
            await _dbContext.TvShows.AddRangeAsync(mapped, context.CancellationToken);
            await _dbContext.SaveChangesAsync(context.CancellationToken);

            foreach (var show in mapped)
            {
                // enqueue a command message (task) to enrich the show with cast information
                // this could be extended to include crew and all manner of other metadata (with a fair amount of work naturally)
                await _enrichmentQueue.EnqueueAsync(new FetchTvShowCastMembers { TvShowId = show.TvShowId });
            }
            page++;
        } while (!context.CancellationToken.IsCancellationRequested && nextResponse.MoreAvailable);

        _logger.LogInformation("TV show index update completed. Next scheduled occurrence will be at {Time}",
            context.NextFireTimeUtc);
    }

    /// <summary>
    /// Maps a show retrieved from TV Maze to a DB entity
    /// </summary>
    private static TvShow Transform(Show show)
    {
        return new TvShow
        {
            TvShowId = show.Id,
            Name = show.Name,
            Url = show.Url,
            LastUpdated = show.Updated
        };
    }
}