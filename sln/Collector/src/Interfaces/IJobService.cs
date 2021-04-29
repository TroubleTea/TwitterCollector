using System;
using TwitterCollector.Model;

namespace TwitterCollector.Interafaces
{
  public interface IJobService
  {
    int CreateJob(string jobName);
    Job GetLastFinishedJob(string jobName);
    void FinishJob(int jobId, DateTime jobFinisedAt, int totalImported, bool rateLimitReached);
  }
}
