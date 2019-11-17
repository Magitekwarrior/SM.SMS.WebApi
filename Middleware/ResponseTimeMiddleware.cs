using SM.SMS.WebApi.Infrastructure.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SM.SMS.WebApi.Middleware
{
  public class ResponseTimeMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILoggerService _logger;
    private const int ReadChunkBufferLength = 4096;

    public ResponseTimeMiddleware(RequestDelegate next,
      ILoggerService logger)
    {
      _next = next;
      _logger = logger;
    }

    public Task InvokeAsync(HttpContext context)
    {
      var originalRequestBody = context.Request.Body;
      string requestBody;

      try
      {
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

        // Start the Timer using Stopwatch  
        var watch = new Stopwatch();
        watch.Start();

        context.Response.OnStarting(() =>
        {
          // Stop the timer information and calculate the time   
          watch.Stop();
          var milliseconds = watch.ElapsedMilliseconds;


          _logger.LogDebug(string.Format("{0} {1}://{2}{3} {4} took {5}ms.",
            context.Request.Method,
            context.Request.Scheme,
            context.Request.Host,
            context.Request.Path,
            context.Request.QueryString,
            milliseconds),
            requestBody);

          return Task.CompletedTask;
        });

        // Call the next delegate/middleware in the pipeline   
        return this._next(context);
      }
      finally
      {
        context.Request.Body = originalRequestBody;
      }
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
