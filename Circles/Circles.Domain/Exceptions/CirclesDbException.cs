using System.Data.Common;

namespace Circles.Domain.Exceptions
{
    public class CirclesDbException : DbException
    {
        public CirclesDbException(string? message) : base(message)
        {
        }

        public CirclesDbException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}