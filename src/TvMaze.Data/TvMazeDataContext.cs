using Microsoft.EntityFrameworkCore;
using TvMaze.Data.Model;

namespace TvMaze.Data;

public class TvMazeDataContext : DbContext
{
    public TvMazeDataContext(DbContextOptions<TvMazeDataContext> options)
        : base(options)
    {
    }
    
    public DbSet<Show> Shows { get; set; }
}