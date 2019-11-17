using Newtonsoft.Json;
using System;

namespace SM.SMS.WebApi.Infrastructure.DomainExceptions.HttpResponse
{
  public static class HttpErrorResponse
  {
    public static string GetHttpErrorResponse(Exception exception)
    {
      var httpDomainErrorResponse = new HttpDomainErrorResponse() { Error = exception.Message };

      var jsonResponse = JsonConvert.SerializeObject(httpDomainErrorResponse);

      return jsonResponse;
    }
  }
}
