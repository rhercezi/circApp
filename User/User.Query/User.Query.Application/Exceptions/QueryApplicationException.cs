using System.Runtime.Serialization;

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