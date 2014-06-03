using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Database.Exceptions
{
    [Serializable]
    public class UnknowDatabaseTypeException : Exception
    {
        public UnknowDatabaseTypeException()
        { }

        public UnknowDatabaseTypeException(string message)
            : base(message)
        { }

        public UnknowDatabaseTypeException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public UnknowDatabaseTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
