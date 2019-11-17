using Microsoft.AspNetCore.Http;

namespace SM.SMS.WebApi.Infrastructure.Tracking
{
  public interface ITrackingService
  {
    string GetTrackingIdHeaderFieldName();

    string GetTrackingIdForContext();

    void AddTrackingIdToContext(HttpContext context);

  }
}
