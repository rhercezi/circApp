using Appointments.Command.Application.Commands;
using Appointments.Domain.Repositories;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Appointments.Command.Application.Handlers
{
    public class DeleteAppointmentCommandHandler : IMessageHandler<DeleteAppointmentCommand>
    {
        private readonly AppointmentRepository _appointmentRepository;
        private readonly CAMapRepository _mapRepository;
        private readonly AppointmentDetailsRepository _detailsRepository;
        private readonly ILogger<DeleteAppointmentCommandHandler> _logger;
        public DeleteAppointmentCommandHandler(AppointmentRepository appointmentRepository,
                                               ILogger<DeleteAppointmentCommandHandler> logger,
                                               CAMapRepository mapRepository,
                                               AppointmentDetailsRepository detailsRepository)
        {
            _appointmentRepository = appointmentRepository;
            _logger = logger;
            _mapRepository = mapRepository;
            _detailsRepository = detailsRepository;
        }

        public async Task<BaseResponse> HandleAsync(DeleteAppointmentCommand command)
        {
            using var session = await _appointmentRepository.GetSession();
            session.StartTransaction();

            var result = await _appointmentRepository.DeleteAppointment(command.AppointmentId, command.UserId);
            
            if (!result.IsAcknowledged && result.DeletedCount == 0)
            {
                session.AbortTransaction();
                _logger.LogInformation($"No matching appointment found for user. Deleted count: {result.DeletedCount}, Command body: {command}");
                return new BaseResponse { ResponseCode = 400, Message = "No matching appointment found for user." };
            }

            var result2 = await _mapRepository.DeleteByAppointmentIdAsync(command.AppointmentId);

            if (!result2.IsAcknowledged && result2.DeletedCount == 0)
            {
                session.AbortTransaction();
                _logger.LogError($"Failed cleaning circle mappings for appointment: {command.Id}");
                return new BaseResponse { ResponseCode = 500, Message = "Failed cleaning circle mappings." };
            }

            await _detailsRepository.DeleteAsync(command.AppointmentId);

            await session.CommitTransactionAsync();
            return new BaseResponse { ResponseCode = 204 };
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((DeleteAppointmentCommand)command);
        }
    }
}