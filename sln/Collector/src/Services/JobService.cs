using System;
using System.Linq;
using TwitterCollector.Model;
using TwitterCollector.Interafaces;
using Dapper;

namespace TwitterCollector.Services
{
  public class JobService : IJobService
  {
    private readonly IDatabaseService _databaseService;

    public JobService(IDatabaseService databaseService)
    {
      _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }

    public int CreateJob(string jobName)
    {
      var sql = "INSERT INTO sentiment.jobs(job_name) VALUES(@JobName) RETURNING id";

      var connection = _databaseService.GetDbConnection();
      var jobId = connection.ExecuteScalar<int>(sql, new { JobName = jobName });

      return jobId;
    }

    public Job GetLastFinishedJob(string jobName)
    {
      var sql = @"
      SELECT
        id Id,
        job_name JobName,
        started_at StartedAt,
        finished_at FinishedAt,
        total_tweets_imported TotalTweetsImported,
        rate_limit_reached RateLimitReached
      FROM sentiment.jobs
      WHERE job_name = @JobName AND finished_at IS NOT NULL
      ORDER BY id DESC
      LIMIT 1
      ";

      var connection = _databaseService.GetDbConnection();
      var job = connection.Query<Job>(sql, new { JobName = jobName });

      return job.FirstOrDefault();
    }

    public void FinishJob(int id, DateTime finishedAt, int totalImported, bool rateLimitReached)
    {
      var sql = @"
      UPDATE sentiment.jobs
      SET finished_at = @FinishedAt,
          total_tweets_imported = @TotalImported,
          rate_limit_reached = @RateLimitReached
      WHERE id = @Id
      ";

      var connection = _databaseService.GetDbConnection();
      connection.Execute(sql, new
      {
        Id = id,
        FinishedAt = finishedAt,
        TotalImported = totalImported,
        RateLimitReached = rateLimitReached
      });
    }
  }
}
