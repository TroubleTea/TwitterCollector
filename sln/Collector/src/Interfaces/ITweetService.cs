using System.Collections.Generic;
using TwitterCollector.Model;

namespace TwitterCollector.Interafaces
{
  public interface ITweetService
  {
    int InsertMany(IList<Tweet> tweets, int jobId);
    long GetLowestTweetIdImportedByJob(int jobId);
  }
}