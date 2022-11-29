using Microsoft.EntityFrameworkCore;
using TvMaze.Api;
using TvMaze.Data;
using TvMaze.Indexer;



IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<ITvMazeClient, TvMazeHttpClient>();
        services.AddHostedService<Worker>();
        services.AddDbContext<TvMazeDataContext>(
            options => options
                .UseSqlite("name=ConnectionStrings:TvMazeCache")
                .UseSnakeCaseNamingConvention());
    })
    .Build();

host.Run();