namespace User.Command.Application.Exceptions
{
    public class UserValidationException : Exception
    {
        public UserValidationException(string? message) : base(message)
        {
        }

        public UserValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}