using Microsoft.EntityFrameworkCore;
using TvMaze.Scraper.Data;

var connectionString = @$"Data Source={Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\tv_maze.db";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TvMazeDataContext>(
    options => options
        .UseSqlite(connectionString)
        .UseSnakeCaseNamingConvention());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();