using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql;
using Dapper;
using TwitterCollector.Model;
using TwitterCollector.Interafaces;

namespace TwitterCollector.Services
{
  public class TweetService : ITweetService
  {
    private readonly IDatabaseService _databaseService;
    private readonly ILogger _logger;

    public TweetService(IDatabaseService databaseService, ILoggerFactory loggerFactory)
    {
      _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
      _logger = loggerFactory.CreateLogger<TweetService>();
    }

    private void LogError(NpgsqlException e)
    {
      var message = $@"
      Message: {e.Message}
      ErrorCode: {e.ErrorCode}
      StackTrace: {e.StackTrace}
      ";

      _logger.LogError(message);
    }

    public long GetLowestTweetIdImportedByJob(int jobId)
    {
      using var connection = _databaseService.GetDbConnection();
      var sql = "SELECT MIN(id) FROM sentiment.tweets WHERE job_id = @JobId";
      var tweetId = connection.QueryFirstOrDefault<long>(sql, new { JobId = jobId });

      return tweetId;
    }

    public int InsertMany(IList<Tweet> tweets, int jobId)
    {
      if (tweets == null)
      {
        throw new ArgumentNullException(nameof(tweets));
      }

      if (tweets.Count == 0)
      {
        return 0;
      }

      using var connection = _databaseService.GetDbConnection();
      const string sql = @"
        INSERT INTO sentiment.tweets(
          id,
          text,
          tweeted_at,
          author_id,
          conversation_id,
          job_id
        )
        VALUES(
          @Id,
          @Text,
          @TweetedAt,
          @AuthorId,
          @ConversationId,
          @JobId
        )
        ON CONFLICT (id)
        DO NOTHING;
        ";

      var affectedRows = 0;
      try
      {
        affectedRows = connection.Execute(sql, (object)Enumerable.Select(tweets, t => new
        {
          t.Id,
          t.Text,
          t.AuthorId,
          t.ConversationId,
          TweetedAt = t.CreatedAt,
          JobId = jobId
        }));
      }
      catch (NpgsqlException e)
      {
        LogError(e);
      }

      _logger.LogInformation($"Inserted {affectedRows} tweets");

      return affectedRows;
    }
  }
}
