namespace TwitterCollector.Model
{
  public class TwitterPagedResultMeta {
    public long NewestId { get; set; }
    public long OldestId { get; set; }
    public int ResultCount { get; set; }
    public string NextToken { get; set; }
  }
}