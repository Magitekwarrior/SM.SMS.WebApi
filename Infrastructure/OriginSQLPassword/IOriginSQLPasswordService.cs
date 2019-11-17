using Microsoft.AspNetCore.Http;

namespace SM.SMS.WebApi.Infrastructure.OriginSQLPassword
{
  public interface IOriginSQLPasswordService
  {
    string GetOriginSQLPasswordHeaderFieldName();

    string GetOriginSQLPasswordForContext();

    void AddOriginSQLPasswordToContext(HttpContext context);
  }
}
