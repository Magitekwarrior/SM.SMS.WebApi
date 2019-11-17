using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SM.SMS.Web.Api.Infrastructure.Connection.Contracts;
using SM.SMS.Web.Api.Infrastructure.Repositories.Dapper;
using SM.SMS.Web.Api.Models;
using SM.SMS.WebApi.Infrastructure.Configurations.Contracts;
using SM.SMS.WebApi.Infrastructure.HttpClients.Contracts;
using SM.SMS.WebApi.Infrastructure.Logging;
using SM.SMS.WebApi.Infrastructure.OriginSQLPassword;
using SM.SMS.WebApi.Infrastructure.OriginSQLUser;
using SM.SMS.WebApi.Infrastructure.Repositories.Contracts;

namespace SM.SMS.WebApi.Infrastructure.Repositories
{
  public class SmsRepo : ISMSRepo
  {
    private string connection;
    private ILoggerService _loggerService;
    private readonly IHttpClientRepo _httpClientRepo;
    private readonly IOriginSQLUserService _originSQLUserService;
    private readonly IOriginSQLPasswordService _originSQLPasswordService;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IConfigService _configService;

    public SmsRepo(IHttpClientRepo httpClientRepo,
      ILoggerService loggerService,
      IConfigService configService,
      IOriginSQLUserService originSQLUserService,
      IOriginSQLPasswordService originSQLPasswordService,
      IConnectionFactory connectionFactory)
    {
      _httpClientRepo = httpClientRepo;
      _originSQLUserService = originSQLUserService;
      _originSQLPasswordService = originSQLPasswordService;
      _connectionFactory = connectionFactory;
      _configService = configService;
      connection = configService.GetDefaultConnectionString();
      _loggerService = loggerService;
    }

    public async Task<bool> SendSMSClickaTell(ShortMessage smsRequest)
    {
      // Send to ClickaTell
      // var SendMessagesResponse =  https://platform.clickatell.com/messages/http/send?apiKey=SendMessagesRequest.apikey==&to=SendMessagesRequest.to&content=SendMessagesRequest.content
      // If (SendMessagesResponse != null) 
      // return true;
      // else
      // return false;

      return true;
    }

    public async Task<IEnumerable<ShortMessage>> GetSmses()
    {
      try
      {
        /*
        var sqlStoredProc = "xxx";

        var response = DapperRepo.GetFromStoredProc<ShortMessage>
               (storedProcedureName: sqlStoredProc,
                 dbconnectionString: _configService.GetDefaultConnectionString(),
                         sqltimeout: 1000
                );

        return response.ToList();
        */

        var resultList = new List<ShortMessage>();
        resultList.Add(new ShortMessage()
        {
          CellNumberSendFrom = "0721114512",
          CellNumberSendTo = "0891514532",
          CreateDate = DateTime.Now.AddDays(-2),
          CreateUser = "testUser",
          Message = "previous message"
        });

        return resultList;

      }
      catch (Exception ex)
      {
        _loggerService.LogError(ex, "Failed to get SMS History", null);

        return new List<ShortMessage>();
      }
    }

  }
}