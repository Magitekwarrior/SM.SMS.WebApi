using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;

namespace WebApi
{
  public class Program
  {
    public static void Main(string[] args)
    {
      BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
      var config = new ConfigurationBuilder()
       .SetBasePath(Directory.GetCurrentDirectory())
       .Build();

      return WebHost.CreateDefaultBuilder(args)
          .UseConfiguration(config)
          .UseLibuv(opts => opts.ThreadCount = 30)
          .UseKestrel(options => { options.Limits.KeepAliveTimeout = new System.TimeSpan(0, 5, 0); })
          .ConfigureLogging(logConfig =>
          {
            logConfig.ClearProviders();
          })
          .UseStartup<Startup>().UseUrls("http://*:62883")
          .Build();
    }
  }
}
