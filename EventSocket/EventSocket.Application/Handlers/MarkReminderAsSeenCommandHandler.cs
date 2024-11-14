using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using EventSocket.Application.Commands;
using EventSocket.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace EventSocket.Application.Handlers
{
    public class MarkReminderAsSeenCommandHandler : IMessageHandler<MarkReminderAsSeenCommand>
    {
        private readonly ReminderRepository _reminderRepository;
        private readonly ILogger<MarkReminderAsSeenCommandHandler> _logger;

        public MarkReminderAsSeenCommandHandler(ReminderRepository reminderRepository,
                                                ILogger<MarkReminderAsSeenCommandHandler> logger)
        {
            _reminderRepository = reminderRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(MarkReminderAsSeenCommand message)
        {
            try
            {
                await _reminderRepository.MarkAsSeenAsync(message.ReminderId);
                return new BaseResponse { ResponseCode = 200 };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred marking reminder as seen: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500 };
            }
        }

        public Task<BaseResponse> HandleAsync(BaseMessage message)
        {
            return HandleAsync((MarkReminderAsSeenCommand)message);
        }
    }
}