using System.Collections.Generic;

namespace TwitterCollector.Model
{
  public class TwitterSearchResult : TwitterPagedResult
    {
      public List<Tweet> Data { get; set; }
    }
}
