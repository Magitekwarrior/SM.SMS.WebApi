using System.Data.SqlClient;

namespace SM.SMS.Web.Api.Infrastructure.Connection.Contracts
{
  public interface IConnectionFactory
  {
    SqlConnection GetNewSqlConnectionWithLoginDetails(SqlConnection oldConnection, string newUserName, string newPassword);
  }
}
