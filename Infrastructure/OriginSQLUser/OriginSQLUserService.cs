using Microsoft.AspNetCore.Http;

namespace SM.SMS.WebApi.Infrastructure.OriginSQLUser
{
  public class OriginSQLUserService : IOriginSQLUserService
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _originSQLUserHeader = "X-Origin-SQLUser";

    public OriginSQLUserService(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }
    public void AddOriginSQLUserToContext(HttpContext context)
    {
      context.Response.Headers[_originSQLUserHeader] = context.Request.Headers.ContainsKey(_originSQLUserHeader)
          ? context.Request.Headers[_originSQLUserHeader].ToString()
          : null;
    }

    public string GetOriginSQLUserForContext()
    {
      var originSQLUser = _httpContextAccessor.HttpContext.Response.Headers[_originSQLUserHeader].ToString();

      return originSQLUser;
    }

    public string GetOriginSQLUserHeaderFieldName()
    {
      return _originSQLUserHeader;
    }
  }
}
