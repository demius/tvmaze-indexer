using TvMaze.Client;
using TvMaze.Indexer.Exceptions;
using TvMaze.Indexer.Processing.Commands;
using TvMaze.Scraper.Data;
using TvMaze.Scraper.Data.Model;

namespace TvMaze.Indexer.Processing.Workers;

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

                    var person = new Person
                    {
                        PersonId = member.Person.Id,
                        Name = member.Person.Name,
                        Birthday = member.Person.Birthday,
                        Url = member.Person.Url,
                        LastUpdated = member.Person.Updated,
                        TvShows = { show }
                    };

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
}