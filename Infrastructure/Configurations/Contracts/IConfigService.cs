namespace SM.SMS.WebApi.Infrastructure.Configurations.Contracts
{
  public interface IConfigService
  {
    string GetServiceEndPoint(string serviceName);
    string GetServiceApiKey(string serviceName);
    string GetDefaultConnectionString();
  }
}
