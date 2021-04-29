using System;

namespace TwitterCollector.Model {
  public class Tweet {
    public long Id { get; set; }
    public long AuthorId { get; set; }
    public long  ConversationId { get; set; }
    public int JobId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Text { get; set; }
    public bool IsReply { get; set; }
  }
}