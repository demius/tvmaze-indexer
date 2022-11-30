using Microsoft.AspNetCore.Mvc;
using TvMaze.Scraper.Api.ResponseTypes;
using TvMaze.Scraper.Data;

namespace TvMaze.Scraper.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TvShowsController : ControllerBase
{
    private readonly TvMazeDataContext _dbContext;
    private readonly ILogger<TvShowsController> _logger;

    public TvShowsController(TvMazeDataContext dbContext, ILogger<TvShowsController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet, Route("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        if (id <= 0) return BadRequest();
        
        var entity = await _dbContext.TvShows.FindAsync(id);
        if (entity == null) return NotFound();

        return Ok(new TvShowDto
        {
            Id = id,
            Name = entity.Name
        });
    }
}