using System;
using System.Data;
using TwitterCollector.Interafaces;
using TwitterCollector.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace TwitterCollector
{
  public class DatabaseService : IDatabaseService
  {
    private readonly DatabaseOptions _databaseOptions;
    public DatabaseService(IOptions<DatabaseOptions> databaseOptions)
    {
      _databaseOptions = databaseOptions?.Value ?? throw new ArgumentNullException(nameof(databaseOptions));
    }

    public IDbConnection GetDbConnection()
    {
      var builder = new NpgsqlConnectionStringBuilder()
      {
        Host = _databaseOptions.Host,
        Port = _databaseOptions.Port,
        Username = _databaseOptions.Username,
        Password = _databaseOptions.Password,
        Database = _databaseOptions.Database
      };

      var connectionString = builder.ToString();
      var connection = new NpgsqlConnection(connectionString);

      return connection;
    }
  }
}