using SM.SMS.WebApi.Infrastructure.Configurations.ConfigModels;
using SM.SMS.WebApi.Infrastructure.Configurations.Contracts;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SM.SMS.WebApi.Infrastructure.Configurations
{
  public class ConfigService : IConfigService
  {
    protected ServicesEndpoints _servicesEndpoints { get; }
    protected AppSettings _appSettings { get; }
    protected ConnectionStrings _connectionStrings { get; }
    protected IConfiguration _config { get; }

    public ConfigService(IConfiguration config,
      AppSettings appSettings,
      ConnectionStrings connStrings,
      ServicesEndpoints servicesEndpoints)
    {
      _config = config;
      _appSettings = appSettings;
      _servicesEndpoints = servicesEndpoints;
      _connectionStrings = connStrings;
    }

    public string GetServiceEndPoint(string serviceName)
    {
      var endpoint = _servicesEndpoints.Services[serviceName].Url;
      return endpoint;
    }

    public string GetServiceApiKey(string serviceName)
    {
      var apiKey = _servicesEndpoints.Services[serviceName].ApiKey;
      return apiKey;
    }

    /// <summary>
    /// Gets a sql/mysql database connection string that is stored in the app settings as "DefaultConnection"
    /// </summary>
    /// <returns></returns>
    public string GetDefaultConnectionString()
    {
      var connString = _connectionStrings.DefaultConnection;
      return connString;
    }

  }
}