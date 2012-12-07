using System;

namespace FakeHttp.Exceptions
{
    public class FakeNotSetupException : Exception
    {
        public FakeNotSetupException(string message) : base(message)
        {
            
        }
    }

    public class NullResponseException : Exception
    {
        public NullResponseException(string message)
            : base(message)
        {

        }

        public NullResponseException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
