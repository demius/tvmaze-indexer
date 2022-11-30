namespace TvMaze.Scraper.Api.ResponseTypes;

public class TvShowDto
{
    public int Id { get; set; }
    
    public string? Name { get; set; }
    
    public IEnumerable<CastMemberDto> Cast { get; set; }
}