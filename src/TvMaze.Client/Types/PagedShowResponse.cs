using System.Collections;

namespace TvMaze.Api.Types;

public class PagedShowResponse
{
    public PagedShowResponse(int page, params Show[]? items)
    {
        PageNumber = page;
        Items = items ?? Array.Empty<Show>();
    }
    
    public IEnumerable<Show> Items { get; }
    
    public int PageNumber { get; }

    public int Count => Items.Count();

    public bool IsEmpty => !Items.Any();
    
    public bool MoreAvailable { get; set; }

    public static PagedShowResponse Empty(int page) => new PagedShowResponse(page);
}