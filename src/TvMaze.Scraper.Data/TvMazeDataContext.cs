using Microsoft.EntityFrameworkCore;
using TvMaze.Scraper.Data.Model;

namespace TvMaze.Scraper.Data;

public class TvMazeDataContext : DbContext
{
    public TvMazeDataContext(DbContextOptions<TvMazeDataContext> options)
        : base(options)
    {
    }
    
    public DbSet<TvShow> TvShows { get; set; }
    
    public DbSet<Person> People { get; set; }
}