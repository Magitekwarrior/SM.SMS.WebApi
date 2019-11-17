using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SM.SMS.WebApi.Infrastructure.HttpClients.Contracts
{
  public interface IHttpClientRepo
  {
    Response PostToWebApi<Request, Response>(Request request,
      string serviceName,
      string version,
      string action,
      string controller,
      [Optional]Dictionary<string, string> customHeaders) where Response : new();

    void PostToWebApi<Request>(Request request,
      string serviceName,
      string version,
      string action,
      string controller,
      [Optional]Dictionary<string, string> customHeaders);

    void PostToWebApi<Request>(Request request,
      string serviceName,
      [Optional] Dictionary<string, string> customHeaders);

    Response PutToWebApi<Request, Response>(Request request,
      string serviceName,
      string version,
      string action,
      string controller,
      [Optional]Dictionary<string, string> customHeaders) where Response : new();

    void PutToWebApi<Request>(Request request,
      string serviceName,
      string version,
      string action,
      string controller,
      [Optional]Dictionary<string, string> customHeaders);

    Response GetFromWebApi<Response>(string serviceName,
      string version,
      string action,
      string controller,
      [Optional]Dictionary<string, object> parameters,
      [Optional]Dictionary<string, string> customHeaders) where Response : new();
  }
}
