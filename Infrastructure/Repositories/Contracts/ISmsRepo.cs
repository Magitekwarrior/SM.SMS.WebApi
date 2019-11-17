using System.Collections.Generic;
using System.Threading.Tasks;
using SM.SMS.Web.Api.Models;

namespace SM.SMS.WebApi.Infrastructure.Repositories.Contracts
{
  public interface ISMSRepo
  {
    Task<bool> SendSMSClickaTell(ShortMessage request);
    Task<IEnumerable<ShortMessage>> GetSmses();
  }
}