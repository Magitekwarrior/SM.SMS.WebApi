using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SM.SMS.Web.Api.ServiceModels;
using SM.SMS.WebApi.Infrastructure.Logging;
using SM.SMS.WebApi.Service.Contracts;

namespace SM.SMS.WebApi.Controllers
{
  [ApiVersion("1.0")]
  [Produces("application/json")]
  [Route("api/v{version:apiVersion}/SMS")]
  public class SMSController : Controller
  {
    private readonly ISMSService _smsService;
    private readonly ILoggerService _loggerService;

    public SMSController(ISMSService smsService, ILoggerService loggerService)
    {
      _smsService = smsService;
      _loggerService = loggerService;
    }

    [HttpPost, MapToApiVersion("1.0")]
    [Route("SaveSMS")]
    public async Task<bool> SendSMS([FromBody] SendSMSRequest request)
    {
      _loggerService.LogInformation("SMS/SaveSMS(SendSMSRequest request)", request);

      return await _smsService.SendSMS(request);
    }

    [HttpGet, MapToApiVersion("1.0")]
    [Route("GetAllSMSHistory")]
    public async Task<GetSMSHistoryResponse> GetAllSMSHistory()
    {
      return await _smsService.GetAllSMSHistory();
    }

  }
}
