using EventSocket.Application.Commands;

namespace EventSocket.Application.DTOs
{
    public class NotificationProcessedDto
    {
        public Guid NotificationId { get; set; }
        public EventCommandType CommandType { get; set; }
    }
}