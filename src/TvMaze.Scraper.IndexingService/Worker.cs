using Microsoft.EntityFrameworkCore;
using TvMaze.Api;
using TvMaze.Api.Types;
using TvMaze.Scraper.Data;
using TvMaze.Scraper.Data.Model;

namespace TvMaze.Indexer;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ITvMazeClient _apiClient;
    private readonly ILogger<Worker> _logger;

    public Worker(IServiceScopeFactory serviceScopeFactory, ITvMazeClient apiClient, ILogger<Worker> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _apiClient = apiClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var resolutionScope = _serviceScopeFactory.CreateScope();
        await using var dbContext = resolutionScope.ServiceProvider.GetService<TvMazeDataContext>();
        
        await dbContext.Database.MigrateAsync(stoppingToken);

        var page = 0;
        PagedShowResponse nextResponse;
        
        do
        {
            nextResponse = await _apiClient.GetShows(page, stoppingToken);

            if (nextResponse.IsEmpty)
                break;
            
            _logger.LogInformation("{Count} show(s) retrieved with index range {Min}-{Max}",
                nextResponse.Count, nextResponse.Items.Min(_ => _.Id), nextResponse.Items.Max(_ => _.Id));

            var mapped = nextResponse.Items.Select(Transform);
            
            foreach (var show in mapped)
            {
                _logger.LogInformation("Adding/Updating show {Id}: {Name}", show.TvShowId, show.Name);

                await dbContext.TvShows.AddAsync(show, stoppingToken);
            }
            
            await dbContext.SaveChangesAsync(stoppingToken);
            
            page++;
        } while (!stoppingToken.IsCancellationRequested && nextResponse.MoreAvailable);
    }

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