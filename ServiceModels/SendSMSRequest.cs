namespace SM.SMS.Web.Api.ServiceModels
{
  public class SendSMSRequest
  {
    public string CellNumber { get; set; }
    public string SendTo { get; set; }
    public string Message { get; set; }
  }
}
