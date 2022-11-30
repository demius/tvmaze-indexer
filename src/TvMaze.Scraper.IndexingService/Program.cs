using Microsoft.EntityFrameworkCore;
using Quartz;
using TvMaze.Client;
using TvMaze.Scraper.Data;
using TvMaze.Indexer.Jobs;
using TvMaze.Indexer.Processing;
using TvMaze.Indexer.Processing.Workers;

var connectionString = @$"Data Source={Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\tv_maze.db";

var hostBuilder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<ITvMazeClient, TvMazeHttpClient>();
        services.AddSingleton<IWorkQueue, InMemoryWorkQueue>();
        services.AddSingleton<CastEnrichmentWorker>();
            
        services.AddDbContext<TvMazeDataContext>(
            options => options
                .UseSqlite(connectionString)
                .UseSnakeCaseNamingConvention());
        
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UseInMemoryStore();
        });
        
        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });
    })
    .Build();

// Use Quartz Scheduler to run the index refresh at the specified interval (currently 1 hourly)
var schedulerFactory = hostBuilder.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();

var indexRefreshJob = JobBuilder.Create<RefreshMovieIndex>().Build();
var indexRefreshTrigger = TriggerBuilder.Create()
    .StartNow()
    .WithSimpleSchedule(x => x
        .WithIntervalInHours(1)
        .RepeatForever())
    .Build();

await scheduler.ScheduleJob(indexRefreshJob, indexRefreshTrigger);

// Start the background worker for enrichment
hostBuilder.Services.GetRequiredService<CastEnrichmentWorker>().Start();

await hostBuilder.RunAsync();