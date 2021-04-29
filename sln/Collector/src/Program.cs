using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TwitterCollector
{
  class Program
  {
    static void Main(string[] args)
    {
      var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false, true)
        .Build();

      var services = new ServiceCollection();

      var app = new App(configuration, services);
      app.Start(args);
    }
  }
}
