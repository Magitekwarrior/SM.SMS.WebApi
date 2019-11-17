using System;

namespace SM.SMS.WebApi.Infrastructure.DomainExceptions
{
  public class DomainException : Exception
  {
    public DomainException(string message) : base(message)
    {

    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {

    }

    public static int HttpErrorCode
    {
      get { return 460; }
    }

  }
}
