using Microsoft.EntityFrameworkCore;

namespace TvMaze.Scraper.Data.Extensions;

public static class QueryableExtensions
{
    public class PagedQueryResult<T>
        where T : class
    {
        public PagedQueryResult(int page, int pageSize, int pageCount, IList<T> results)
        {
            CurrentPage = page;
            PageSize = pageSize;
            PageCount = pageCount;
            Results = results;
        }

        public int CurrentPage { get; }

        public int PageSize { get; }

        public int PageCount { get; }

        public IList<T> Results { get; }
    }

    public static async ValueTask<PagedQueryResult<T>> AsPagedResultAsync<T>(this IQueryable<T> query, int page, int pageSize)
        where T : class
    {
        if (page <= 0)
            throw new ArgumentOutOfRangeException();

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException();

        var count = query.Count();
        var pageCount = Math.Ceiling((double)count / pageSize);
        var results = await query
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedQueryResult<T>(
            page,
            pageSize,
            (int)pageCount,
            results
        );
    }
}