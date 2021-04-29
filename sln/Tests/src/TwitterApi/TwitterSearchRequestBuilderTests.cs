using System;
using TwitterCollector;
using Xunit;
using System.Collections.Generic;
using RestSharp;

namespace Tests.TwitterApi
{
  public class TwitterSearchRequestBuilderTests
  {
    [Fact]
    public void TwitterSearchRequestBuilder_ctor_should_throw_ArgumentNullException_if_searchQuery_parameter_is_null()
    {
      Assert.Throws<ArgumentNullException> (() => new TwitterSearchRequestBuilder(null));
    }

    [Fact]
    public void TwitterSearchRequestBuilder_SearchQuery_setter_should_throw_ArgumentNullException_if_value_is_null()
    {
      var searchQuery = new TwitterSearchQuery("mysearchquery");
      var builder = new TwitterSearchRequestBuilder(searchQuery);

      Assert.Throws<ArgumentNullException>(() => builder.SearchQuery = null);
    }

    [Fact]
    public void TwitterSearchRequestBuilder_MaxResults_setter_should_throw_ArgumentOutOfRangeException_if_value_is_greater_than_100()
    {
      var searchQuery = new TwitterSearchQuery("mysearchquery");
      var builder = new TwitterSearchRequestBuilder(searchQuery);

      Assert.Throws<ArgumentOutOfRangeException>(() => builder.MaxResults = 101);
    }

    [Fact]
    public void TwitterSearchRequestBuilder_MaxResults_setter_should_throw_ArgumentOutOfRangeException_if_value_is_less_than_1()
    {
      var searchQuery = new TwitterSearchQuery("mysearchquery");
      var builder = new TwitterSearchRequestBuilder(searchQuery);

      Assert.Throws<ArgumentOutOfRangeException>(() => builder.MaxResults = 0);
      Assert.Throws<ArgumentOutOfRangeException>(() => builder.MaxResults = -1);
    }

    [Fact]
    public void TwitterSearchRequestBuilder_Build_should_reflect_assigned_MaxResults()
    {
      var searchQuery = new TwitterSearchQuery("mysearchquery");
      var builder = new TwitterSearchRequestBuilder(searchQuery)
      {
        MaxResults = 50
      };

      Assert.Contains(builder.Build().Parameters, p =>
        p.Name == "max_results" && p.Value.Equals("50"));

      builder.MaxResults = 20;
      Assert.Equal(20, builder.MaxResults);
      Assert.Contains(builder.Build().Parameters, p =>
        p.Name == "max_results" && p.Value.Equals("20"));
    }

    [Fact]
    public void TwitterSearchRequestBuilder_Build_should_reflect_assigned_TweetFields()
    {
      var searchQuery = new TwitterSearchQuery("mysearchquery");
      var tweetFields = new List<string> {"A","B","C"};
      var builder = new TwitterSearchRequestBuilder(searchQuery);

      Assert.NotNull(builder.TweetFields);
      Assert.Empty(builder.TweetFields);

      Assert.Throws<ArgumentNullException>(() => builder.TweetFields = null);

      builder.TweetFields = tweetFields;
      Assert.Equal(tweetFields, builder.TweetFields);

      Assert.Contains(builder.Build().Parameters, p =>
        p.Name == "tweet.fields" && p.Value.Equals(string.Join(',', tweetFields)));
    }

    [Fact]
    public void TwitterSearchRequestBuilder_Build_should_reflect_assigned_SearchQuery()
    {
      var searchQuery = new TwitterSearchQuery("mysearchquery");
      var builder = new TwitterSearchRequestBuilder(searchQuery);

      Assert.NotNull(builder.SearchQuery);

      builder.SearchQuery = searchQuery;
      Assert.Equal(searchQuery, builder.SearchQuery);

      Assert.Contains(builder.Build().Parameters, p =>
        p.Name == "query" && p.Value.Equals(searchQuery.Query));
    }

    [Fact]
    public void TwitterSearchRequestBuilder_Build_should_reflect_assigned_SinceId()
    {
      var searchQuery = new TwitterSearchQuery("mysearchquery");
      var builder = new TwitterSearchRequestBuilder(searchQuery);

      Assert.Equal(default, builder.StartTime);

      var startTime = DateTime.UtcNow;

      builder.StartTime = startTime;
      Assert.Equal(startTime, builder.StartTime);

      var startTimeIso8601 = startTime.ToString("yyyy-MM-ddTHH:mm:ssZ");

      var client = new RestClient("https://twatter.com/");
      var uri = client.BuildUri(builder.Build()).ToString();

      Assert.Contains(builder.Build().Parameters, p =>
        p.Name == "start_time" && p.Value.Equals(startTimeIso8601));
    }

    [Fact]
    public void TwitterSearchRequestBuilder_Build_should_reflect_assigned_NextToken()
    {
      var searchQuery = new TwitterSearchQuery("mysearchquery");
      var builder = new TwitterSearchRequestBuilder(searchQuery);

      Assert.Null(builder.NextToken);

      builder.NextToken = "mypaginationtoken";
      Assert.Equal("mypaginationtoken", builder.NextToken);

      Assert.Contains(builder.Build().Parameters, p =>
        p.Name == "next_token" && p.Value.Equals("mypaginationtoken"));
    }
  }
}
