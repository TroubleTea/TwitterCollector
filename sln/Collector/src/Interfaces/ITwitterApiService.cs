using System;
using System.Collections.Generic;
using TwitterCollector.Model;

namespace TwitterCollector.Interafaces
{

  public interface ITwitterApiService
  {
    IEnumerable<TwitterSearchResult> SearchDirectTweetsFromUsers(IList<TwitterUser> twitterUsers, DateTime startTime, long untilId);
    IEnumerable<TwitterSearchResult> SearchRepliesForDirectTweetsFromUsers(IList<TwitterUser> twitterUsers, DateTime startTime, long untilId);
  }
}
