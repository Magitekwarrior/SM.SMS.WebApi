using System.Collections.Generic;

namespace SM.SMS.WebApi.Infrastructure.Configurations.ConfigModels
{
  public class ServicesEndpoints
  {
    public Dictionary<string, ServiceAttributes> Services { get; set; } 
  }

  public partial class ServiceAttributes
  {
    public string Url { get; set; }
    public string ApiKey { get; set; }
  }
}
