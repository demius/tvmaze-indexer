using Flurl;
using Flurl.Http;
using TvMaze.Api.Types;

namespace TvMaze.Api;

/// <inheritdoc cref="ITvMazeClient"/>
public class TvMazeHttpClient : ITvMazeClient 
{
    private const string DEFAULT_BASE_URL = "https://api.tvmaze.com";

    public TvMazeHttpClient(string apiBaseUrl = DEFAULT_BASE_URL)
    {
        if (string.IsNullOrWhiteSpace(apiBaseUrl))
            throw new ArgumentNullException(nameof(apiBaseUrl));

        if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out Uri uri))
            throw new UriFormatException($"The provided API url ({apiBaseUrl}) must be absolute");

        ApiBaseUrl = apiBaseUrl;
    }

    /// <summary>
    /// Gets the TVMaze API base URL
    /// </summary>
    public string ApiBaseUrl { get; }
    
    /// <inheritdoc cref="ITvMazeClient"/>
    public Task<IEnumerable<CastMember>> GetCast(int showId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc cref="ITvMazeClient"/>
    public async Task<IEnumerable<Show>> GetShows(int offset = 0, CancellationToken cancellationToken = default)
    {
        var page = Math.Floor(offset + 1 / 240d);
        var response = await DEFAULT_BASE_URL
            .AppendPathSegment("shows")
            .SetQueryParam("page", page)
            .GetAsync(cancellationToken);

        return await response.GetJsonAsync<IList<Show>>();
    }
}