using System.Net;
using Flurl;
using Flurl.Http;
using Polly;
using TvMaze.Api.Types;

namespace TvMaze.Api;

/// <inheritdoc cref="ITvMazeClient"/>
public class TvMazeHttpClient : ITvMazeClient 
{
    private const string DEFAULT_BASE_URL = "https://api.tvmaze.com";

    /// <summary>
    /// Creates a new TV Maze API client instance
    /// </summary>
    /// <param name="apiBaseUrl">Optionally specifies the absolute TV Maze API base URL</param>
    /// <exception cref="ArgumentNullException">Raised if the provided base URL is null or empty</exception>
    /// <exception cref="UriFormatException">Raised when the base URL is a malformed or relative URI</exception>
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
    public async Task<PagedShowResponse> GetShows(int page = 0, CancellationToken cancellationToken = default)
    {
        var policy = BuildRetryPolicy();
        
        var response = await policy.ExecuteAsync(() => DEFAULT_BASE_URL
            .AppendPathSegment("shows")
            .SetQueryParam("page", page)
            .AllowHttpStatus("404")
            .GetAsync(cancellationToken));

        if(response.StatusCode == 404)
            return PagedShowResponse.Empty(page);

        var shows = await response.GetJsonAsync<Show[]>();
        return new PagedShowResponse(page, shows)
        {
            MoreAvailable = shows.Length > 0
        };
    }
    
    private static AsyncPolicy BuildRetryPolicy()
    {
        return Policy
            .Handle<FlurlHttpException>(IsTransientFault)
            .WaitAndRetryAsync(5, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

        bool IsTransientFault(FlurlHttpException ex)
        {
            int[] worthRetrying =
            {
                (int)HttpStatusCode.RequestTimeout,
                (int)HttpStatusCode.TooManyRequests,
                (int)HttpStatusCode.BadGateway,
                (int)HttpStatusCode.ServiceUnavailable,
                (int)HttpStatusCode.GatewayTimeout
            };
            
            return ex.StatusCode.HasValue && worthRetrying.Contains(ex.StatusCode.Value);
        }
    }
}