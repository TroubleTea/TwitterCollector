using System;
using System.Linq;
using System.Collections.Generic;
using Dapper;
using TwitterCollector.Model;
using TwitterCollector.Interafaces;

namespace TwitterCollector.Services
{
  public class TwitterUserService : ITwitterUserService
  {
    private readonly IDatabaseService _datadabaseService;
    public TwitterUserService(IDatabaseService databaseService)
    {
      _datadabaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }

    public IList<TwitterUser> GetUsers()
    {
      var connection = _datadabaseService.GetDbConnection();

      var sql = "SELECT *, twitter_handle TwitterHandle FROM sentiment.twitter_users";
      var result = connection.Query<TwitterUser>(sql);

      return result.ToList();
    }
  }
}
