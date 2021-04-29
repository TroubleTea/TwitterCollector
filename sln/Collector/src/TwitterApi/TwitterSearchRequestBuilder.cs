using System;
using RestSharp;
using TwitterCollector.Interafaces;
using System.Collections.Generic;

namespace TwitterCollector
{
  public class TwitterSearchRequestBuilder : ITwitterRequestBuilder
  {
    public const int TwitterApiMaxResults = 100;
    public TwitterSearchQuery SearchQuery
    {
      get => _searchQuery;
      set => SetSearchQuery(value);
    }
    private TwitterSearchQuery _searchQuery;

    public int MaxResults
    {
      get => _maxResults;
      set => SetMaxResults(value);
    }
    private int _maxResults = TwitterApiMaxResults;

    public DateTime StartTime { get; set; }

    public List<string> TweetFields
    {
      get => _tweetFields;
      set => SetTweetFields(value);
    }
    private List<string> _tweetFields = new();

    public string NextToken { get; set; }
    public long UntilId
    {
      get => _untilId;
      set => SetUntilId(value);
    }

    private long _untilId;

    public TwitterSearchRequestBuilder(TwitterSearchQuery searchQuery)
    {
      SetSearchQuery(searchQuery);
    }

    private void SetSearchQuery(TwitterSearchQuery searchQuery)
    {
      _searchQuery = searchQuery ?? throw new ArgumentNullException(nameof(searchQuery));
    }

    private void SetTweetFields(List<string> tweetFields)
    {
      _tweetFields = tweetFields ?? throw new ArgumentNullException(nameof(tweetFields));
    }

    private void SetMaxResults(int maxResults)
    {
      if (maxResults > TwitterApiMaxResults)
      {
        throw new ArgumentOutOfRangeException(nameof(maxResults), "Twitter cannot return more than 100 results on one page.");
      }

      if (maxResults < 1)
      {
        throw new ArgumentOutOfRangeException(nameof(maxResults), "You need to get at least 1 result.");
      }

      _maxResults = maxResults;
    }

    private void SetUntilId(long untilId)
    {
      if (untilId < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(untilId));
      }

      _untilId = untilId;
    }

    public IRestRequest Build()
    {
      var request = new RestRequest("/2/tweets/search/recent");

      if (TweetFields.Count > 0)
      {
        request.AddQueryParameter("tweet.fields", string.Join(',', TweetFields));
      }

      request.AddQueryParameter("max_results", _maxResults.ToString());
      request.AddQueryParameter("query", _searchQuery.Query);

      if (StartTime != default && UntilId == 0)
      {
        request.AddQueryParameter("start_time", StartTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
      }

      if (UntilId > 0)
      {
        request.AddQueryParameter("until_id", UntilId.ToString());
      }

      if (!string.IsNullOrEmpty(NextToken))
      {
        request.AddQueryParameter("next_token", NextToken);
      }

      return request;
    }
  }
}
