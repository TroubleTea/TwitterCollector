using CommandLine;

namespace TwitterCollector.Options
{
  public enum JobType
  {
    CollectTweets,
    CollectReplies
  }
  public class CommandLineOptions
  {
    [Option('j',"job", Required = true, HelpText = "Select the job to run. CollectTweets or CollectResponses")]
    public JobType Job { get; set; }
  }
}