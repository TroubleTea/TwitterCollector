using System;
using TwitterCollector.Services;
using TwitterCollector.Options;
using TwitterCollector.Interafaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Diagnostics;
using CommandLine;

namespace TwitterCollector
{
  public class App
  {
    private readonly IConfiguration _configuratuin;
    private readonly IServiceCollection _services;
    private IServiceProvider _serviceProvider;

    public App(IConfiguration configuration, IServiceCollection services)
    {
      _configuratuin = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public void Start(string[] args)
    {
      Configure();

      var collectorService = _serviceProvider.GetService<ICollectorService>();

      Parser.Default.ParseArguments<CommandLineOptions>(args)
        .WithParsed(o =>
        {
          if (o.Job == JobType.CollectTweets)
          {
            Collect(() => collectorService.CollectTweets());
          }

          if (o.Job == JobType.CollectReplies)
          {
            Collect(() => collectorService.CollectReplies());
          }
        });
    }

    private void Configure()
    {
      _services.AddOptions<DatabaseOptions>().Bind(_configuratuin.GetSection(nameof(DatabaseOptions)));
      _services.AddOptions<TwitterApiOptions>().Bind(_configuratuin.GetSection(nameof(TwitterApiOptions)));

      _services.AddLogging(builder =>
        builder.AddConsole().AddFile("Logs/collector-{Date}.log"))
          .AddTransient<IRestClient, RestClient>()
          .AddTransient<IDatabaseService, DatabaseService>()
          .AddTransient<ITwitterUserService, TwitterUserService>()
          .AddTransient<ITweetService, TweetService>()
          .AddTransient<ITwitterApiService, TwitterApiService>()
          .AddSingleton<ICollectorService, CollectorService>()
          .AddTransient<IJobService, JobService>();

      _serviceProvider = _services.BuildServiceProvider();
    }

    private void Collect(Func<int> collectFunc)
    {
      var logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

      var watch = new Stopwatch();
      watch.Start();

      var tweetsCount = collectFunc();

      watch.Stop();
      logger.LogInformation($"Retrieved {tweetsCount} tweets in {watch.Elapsed.TotalSeconds} seconds");
    }
  }
}
