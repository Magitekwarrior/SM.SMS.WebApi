using System;
using System.Data.SqlClient;
using SM.SMS.Web.Api.Infrastructure.Connection.Contracts;
using SM.SMS.WebApi.Infrastructure.Configurations.Contracts;

namespace SM.SMS.Web.Api.Infrastructure.Connection
{
  public class ConnectionFactory : IConnectionFactory
  {
    private readonly IConfigService _configService;

    public ConnectionFactory(IConfigService configService)
    {
      _configService = configService;
    }

    public SqlConnection GetNewSqlConnectionWithLoginDetails(SqlConnection oldConnection, string newUserName, string newPassword)
    {
      if (newUserName.Equals(String.Empty) || newPassword.Equals(String.Empty))
      {
        return oldConnection;
      }
      //Default the new connection to the old connection
      SqlConnection newSqlConnection = oldConnection;

      //Create a builder object using the old connection string as the base
      SqlConnectionStringBuilder newBuilder = new SqlConnectionStringBuilder(oldConnection.ConnectionString);

      //Now overwrite the user name and password with the new values
      newBuilder.UserID = newUserName;
      newBuilder.Password = newPassword;
      newBuilder.Pooling = false;

      //Set the new connection
      newSqlConnection.ConnectionString = newBuilder.ConnectionString;

      return newSqlConnection;
    }
  }
}
