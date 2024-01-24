namespace User.Command.Application.Exceptions
{
    public class UserSystemException : Exception
    {
        public UserSystemException(string? message) : base(message)
        {
        }

        public UserSystemException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}