using System;
using System.Collections.Generic;

namespace SM.SMS.Web.Api.ServiceModels
{
  public class GetSMSHistoryResponse
  {
    public IEnumerable<SmsHistory> SmsHistory { get; set; }
  }

  public class SmsHistory
  {
    public string Number { get; set; }
    public string Message { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateUser { get; set; }
  }

}