using Microsoft.AspNetCore.Http;
using System;

namespace SM.SMS.WebApi.Infrastructure.Tracking
{
  public class TrackingService : ITrackingService
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _trackingIdName = "X-Tracking-Id";

    public TrackingService(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public string GetTrackingIdHeaderFieldName()
    {
      return _trackingIdName;
    }

    public string GetTrackingIdForContext()
    {
      var trackingId = _httpContextAccessor.HttpContext.Response.Headers[_trackingIdName].ToString();

      return trackingId;
    }

    public void AddTrackingIdToContext(HttpContext context)
    {
      context.Response.Headers[_trackingIdName] = context.Request.Headers.ContainsKey(_trackingIdName)
        ? context.Request.Headers[_trackingIdName].ToString()
        : GetNewTrackingId();
    }

    private string GetNewTrackingId()
    {
      var result = Guid.NewGuid().ToString();

      return result;
    }
  }
}
