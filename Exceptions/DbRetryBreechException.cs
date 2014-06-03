using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Database.Exceptions
{
    [Serializable]
    public class DbRetryCountBreachException : Exception
    {
        public DbRetryCountBreachException()
        { }

        public DbRetryCountBreachException(string message)
            : base(message)
        { }

        public DbRetryCountBreachException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public DbRetryCountBreachException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
