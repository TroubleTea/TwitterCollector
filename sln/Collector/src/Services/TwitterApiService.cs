using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using RestSharp;
using TwitterCollector.Model;
using TwitterCollector.Options;
using TwitterCollector.Interafaces;
using TwitterCollector.Exceptions;

namespace TwitterCollector.Services
{
  public class TwitterApiService : ITwitterApiService
  {
    private readonly TwitterApiOptions _twitterApiOptions;
    private readonly IRestClient _restClient;
    private readonly ILogger _logger;

    public TwitterApiService(IOptions<TwitterApiOptions> twitterApiOptions, IRestClient restClient, ILoggerFactory loggerFactory)
    {
      _twitterApiOptions = twitterApiOptions?.Value ?? throw new ArgumentNullException(nameof(twitterApiOptions));
      _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
      _logger = loggerFactory.CreateLogger<TwitterApiService>();

      if (string.IsNullOrEmpty(_twitterApiOptions.BearerToken))
      {
        throw new InvalidOperationException("Twitter API bearer token is not configured");
      }

      _restClient.AddDefaultHeader("Authorization", $"Bearer {_twitterApiOptions.BearerToken}");
      _restClient.BaseUrl = _twitterApiOptions.BaseUrl;
    }

    private IEnumerable<TwitterSearchResult> ExecuteSearch(IList<TwitterSearchQuery> searchQueries, DateTime startTime, long untilId)
    {
      foreach (var searchQuery in searchQueries)
      {
        var tweetFields = new List<string> { "author_id", "conversation_id", "created_at" };
        var builder = new TwitterSearchRequestBuilder(searchQuery)
        {
          MaxResults = 100,
          StartTime = startTime,
          UntilId = untilId,
          TweetFields = tweetFields
        };

        while (true)
        {
          var request = builder.Build();
          var uri = _restClient.BuildUri(request);

          _logger.LogInformation($"{request.Method} '{uri.AbsoluteUri}'");

          var response = _restClient.Get<TwitterSearchResult>(request);

          if (!response.IsSuccessful)
          {
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
              throw new TwitterApiLimitExceededException(response);
            }

            throw new SearchRequestFailedException(response);
          }

          var apiRateLimit = int.Parse(response.Headers.FirstOrDefault(h => h.Name == "x-rate-limit-limit")?.Value.ToString());
          var apiRateLimitRemaining = int.Parse(response.Headers.FirstOrDefault(h => h.Name == "x-rate-limit-remaining")?.Value.ToString());

          _logger.LogInformation($"API rate limit '{apiRateLimitRemaining}' remaining from '{apiRateLimit}'");

          if (apiRateLimitRemaining < 1)
          {
            throw new TwitterApiLimitExceededException(response);
          }

          var meta = response.Data?.Meta;
          var data = response.Data?.Data;

          builder.NextToken = meta.NextToken;

          _logger.LogInformation($"Search returned {data?.Count ?? 0} tweets. Next pagination token is {meta?.NextToken ?? "not set"}");

          yield return response.Data;

          if (string.IsNullOrEmpty(meta.NextToken))
          {
            break;
          }
        }
      }
    }

    public IEnumerable<TwitterSearchResult> SearchDirectTweetsFromUsers(IList<TwitterUser> twitterUsers, DateTime startTime, long untilId)
    {
      var searchQueries = TwitterApiSearchQueryBuilder.DirectTweetsFromUsers(twitterUsers);
      _logger.LogInformation($"Created {searchQueries.Count} search queries for {twitterUsers.Count} users");

      return ExecuteSearch(searchQueries, startTime, untilId);
    }

    public IEnumerable<TwitterSearchResult> SearchRepliesForDirectTweetsFromUsers(IList<TwitterUser> twitterUsers, DateTime startTime, long untilId)
    {
      var searchQueries = TwitterApiSearchQueryBuilder.DirectRepliesToTweetsFromUsers(twitterUsers);
      _logger.LogInformation($"Created {searchQueries.Count} search queries for {twitterUsers.Count} users");

      return ExecuteSearch(searchQueries, startTime, untilId);
    }
  }
}
