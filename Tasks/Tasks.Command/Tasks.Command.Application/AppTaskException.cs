namespace Tasks.Command.Application.Exceptions
{
    public class AppTaskException : Exception
    {
        public AppTaskException(string? message) : base(message)
        {
        }

        public AppTaskException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}