using System;
using System.Net.Http;

namespace SM.SMS.WebApi.Infrastructure.HttpClients.Contracts
{
  public interface IHttpClientFactory : IDisposable
  {
    HttpClient Create(string baseAddress);

    HttpClient CreateClient(string serviceName);

  }
}
