using System;
using TwitterCollector.Interafaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using TwitterCollector.Model;
using TwitterCollector.Exceptions;
using System.Linq;

namespace TwitterCollector.Services
{
  public class CollectorService : ICollectorService
  {
    private readonly IJobService _jobService;
    private readonly ITwitterUserService _twitterUserService;
    private readonly ITweetService _tweetService;
    private readonly ITwitterApiService _twitterApiService;
    private readonly ILogger _logger;

    public CollectorService(
      IJobService jobService,
      ITwitterUserService twitterUserService,
      ITweetService tweetService,
      ITwitterApiService twitterApiService,
      ILoggerFactory loggerFactory)
    {
      _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
      _twitterUserService = twitterUserService ?? throw new ArgumentNullException(nameof(twitterUserService));
      _tweetService = tweetService ?? throw new ArgumentNullException(nameof(tweetService));
      _twitterApiService = twitterApiService ?? throw new ArgumentNullException(nameof(twitterApiService));

      _logger = loggerFactory.CreateLogger<CollectorService>();
    }

    private void LogFailedSearchRequest(SearchRequestFailedException e)
    {
      var exceptionType = e.GetType();
      if (exceptionType == typeof(TwitterApiLimitExceededException))
      {
        _logger.LogWarning("API limit reached.");
      }
      else
      {
        var message = $@"
          Message: {e.Message}
          ExceptionType: {exceptionType}
          StatusCode: {e.Response.StatusCode}
          StackTrace: {e.StackTrace}
          ";

        _logger.LogError(message);
      }
    }

    private int ExecuteJob(string jobName, Func<IList<TwitterUser>, DateTime, long, IEnumerable<TwitterSearchResult>> searchFunc)
    {
      var jobId = _jobService.CreateJob(jobName);

      _logger.LogInformation($"Job '{jobName}' created with ID '{jobId}'");

      var lastJob = _jobService.GetLastFinishedJob(jobName);

      DateTime startTime = default;
      long untilId = 0;
      if (lastJob != null)
      {
        if (lastJob.RateLimitReached)
        {
          untilId = _tweetService.GetLowestTweetIdImportedByJob(lastJob.Id);
        }

        startTime = lastJob.FinishedAt;
        _logger.LogInformation($"Last job ID was '{lastJob.Id}'. Collecting tweets since '{startTime}'");
      }

      var collectedTweetsCount = 0;
      var twitterUsers = _twitterUserService.GetUsers();

      if (twitterUsers.Count == 0)
      {
        throw new InvalidOperationException("Are you sure you populated the users table?");
      }

      var rateLimitReached = false;
      try
      {
        var searchResults = searchFunc(twitterUsers, startTime, untilId);

        // HACK: If the API limit was reached, we need to fetch tweets with IDs lower than the lowest fetched by
        //       The last job. This will not include new tweets that were posted since the last job ran. We need
        //       To fetch them seperately.
        if (untilId > 0)
        {
          _logger.LogInformation("Fetching new tweets since the last job usnig start_time.");

          searchResults = searchResults.Concat(searchFunc(twitterUsers, startTime, 0));
        }

        foreach (var searchResult in searchResults)
        {
          var tweet = searchResult.Data;

          // Our query did not return any tweets
          if (searchResult.Data == null)
          {
            continue;
          }

          var insertedTweets = _tweetService.InsertMany(tweet, jobId);
          collectedTweetsCount += insertedTweets;
        }
      }
      catch (SearchRequestFailedException e)
      {
        if (e.GetType() == typeof(TwitterApiLimitExceededException))
        {
          rateLimitReached = true;
        }

        LogFailedSearchRequest(e);
      }
      finally
      {
        var jobFinisedAt = DateTime.UtcNow;
        _jobService.FinishJob(jobId, jobFinisedAt, collectedTweetsCount, rateLimitReached);

        _logger.LogInformation($"Job '{jobName}' with ID '{jobId}' finished at '{jobFinisedAt}'");
      }

      return collectedTweetsCount;
    }

    public int CollectTweets()
    {
      return ExecuteJob("CollectTweets", (IList<TwitterUser> twitterUsers, DateTime startTime, long untilId) =>
        _twitterApiService.SearchDirectTweetsFromUsers(twitterUsers, startTime, untilId));
    }

    public int CollectReplies()
    {
      return ExecuteJob("CollectReplies", (IList<TwitterUser> twitterUsers, DateTime startTime, long untilId) =>
        _twitterApiService.SearchRepliesForDirectTweetsFromUsers(twitterUsers, startTime, untilId));
    }
  }
}
