using System;

namespace TwitterCollector
{
  public class TwitterSearchQuery
  {
    public const int SearchQueryCharLimit = 512;
    public string Query { get; }

    public TwitterSearchQuery(string query)
    {
      if (string.IsNullOrEmpty(query))
      {
        throw new ArgumentException("Search query cannot be empty");
      }

      if (query.Length > SearchQueryCharLimit)
      {
        throw new ArgumentOutOfRangeException(nameof(query), "Twitter search queries can only be 512 characters long.");
      }

      Query = query;
    }
  }
}
