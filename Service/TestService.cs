using System;
using System.Collections.Generic;
using SM.SMS.WebApi.Infrastructure.Logging;
using SM.SMS.WebApi.Service.Contracts;

namespace SM.SMS.WebApi.Service
{
  public class TestService : ITestService
  {
    private readonly ILoggerService _loggerService;

    public TestService(ILoggerService loggerService)
    {
      _loggerService = loggerService;
    }

    public bool IsAliveWithDependancyInjection()
    {
      return true;
    }

    public void TestLogging()
    {
      _loggerService.LogInformation("Information Message", new List<string> { "info string list 1", "info string list 1" }
        , "Info Object Line 1", "Info Object Line 2");

      _loggerService.LogDebug("Debug Message", new List<string> { "debug string list 1", "debug string list 1" }
        , "Debug Object Line 1", "Debug Object Line 2");

      _loggerService.LogError(new Exception("This is a test exception for Log Testing."), "Error Message",
        new object());

      throw new Exception("Unhandled exception for Log Testing.");

    }
  }
}
