using Microsoft.AspNetCore.Http;

namespace SM.SMS.WebApi.Infrastructure.OriginSQLUser
{
  public interface IOriginSQLUserService
  {
    string GetOriginSQLUserHeaderFieldName();

    string GetOriginSQLUserForContext();

    void AddOriginSQLUserToContext(HttpContext context);
  }
}
