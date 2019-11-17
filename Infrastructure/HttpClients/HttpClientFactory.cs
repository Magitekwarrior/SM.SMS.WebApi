using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;
using SM.SMS.WebApi.Infrastructure.Configurations.Contracts;
using SM.SMS.WebApi.Infrastructure.HttpClients.Contracts;
using SM.SMS.WebApi.Infrastructure.OriginSQLPassword;
using SM.SMS.WebApi.Infrastructure.OriginSQLUser;
using SM.SMS.WebApi.Infrastructure.Tracking;

namespace SM.SMS.WebApi.Infrastructure.HttpClients
{
  /// <summary>
  /// This class will intend is that we can reuse HttpClient instances
  /// </summary>
  public sealed class HttpClientFactory : IHttpClientFactory
  {
    private readonly ITrackingService _trackingService;

    private readonly IConfigService _configService;
    private readonly IOriginSQLUserService _originSQLUserService;
    private readonly IOriginSQLPasswordService _originSQLPasswordService;

    //The instance that will keep all HttpClient instances per baseAddress
    private readonly ConcurrentDictionary<Uri, HttpClient> _httpClients;

    public HttpClientFactory(ITrackingService trackingService, IConfigService configService,
      IOriginSQLUserService originSQLUserService,
      IOriginSQLPasswordService originSQLPasswordService)
    {
      _trackingService = trackingService;
      _configService = configService;
      _originSQLUserService = originSQLUserService;
      _originSQLPasswordService = originSQLPasswordService;
      _httpClients = new ConcurrentDictionary<Uri, HttpClient>();
    }

    /// <summary>
    /// Returns an instance of HttpClient
    /// </summary>
    /// <param name="baseAddress">the key that will be used to create a new instance of HttpClient</param>
    /// <returns></returns>
    public HttpClient Create(string baseAddress)
    {
      var client = _httpClients.GetOrAdd(new Uri(baseAddress),
          b => new HttpClient { BaseAddress = b });

      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

      var trackingIdHeaderFieldName = _trackingService.GetTrackingIdHeaderFieldName();
      client.DefaultRequestHeaders.Remove(trackingIdHeaderFieldName);
      var trakingId = _trackingService.GetTrackingIdForContext();
      client.DefaultRequestHeaders.Add(trackingIdHeaderFieldName, trakingId);

      // Add OriginUser to header
      var originSQLUserHeaderFieldName = _originSQLUserService.GetOriginSQLUserHeaderFieldName();
      client.DefaultRequestHeaders.Remove(originSQLUserHeaderFieldName);
      var originSQLUser = _originSQLUserService.GetOriginSQLUserForContext();
      client.DefaultRequestHeaders.Add(originSQLUserHeaderFieldName, originSQLUser);

      // Add OriginUser to header
      var originSQLPasswordHeaderFieldName = _originSQLPasswordService.GetOriginSQLPasswordHeaderFieldName();
      client.DefaultRequestHeaders.Remove(originSQLPasswordHeaderFieldName);
      var originSQLPassword = _originSQLPasswordService.GetOriginSQLPasswordForContext();
      client.DefaultRequestHeaders.Add(originSQLPasswordHeaderFieldName, originSQLPassword);

      AddTracking(client);

      return client;
    }
    private void AddTracking(HttpClient client)
    {
      var trackingIdHeaderFieldName = _trackingService.GetTrackingIdHeaderFieldName();
      client.DefaultRequestHeaders.Remove(trackingIdHeaderFieldName);
      var trakingId = _trackingService.GetTrackingIdForContext();
      client.DefaultRequestHeaders.Add(trackingIdHeaderFieldName, trakingId);
    }

    /// <summary>
    /// will create a HttpClient and add Jwt to the header
    /// </summary>
    /// <param name="serviceName">service name in the configuration</param>
    /// <returns></returns>
    public HttpClient CreateClient(string serviceName)
    {
      var serviceUrl = _configService.GetServiceEndPoint(serviceName);

      var client = Create(serviceUrl);

      return client;
    }

    public void Dispose()
    {
      foreach (var httpClient in _httpClients.Values)
      {
        httpClient.Dispose();
      }
    }
  }
}
