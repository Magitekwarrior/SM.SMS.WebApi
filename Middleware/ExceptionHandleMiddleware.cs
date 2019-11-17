using SM.SMS.WebApi.Infrastructure.DomainExceptions;
using SM.SMS.WebApi.Infrastructure.DomainExceptions.HttpResponse;
using SM.SMS.WebApi.Infrastructure.Logging;
using SM.SMS.WebApi.Infrastructure.Tracking;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using SM.SMS.Web.Api.Infrastructure.DomainExceptions;

namespace SM.SMS.WebApi.Middleware
{
  public class ExceptionHandleMiddleware
  {
    private readonly ILoggerService _logger;
    private readonly ITrackingService _trackingService;
    private readonly RequestDelegate _next;
    private const int ReadChunkBufferLength = 4096;

    public ExceptionHandleMiddleware(
      RequestDelegate next,
      ILoggerService logger,
      ITrackingService trackingService)
    {
      _next = next;
      _logger = logger;
      _trackingService = trackingService;
    }

    /// <summary>
    /// Will catch all exceptions in application
    /// Will add tracking Id to the HttpContext for future use
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context /* other scoped dependencies */)
    {
      var originalRequestBody = context.Request.Body;
      string requestBody = string.Empty;

      try
      {
        _trackingService.AddTrackingIdToContext(context);

        try
        {
          context.Request.EnableRewind();

          using (var requestStream = new MemoryStream())
          {
            context.Request.Body.CopyTo(requestStream);
            requestBody = ReadStreamInChunks(requestStream);
          }
        }
        finally
        {
          context.Request.Body.Position = 0;
        }

        await _next(context);
      }
      catch (Exception ex)
      {
        // Capture Exception in our own logs
        await HandleExceptionAsync(context, ex, requestBody);

        bool logAsError = true;

        if (ex is ReplayDomainException)
        {
          logAsError = false;
        }

        if (logAsError)
        {
          var st = new System.Diagnostics.StackTrace(ex, true);
          var frame = st.GetFrame(0);
          var fileName = frame.GetFileName();
          var methodName = frame.GetMethod().Name;
          var line = frame.GetFileLineNumber();
          var column = frame.GetFileColumnNumber();

          string culprit = String.Format("{0} ({1}, line {2}, column {3})", methodName, fileName, line, column);
        }
      }
      finally
      {
        context.Request.Body = originalRequestBody;
      }
    }

    /// <summary>
    /// Handles all exceptions in application
    /// </summary>
    /// <param name="context">The HttpContext for the request</param>
    /// <param name="exception">The exception that was thrown</param>
    /// <returns></returns>
    private Task HandleExceptionAsync(HttpContext context, Exception exception, String requestBody)
    {
      var logMessage = "An unhandled exception has occurred: " + exception.Message;
      var code = (int)HttpStatusCode.BadRequest;
      var logInfo = false;

      var result = HttpErrorResponse.GetHttpErrorResponse(exception);

      if (exception is ReplayDomainException)
      {
        logMessage = "A replay domain validation exception has occurred: " + exception.Message;
        code = DomainException.HttpErrorCode;
        logInfo = true;
      }
      else if (exception is DomainException)
      {
        logMessage = "A domain validation exception has occurred: " + exception.Message;
        code = DomainException.HttpErrorCode;
      }
      else if (exception is InvalidOperationException)
      {
        logMessage = "Authentication invalid: " + exception.Message;
        code = (int)HttpStatusCode.Unauthorized;
      }

      // Custom exceptions with above IF..

      if (logInfo)
      {
        _logger.LogInformation(exception, logMessage,
          string.Format("Original Request: {0} {1}://{2}{3} {4}",
            context.Request.Method,
            context.Request.Scheme,
            context.Request.Host,
            context.Request.Path,
            context.Request.QueryString),
          requestBody);
      }
      else
      {
        _logger.LogError(exception, logMessage,
          string.Format("Original Request: {0} {1}://{2}{3} {4}",
            context.Request.Method,
            context.Request.Scheme,
            context.Request.Host,
            context.Request.Path,
            context.Request.QueryString),
          requestBody);
      }

      context.Response.ContentType = "application/json";
      context.Response.StatusCode = code;

      return context.Response.WriteAsync(result);
    }

    private string ReadStreamInChunks(Stream stream)
    {
      stream.Seek(0, SeekOrigin.Begin);
      string result;
      using (var textWriter = new StringWriter())
      using (var reader = new StreamReader(stream))
      {
        var readChunk = new char[ReadChunkBufferLength];
        int readChunkLength;

        do
        {
          readChunkLength = reader.ReadBlock(readChunk, 0, ReadChunkBufferLength);
          textWriter.Write(readChunk, 0, readChunkLength);
        } while (readChunkLength > 0);

        result = textWriter.ToString();
      }

      return result;
    }
  }
}
