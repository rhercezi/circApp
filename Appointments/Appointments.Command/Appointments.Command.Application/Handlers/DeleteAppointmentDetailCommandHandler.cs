using Appointments.Command.Application.Commands;
using Appointments.Command.Application.EventProducer;
using Appointments.Domain.Repositories;
using Core.DTOs;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Appointments.Command.Application.Handlers
{
    public class DeleteAppointmentDetailCommandHandler : IMessageHandler<DeleteAppointmentDetailCommand>
    {
        private readonly AppointmentDetailsRepository _detailsRepository;
        private readonly AppointmentRepository _appointmentRepository;
        private readonly ILogger<DeleteAppointmentDetailCommandHandler> _logger;
        private readonly AppointmentEventProducer _eventProducer;
        public DeleteAppointmentDetailCommandHandler(AppointmentDetailsRepository detailsRepository,
                                                     AppointmentRepository appointmentRepository,
                                                     ILogger<DeleteAppointmentDetailCommandHandler> logger,
                                                     AppointmentEventProducer eventProducer)
        {
            _detailsRepository = detailsRepository;
            _appointmentRepository = appointmentRepository;
            _logger = logger;
            _eventProducer = eventProducer;
        }

        public async Task<BaseResponse> HandleAsync(DeleteAppointmentDetailCommand command)
        {
            var appointment = await _appointmentRepository.GetAppointmentById(command.AppointmentId);

            if (appointment.CreatorId != command.UserId)
            {
                _logger.LogCritical("User mismatch user with the Id: {UserId} tried deleting appointment details {AppointmentId}", command.UserId, command.AppointmentId );
                return new BaseResponse { ResponseCode = 400, Message = "Only the appointment creator can delete the appointment details" };
            }

            var result = await _detailsRepository.DeleteAsync(command.AppointmentId);
            if (!result.IsAcknowledged)
            {
                _logger.LogError("Failed deleting appointment details. DeletedCount = {DeletedCount}, Command = {command}", result.DeletedCount, command);
                return new BaseResponse { ResponseCode = 500, Message = "Failed deleting appointment details" };
            }

            await _eventProducer.ProduceAsync(
                new AppointmentChangePublicEvent(
                    appointment.Id,
                    appointment.Title,
                    EventType.Update,
                    appointment.CreatorId,
                    appointment.StartDate,
                    appointment.Circles
                )
            );

            return new BaseResponse { ResponseCode = 204 };
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((DeleteAppointmentDetailCommand)command);
        }
    }
}