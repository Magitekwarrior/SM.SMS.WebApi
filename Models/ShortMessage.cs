using System;

namespace SM.SMS.Web.Api.Models
{
  public class ShortMessage
  {
    public string CellNumberSendTo { get; set; }
    public string CellNumberSendFrom { get; set; }
    public string Message { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateUser { get; set; }
  }
}
