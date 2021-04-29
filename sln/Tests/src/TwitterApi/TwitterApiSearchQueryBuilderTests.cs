using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using TwitterCollector;
using TwitterCollector.Model;

namespace Tests.TwitterApi
{
  public class TwitterApiSearchQueryBuilderTests
  {
    [Fact]
    public void DirectTweetsFromUsers_shoud_throw_ArgumentNullException_if_users_argument_is_empty()
    {
      var users = new List<TwitterUser>();

      Assert.Throws<ArgumentException>(() => TwitterApiSearchQueryBuilder.DirectTweetsFromUsers(users));
    }

    [Fact]
    public void DirectTweetsFromUsers_shoud_throw_ArgumentNullException_if_users_argument_is_null()
    {
      List<TwitterUser> users = null;

      Assert.Throws<ArgumentException>(() => TwitterApiSearchQueryBuilder.DirectTweetsFromUsers(users));
    }

    [Fact]
    public void DirectTweetsFromUsers_test_generated_queries()
    {
      var users = File.ReadAllLines("DataSets/TwitterHandles.txt").Select(l => new TwitterUser{
        TwitterHandle = l
      }).ToList();

      var searchQueries = TwitterApiSearchQueryBuilder.DirectTweetsFromUsers(users);

      Assert.NotNull(searchQueries);
      Assert.NotEmpty(searchQueries);

      searchQueries.ForEach(q => {
        Assert.NotNull(q);
        Assert.NotEmpty(q.Query);
        Assert.True(q.Query.Length <= 512, "Query exceeds maximum length of 512 characters.");
      });

      var mergedQueries = String.Concat(searchQueries.Select(q => q.Query));

      users.ForEach(u => Assert.Contains(u.TwitterHandle, mergedQueries));
    }
  }
}
