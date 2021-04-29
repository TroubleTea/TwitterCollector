using System.Data;

namespace TwitterCollector.Interafaces
{
  public interface IDatabaseService
  {
    IDbConnection GetDbConnection();
  }
}