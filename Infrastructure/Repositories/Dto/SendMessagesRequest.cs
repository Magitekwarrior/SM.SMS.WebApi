namespace SM.SMS.Web.Api.Infrastructure.Repositories.Dto
{
  public class SendMessagesRequest
  {
    public string apiKey { get; set; }
    public string to { get; set; }
    public string content { get; set; }

  }
}
