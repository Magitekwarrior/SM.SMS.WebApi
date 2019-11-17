using SM.SMS.WebApi.Infrastructure.Tracking;
using Newtonsoft.Json;
using NLog;
using System;

namespace SM.SMS.WebApi.Infrastructure.Logging
{
  public class LoggerService : ILoggerService
  {
    private ILogger _logger;
    private ITrackingService _trackingService;

    public LoggerService(ITrackingService trackingService)
    {
      _logger = LogManager.GetLogger(this.GetType().FullName);
      _trackingService = trackingService;
    }

    /// <summary>
    /// Log Debug
    /// </summary>
    /// <param name="message">Debug Message</param>
    /// <param name="objects">objects to format into debug message</param>
    public void LogDebug(string message, params object[] objects)
    {
      LogWithEventInfo(LogLevel.Debug, message, objects);
    }

    /// <summary>
    /// Log Information
    /// </summary>
    /// <param name="message">Information Message</param>
    /// <param name="objects">objects to format into info message</param>
    public void LogInformation(string message, params object[] objects)
    {
      LogWithEventInfo(LogLevel.Info, message, objects);
    }

    /// <summary>
    /// Log Information (with an Exception)
    /// </summary>
    /// <param name="ex">The exception to log</param>
    /// <param name="message">Error message to log</param>
    /// <param name="objects">objects to format into error message</param>
    public void LogInformation(Exception ex, string message, params object[] objects)
    {
      LogWithEventInfo(LogLevel.Info, message, ex, objects);
    }


    /// <summary>
    /// Log Error
    /// </summary>
    /// <param name="ex">The exception to log</param>
    /// <param name="message">Error message to log</param>
    /// <param name="objects">objects to format into error message</param>
    public void LogError(Exception ex, string message, params object[] objects)
    {
      LogWithEventInfo(LogLevel.Error, message, ex, objects);
    }

    /// <summary>
    /// Writes a custom event info to the log
    /// </summary>
    /// <param name="level">Level of the Log</param>
    /// <param name="message">The message to be logged</param>
    /// <param name="objects">The list of objects to be logged along with the message</param>
    private void LogWithEventInfo(LogLevel level, string message, params object[] objects)
    {
      LogWithEventInfo(level, message, null, objects);
    }

    /// <summary>
    /// Writes a custom event info to the log file with an exception
    /// </summary>
    /// <param name="level">Level of the Log</param>
    /// <param name="message">The message to be logged</param>
    /// <param name="ex">The exception to be logged</param>
    /// <param name="objects">The list of objects to be logged along with the message</param>
    private void LogWithEventInfo(LogLevel level, string message, Exception ex, params object[] objects)
    {
      var eventInfo = new LogEventInfo(level, _logger.Name, message);
      eventInfo.Exception = ex;

      // Add the tracking id
      var trackingId = _trackingService.GetTrackingIdForContext();
      eventInfo.Properties["TrackingId"] = trackingId;
      eventInfo.Properties["Context"] = JsonConvert.SerializeObject(objects);

      // Log the custom event info
      _logger.Log(eventInfo);
    }
  }
}
