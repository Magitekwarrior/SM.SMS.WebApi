using SM.SMS.WebApi.Infrastructure.Logging;
using SM.SMS.WebApi.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SM.SMS.WebApi.Controllers
{
  [Authorize(AuthenticationSchemes = "Basic")]
  [ApiVersion("1.0")]
  [Produces("application/json")]
  [Route("api/v{version:apiVersion}/test")]

  public class TestController : Controller
  {
    private readonly ITestService _testService;

    public TestController(ILoggerService loggerService, ITestService testService)
    {
      _testService = testService;
    }

    [HttpPost, MapToApiVersion("1.0")]
    [Route("TestLogging")]
    public void TestLogging()
    {
      _testService.TestLogging();
    }

    [AllowAnonymous]
    [HttpPost, MapToApiVersion("1.0")]
    [Route("IsAlive")]
    public bool IsAlive()
    {
      return true;
    }

    [AllowAnonymous]
    [HttpPost, MapToApiVersion("1.0")]
    [Route("IsAliveWithDependancyInjection")]
    public bool IsAliveWithDependancyInjection()
    {
      return _testService.IsAliveWithDependancyInjection();
    }
  }
}