using System;
using SM.SMS.WebApi.Infrastructure.DomainExceptions;

namespace SM.SMS.Web.Api.Infrastructure.DomainExceptions
{
  public class ReplayDomainException : DomainException
  {
    public ReplayDomainException(string message) : base(message)
    {
    }

    public ReplayDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}
