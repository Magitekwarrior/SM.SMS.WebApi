using System;

namespace SM.SMS.WebApi.Infrastructure.Logging
{
  public interface ILoggerService
  {
    void LogDebug(string debug, params object[] objects);

    void LogInformation(string info, params object[] objects);

    void LogInformation(Exception ex, string message, params object[] objects);

    void LogError(Exception ex, string message, params object[] objects);
  }
}