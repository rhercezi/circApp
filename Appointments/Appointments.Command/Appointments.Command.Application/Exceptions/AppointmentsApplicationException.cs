namespace Appointments.Command.Application.Exceptions
{
    public class AppointmentsApplicationException : Exception
    {
        public AppointmentsApplicationException(string? message) : base(message)
        {
        }

        public AppointmentsApplicationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}