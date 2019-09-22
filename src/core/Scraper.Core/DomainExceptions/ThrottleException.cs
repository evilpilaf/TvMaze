using System;

namespace Scraper.Core.DomainExceptions
{
    public class ThrottleException : Exception
    {
    }

    public class NotFoundException : Exception
    {
    }

    public class ApiCallException : Exception
    {
    }

    public class PersistenceException : Exception
    {
        public PersistenceException(string message, Exception innerException)
                    : base(message, innerException)
        {

        }
    }
}
