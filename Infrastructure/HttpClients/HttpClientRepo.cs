using SM.SMS.WebApi.Infrastructure.HttpClients.Contracts;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;

namespace SM.SMS.WebApi.Infrastructure.HttpClients
{
  public class HttpClientRepo : IHttpClientRepo
  {
    private IHttpClientFactory _httpClientFactory;

    public HttpClientRepo(IHttpClientFactory httpClientFactory)
    {
      _httpClientFactory = httpClientFactory;
    }

    public void PostToWebApi<Request>(Request request,
      string serviceName,
      string version,
      string action,
      string controller,
      [Optional]Dictionary<string, string> customHeaders)
    {
      var httpClient = _httpClientFactory.CreateClient(serviceName);

      if (customHeaders != null)
      {
        foreach (var header in customHeaders)
        {
          httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
      }

      var req = JsonConvert.SerializeObject(request);
      var content = new StringContent(req, Encoding.UTF8, "application/json");

      var serviceUrl = httpClient.BaseAddress.AbsoluteUri + version;
      string requestUri = GetRequestUri(serviceUrl, action, controller);

      var response = httpClient.PostAsync(requestUri, content).Result;
      var result = HttpClientReponse<object>.ReadMessage(response);
      //response.EnsureSuccessStatusCode();
    }

    public Response PostToWebApi<Request, Response>(Request request,
      string serviceName,
      string version,
      string action,
      string controller,
      [Optional]Dictionary<string, string> customHeaders) where Response : new()
    {
      var httpClient = _httpClientFactory.CreateClient(serviceName);

      if (customHeaders != null)
      {
        foreach (var header in customHeaders)
        {
          httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
      }

      var req = JsonConvert.SerializeObject(request);
      var content = new StringContent(req, Encoding.UTF8, "application/json");

      var serviceUrl = httpClient.BaseAddress.AbsoluteUri + version;
      string requestUri = GetRequestUri(serviceUrl, action, controller);

      var response = httpClient.PostAsync(requestUri, content).Result;

      var result = HttpClientReponse<Response>.ReadMessage(response);
      return result;
    }

    public void PostToWebApi<Request>(Request request,
      string serviceName,
      [Optional]Dictionary<string, string> customHeaders)
    {
      var httpClient = _httpClientFactory.CreateClient(serviceName);

      if (customHeaders != null)
      {
        foreach (var header in customHeaders)
        {
          httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
      }

      var req = JsonConvert.SerializeObject(request);
      var content = new StringContent(req, Encoding.UTF8, "application/json");

      var serviceUrl = httpClient.BaseAddress.AbsoluteUri;

      var response = httpClient.PostAsync(serviceUrl, content).Result;

      //var result = HttpClientReponse<object>.ReadMessage(response);
    }

    public Response PutToWebApi<Request, Response>(Request request,
      string serviceName,
      string version,
      string action,
      string controller,
      [Optional]Dictionary<string, string> customHeaders) where Response : new()
    {
      var httpClient = _httpClientFactory.CreateClient(serviceName);

      if (customHeaders != null)
      {
        foreach (var header in customHeaders)
        {
          httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
      }

      var req = JsonConvert.SerializeObject(request);
      var content = new StringContent(req, Encoding.UTF8, "application/json");

      var serviceUrl = httpClient.BaseAddress.AbsoluteUri + version;
      string requestUri = GetRequestUri(serviceUrl, action, controller);

      var response = httpClient.PutAsync(requestUri, content).Result;

      var result = HttpClientReponse<Response>.ReadMessage(response);
      return result;
    }

    public void PutToWebApi<Request>(Request request,
      string serviceName,
      string version,
      string action,
      string controller,
      [Optional]Dictionary<string, string> customHeaders)
    {
      var httpClient = _httpClientFactory.CreateClient(serviceName);

      if (customHeaders != null)
      {
        foreach (var header in customHeaders)
        {
          httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
      }

      var req = JsonConvert.SerializeObject(request);
      var content = new StringContent(req, Encoding.UTF8, "application/json");

      var serviceUrl = httpClient.BaseAddress.AbsoluteUri + version;
      string requestUri = GetRequestUri(serviceUrl, action, controller);

      var response = httpClient.PutAsync(requestUri, content).Result;
      response.EnsureSuccessStatusCode();
    }

    public Response GetFromWebApi<Response>(string serviceName,
      string version,
      string action,
      string controller,
      [Optional]Dictionary<string, object> parameters,
      [Optional]Dictionary<string, string> customHeaders
      ) where Response : new()
    {
      var httpClient = _httpClientFactory.CreateClient(serviceName);

      if (customHeaders != null)
      {
        foreach (var header in customHeaders)
        {
          httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
      }

      var serviceUrl = httpClient.BaseAddress.AbsoluteUri + version;

      string requestUri = string.Empty;
      if (parameters != null)
      {
        requestUri = GetRequestUri(serviceUrl, action, controller, parameters);
      }
      else
      {
        requestUri = GetRequestUri(serviceUrl, action, controller);
      }

      var response = httpClient.GetAsync(requestUri).Result;

      var result = HttpClientReponse<Response>.ReadMessage(response);
      return result;
    }

    private static string GetRequestUri(string serviceUrl,
      string action,
      string controller,
      [Optional]Dictionary<string, object> parameters)
    {
      var baseUri = string.Format("{0}/{1}/{2}",
                                  serviceUrl,
                                  controller,
                                  action);

      var requestUri = string.Format("{0}", baseUri);
      if (parameters != null)
      {
        requestUri = string.Format("{0}?", requestUri);
        foreach (var param in parameters)
        {
          var value = (param.Value != null) ? param.Value.ToString() : "null";

          requestUri = QueryHelpers.AddQueryString(requestUri, param.Key, value);
        }
      }

      return requestUri;
    }
  }
}
