using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace User.Query.Application.Exceptions
{
    public class QueryApplicationException : Exception
    {
        public QueryApplicationException(string? message) : base(message)
        {
        }

        public QueryApplicationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected QueryApplicationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}