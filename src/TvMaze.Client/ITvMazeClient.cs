namespace TvMaze.Api;

using System.Threading;
using TvMaze.Api.Types;

/// <summary>
/// Represents a TVMaze API client
/// </summary>
public interface ITvMazeClient
{
    /// <summary>
    /// Retrieves the cast members for a particular show
    /// </summary>
    /// <param name="showId">The unique TVMaze show identifier</param>
    /// <returns>A collection of cast members</returns>
    Task<IEnumerable<CastMember>> GetCast(int showId);

    /// <summary>
    /// Retrieves a paginated list of shows from TVMaze
    /// </summary>
    /// <param name="page">The zero-based page index to retrieve</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A collection of shows and their associated metadata</returns>
    /// <remarks>The TVMaze API currently limits the set to 250 results per page</remarks>
    Task<PagedShowResponse> GetShows(int page, CancellationToken cancellationToken = default);
}
