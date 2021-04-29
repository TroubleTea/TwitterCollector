using System.Collections.Generic;
using TwitterCollector.Model;

namespace TwitterCollector.Interafaces
{
  public interface ITwitterUserService
  {
    IList<TwitterUser> GetUsers();
  }
}
