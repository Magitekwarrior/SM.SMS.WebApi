using SM.SMS.WebApi.Infrastructure.DomainExceptions;
using SM.SMS.WebApi.Infrastructure.DomainExceptions.HttpResponse;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using SM.SMS.Web.Api.Infrastructure.DomainExceptions;

namespace SM.SMS.WebApi.Infrastructure.HttpClients
{
  public static class HttpClientReponse<T>
  {
    /// <summary>
    /// Read the message from HttpResponseMessage and throw and ReplayDomainException if we get our internal Http error code: 460
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <returns></returns>
    public static T ReadMessage(HttpResponseMessage responseMessage)
    {
      var responseData = responseMessage.Content.ReadAsStringAsync().Result;

      var httpCode = (int)responseMessage.StatusCode;

      var domainExceptionHttpCode = DomainException.HttpErrorCode;

      if (httpCode == domainExceptionHttpCode) //our domain 
      {
        var httpDomainError = JsonConvert.DeserializeObject<HttpDomainErrorResponse>(responseData);

        throw new ReplayDomainException(httpDomainError.Error);//.ErrorMessage);
      }
      else if (httpCode == 401) //Might need to go into more detail here
      {
        throw new InvalidOperationException();
      }
      else if (httpCode >= 400) //Might need to go into more detail here
      {
        throw new Exception(responseData);
      }

      //success
      return JsonConvert.DeserializeObject<T>(responseData);
    }
  }
}
