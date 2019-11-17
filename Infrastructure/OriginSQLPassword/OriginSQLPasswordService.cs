using Microsoft.AspNetCore.Http;

namespace SM.SMS.WebApi.Infrastructure.OriginSQLPassword
{
  public class OriginSQLPasswordService : IOriginSQLPasswordService
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _originSQLPasswordHeader = "X-Origin-SQLPassword";

    public OriginSQLPasswordService(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }
    public void AddOriginSQLPasswordToContext(HttpContext context)
    {
      context.Response.Headers[_originSQLPasswordHeader] = context.Request.Headers.ContainsKey(_originSQLPasswordHeader)
          ? context.Request.Headers[_originSQLPasswordHeader].ToString()
          : null;
    }

    public string GetOriginSQLPasswordForContext()
    {
      var originSQLPassword = _httpContextAccessor.HttpContext.Response.Headers[_originSQLPasswordHeader].ToString();

      return originSQLPassword;
    }

    public string GetOriginSQLPasswordHeaderFieldName()
    {
      return _originSQLPasswordHeader;
    }
  }
}
