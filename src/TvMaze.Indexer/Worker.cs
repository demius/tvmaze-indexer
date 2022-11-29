using Microsoft.EntityFrameworkCore;
using TvMaze.Api;
using TvMaze.Data;

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
        await using var dbContext = resolutionScope.ServiceProvider.GetService<TvMazeIndexContext>();
        await dbContext.Database.MigrateAsync(stoppingToken);

        // var offset = 0;
        
        // while (!stoppingToken.IsCancellationRequested && offset < 2400)
        // {
        //     var shows = await _apiClient.GetShows(offset, stoppingToken);
        //     
        //     offset = shows.Max(_ => _.Id);
        // }
    }
}