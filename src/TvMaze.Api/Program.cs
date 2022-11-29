using Microsoft.EntityFrameworkCore;
using TvMaze.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TvMazeDataContext>(
    options => options
        .UseSqlite("name=ConnectionStrings:TvMazeCache")
        .UseSnakeCaseNamingConvention());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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