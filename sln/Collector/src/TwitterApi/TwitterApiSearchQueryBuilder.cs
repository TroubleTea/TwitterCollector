using System;
using System.Collections.Generic;
using TwitterCollector.Model;

namespace TwitterCollector
{
  public class TwitterApiSearchQueryBuilder
  {
    /// <summary>
    /// Returns a list of Twitter API search queries that will be limited to the tweets of the provided list of users.
    /// Instead of returning one large query, it will split them up to comply with Twitter's limit of 512 characters
    /// for search query strings.
    /// <summary/>
    public static List<TwitterSearchQuery> DirectTweetsFromUsers(IList<TwitterUser> users)
    {
      if (users == null || users.Count == 0)
      {
        throw new ArgumentException("Users list cannot be empty");
      }

      const string queryEnd = ") -is:reply -is:retweet";
      var twitterSearchQueries = new List<TwitterSearchQuery>();
      var lastIndex = 0;

      while (lastIndex < users.Count - 1)
      {
        var twitterSearchQuery = "(";

        for (var i = lastIndex; i < users.Count; i++)
        {
          var fromQueryOperators = $"from:{users[i].TwitterHandle}";
          if (i > lastIndex)
          {
            fromQueryOperators = $" OR {fromQueryOperators}";
          }
          lastIndex = i;

          var newQuery = twitterSearchQuery + fromQueryOperators;
          if (newQuery.Length > (TwitterSearchQuery.SearchQueryCharLimit - queryEnd.Length))
          {
            break;
          }

          twitterSearchQuery = newQuery;
        }

        twitterSearchQuery += queryEnd;
        twitterSearchQueries.Add(new TwitterSearchQuery(twitterSearchQuery));
      }

      return twitterSearchQueries;
    }

    public static List<TwitterSearchQuery> DirectRepliesToTweetsFromUsers(IList<TwitterUser> users)
    {
      if (users == null || users.Count == 0)
      {
        throw new ArgumentException("Users list cannot be empty");
      }

      const string queryEnd = ") is:reply -is:retweet";
      var twitterSearchQueries = new List<TwitterSearchQuery>();
      var lastIndex = 0;

      while (lastIndex < users.Count - 1)
      {
        var twitterSearchQuery = "(";

        for (var i = lastIndex; i < users.Count; i++)
        {
          var fromQueryOperators = $"to:{users[i].TwitterHandle}";
          if (i > lastIndex)
          {
            fromQueryOperators = $" OR {fromQueryOperators}";
          }
          lastIndex = i;

          var newQuery = twitterSearchQuery + fromQueryOperators;
          if (newQuery.Length > (TwitterSearchQuery.SearchQueryCharLimit - queryEnd.Length))
          {
            break;
          }

          twitterSearchQuery = newQuery;
        }

        twitterSearchQuery += queryEnd;
        twitterSearchQueries.Add(new TwitterSearchQuery(twitterSearchQuery));
      }

      return twitterSearchQueries;
    }
  }
}
