using System.Threading.Tasks;
using SM.SMS.Web.Api.ServiceModels;

namespace SM.SMS.WebApi.Service.Contracts
{
  public interface ISMSService
  {
    Task<bool> SendSMS(SendSMSRequest request);
    Task<GetSMSHistoryResponse> GetAllSMSHistory();
  }
}
