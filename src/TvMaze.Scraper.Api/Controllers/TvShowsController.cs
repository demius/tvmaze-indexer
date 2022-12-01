using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TvMaze.Scraper.Api.ResponseTypes;
using TvMaze.Scraper.Data;
using TvMaze.Scraper.Data.Extensions;
using TvMaze.Scraper.Data.Model;

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

    [HttpGet]
    [EndpointDescription("Searches for TV Shows by their unique TV Maze identifier")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TvShowDto[]))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Search(int searchQuery, int pageSize = 20, int page = 1)
    {
        if (pageSize <= 0)
            pageSize = 20;

        page = Math.Max(page, 1);
        
        var resultSet = await _dbContext.TvShows
            .Include(_ => _.CastMembers)
            .Where(_ => _.TvShowId.ToString().Contains(searchQuery.ToString()))
            .OrderBy(_ => _.TvShowId)
            .AsPagedResultAsync(page, pageSize);
        
        HttpContext.Response.Headers.Add("x-tvm-page", page.ToString());
        HttpContext.Response.Headers.Add("x-tvm-page-size", pageSize.ToString());
        HttpContext.Response.Headers.Add("x-tvm-page-count", resultSet.PageCount.ToString());
        
        var mapped = resultSet.Results.Select(Map);

        return Ok(mapped);
    }

    private static TvShowDto Map(TvShow show)
    {
        var cast = show.CastMembers
            .OrderByDescending(e => e.Birthday)
            .Select(e => new CastMemberDto
            {
                Id = e.PersonId,
                Name = e.Name,
                Birthday = e.Birthday
            })
            .ToArray();

        return new TvShowDto
        {
            Id = show.TvShowId,
            Name = show.Name,
            Cast = cast
        };
    }
}