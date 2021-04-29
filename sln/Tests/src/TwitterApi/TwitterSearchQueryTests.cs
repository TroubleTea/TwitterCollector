using System;
using System.IO;
using TwitterCollector;
using Xunit;

namespace Tests.TwitterApi
{
  public class TwitterSearchQueryTests
  {
    [Fact]
    public void TwitterSearchQuery_ctor_shoud_throw_ArgumentExcetion_if_query_parameter_is_null_or_empty()
    {
      Assert.Throws<ArgumentException>(() => new TwitterSearchQuery(null));
      Assert.Throws<ArgumentException>(() => new TwitterSearchQuery(""));
    }

    [Fact]
    public void TwitterSearchQuery_ctor_should_throw_ArgumentOutOfRangeException_if_query_parameter_length_exceeds_512_characters()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
        new TwitterSearchQuery(File.ReadAllText("DataSets/513CharactersString.txt")));

      Assert.Throws<ArgumentOutOfRangeException>(() =>
        new TwitterSearchQuery(File.ReadAllText("DataSets/1024CharactersString.txt")));
    }

    [Fact]
    public void TwitterSearchQuery_ctor_should_assign_Query_property()
    {
      var testString = File.ReadAllText("DataSets/512CharactersString.txt");
      var query = new TwitterSearchQuery(testString);

      Assert.Equal(testString, query.Query);
    }
  }
}
