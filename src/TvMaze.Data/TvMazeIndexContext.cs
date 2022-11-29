using Microsoft.EntityFrameworkCore;
using TvMaze.Data.Model;

namespace TvMaze.Data;

public class TvMazeIndexContext : DbContext
{
    public TvMazeIndexContext(DbContextOptions<TvMazeIndexContext> options)
        : base(options)
    {
    }
    
    public DbSet<Show> Shows { get; set; }
}