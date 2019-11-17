using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SM.SMS.Web.Api.Models;
using SM.SMS.Web.Api.ServiceModels;
using SM.SMS.WebApi.Infrastructure.Logging;
using SM.SMS.WebApi.Infrastructure.Repositories.Contracts;
using SM.SMS.WebApi.Service.Contracts;

namespace SM.SMS.WebApi.Service
{
  public class SMSService : ISMSService
  {
    private readonly ILoggerService _loggerService;
    private readonly ISMSRepo _smsRepo;

    public SMSService(ILoggerService loggerService,
      ISMSRepo smsRepo)
    {
      _loggerService = loggerService;
      _smsRepo = smsRepo;
    }

    public async Task<bool> SendSMS(SendSMSRequest request)
    {
      _loggerService.LogInformation("SendSMS(SendSMSRequest request) - START", request);

      // Normally create a mapper here - in case sms provider requires diff info. (^_^)

      var clickaTellRequest = new ShortMessage()
      {
        CellNumberSendFrom = request.CellNumber,
        CellNumberSendTo = request.SendTo,
        Message = request.Message
      };

      // Normally have a gateway here in case sms provider changes (^_^)

      // No need to try catch, unless there's specific error type we want to look for.
      // Exception handling is done by middleware.

      var success = await _smsRepo.SendSMSClickaTell(clickaTellRequest);

      _loggerService.LogInformation("SendSMS(SendSMSRequest request) - END", request);

      return success;
    }

    public async Task<GetSMSHistoryResponse> GetAllSMSHistory()
    {
      var result = new GetSMSHistoryResponse();

      _loggerService.LogInformation("SendSMS(SendSMSRequest request) - START");

      var response = await _smsRepo.GetSmses();

      _loggerService.LogInformation("SendSMS(SendSMSRequest request) - END");

      // MAP Response to output response object:
      var resultList = new List<SmsHistory>();
      resultList.Add(new SmsHistory()
      {
        CreateDate = DateTime.Now.AddDays(-2),
        CreateUser = "test User",
        Message = "test message",
        Number = "070025146"
      });

      result.SmsHistory = new List<SmsHistory>();
      var smsHistory = result.SmsHistory.ToList();
      smsHistory.AddRange(resultList);
      result.SmsHistory = smsHistory;

      return result;
    }
  }
}
