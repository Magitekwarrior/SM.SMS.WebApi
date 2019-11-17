using Microsoft.AspNetCore.Builder;

namespace SM.SMS.WebApi.Middleware
{
  public static class MiddlewareExtensions
  {
    public static IApplicationBuilder UseExceptionHandleMiddleware(this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<ExceptionHandleMiddleware>();
    }

    public static IApplicationBuilder UseResponseTimeMiddleware(this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<ResponseTimeMiddleware>();
    }
  }
}
