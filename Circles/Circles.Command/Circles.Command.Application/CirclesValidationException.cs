namespace Circles.Command.Application
{
    public class CirclesValidationException : Exception
    {
        public CirclesValidationException(string? message) : base(message)
        {
        }

        public CirclesValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}