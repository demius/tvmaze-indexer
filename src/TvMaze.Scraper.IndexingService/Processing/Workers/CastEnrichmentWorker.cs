using TvMaze.Client;
using TvMaze.Indexer.Exceptions;
using TvMaze.Indexer.Processing.Tasks;
using TvMaze.Scraper.Data;
using TvMaze.Scraper.Data.Model;

namespace TvMaze.Indexer.Processing.Workers;

/// <summary>
/// A worker service responsible for processing cast enrichment tasks
/// </summary>
public class CastEnrichmentWorker
{
    private readonly IWorkQueue _workQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ITvMazeClient _apiClient;
    private readonly ILogger<CastEnrichmentWorker> _logger;
    private readonly CancellationToken _cancellationToken;

    public CastEnrichmentWorker(IWorkQueue workQueue, IHostApplicationLifetime applicationLifetime, 
        IServiceScopeFactory scopeFactory, ITvMazeClient apiClient, ILogger<CastEnrichmentWorker> logger)
    {
        _workQueue = workQueue;
        _scopeFactory = scopeFactory;
        _apiClient = apiClient;
        _logger = logger;
        _cancellationToken = applicationLifetime.ApplicationStopping;
    }

    /// <summary>
    /// Initializes the worker's task consumption loop
    /// </summary>
    public void Start()
    {
        Task.Run(async () => await MonitorAsync(), _cancellationToken);
    }

    private async ValueTask MonitorAsync()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            try
            {
                var task = await _workQueue.DequeueAsync(_cancellationToken);
                if(task is not FetchTvShowCastMembers)
                    continue;

                var cast = await _apiClient.GetCast(task.TvShowId, _cancellationToken);
                if (!cast.Any())
                {
                    _logger.LogInformation("No cast members are listed for TV Show {Id}", task.TvShowId);
                    continue;
                }
                
                using var resolutionScope = _scopeFactory.CreateScope();
                await using var dbContext = resolutionScope.ServiceProvider.GetService<TvMazeDataContext>();

                var show = await dbContext.TvShows.FindAsync(task.TvShowId, _cancellationToken);
                if (show == null)
                    throw new TvShowNotIndexedException(task.TvShowId);

                foreach (var member in cast)
                {
                    var existingPersonEntity = await dbContext.People.FindAsync(member.Person.Id);
                    if (existingPersonEntity is not null)
                    {
                        if (!existingPersonEntity.TvShows.Contains(show))
                            existingPersonEntity.TvShows.Add(show);
                        
                        continue;
                    }

                    var person = Map(member, show);

                    await dbContext.People.AddAsync(person, _cancellationToken);
                }
                
                await dbContext.SaveChangesAsync(_cancellationToken);
                
                _logger.LogInformation("Fetch casting for show {Id}", task.TvShowId);
            }
            catch (OperationCanceledException)
            {
                // noop (cancellation signaled)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch/persist cast members");
            }
        }
    }

    /// <summary>
    /// Maps a TV Maze cast member to a new database entity
    /// </summary>
    private static Person Map(TvMaze.Client.Types.CastMember castMember, TvShow show)
    {
        return new Person
        {
            PersonId = castMember.Person.Id,
            Name = castMember.Person.Name,
            Birthday = castMember.Person.Birthday,
            Url = castMember.Person.Url,
            LastUpdated = castMember.Person.Updated,
            TvShows = { show }
        };
    }
}