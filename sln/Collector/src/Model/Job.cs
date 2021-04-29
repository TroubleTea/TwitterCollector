using System;

namespace TwitterCollector.Model
{
  public class Job
  {
    public int Id { get; set; }
    public string JobName { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
    public int TotalTweetsImported { get; set; }
    public bool RateLimitReached { get; set; }
  }
}
