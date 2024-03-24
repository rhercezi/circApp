using Appointments.Command.Application.Commands;
using Appointments.Command.Application.Exceptions;
using Appointments.Domain.Repositories;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Appointments.Command.Application.Handlers
{
    public class DeleteAppointmentDetailCommandHandler : ICommandHandler<DeleteAppointmentDetailCommand>
    {
        private readonly AppointmentDetailsRepository _detailsRepository;
        private readonly AppointmentRepository _appointmentRepository;
        private readonly ILogger<DeleteAppointmentDetailCommandHandler> _logger;
        public DeleteAppointmentDetailCommandHandler(AppointmentDetailsRepository detailsRepository,
                                                     AppointmentRepository appointmentRepository,
                                                     ILogger<DeleteAppointmentDetailCommandHandler> logger)
        {
            _detailsRepository = detailsRepository;
            _appointmentRepository = appointmentRepository;
            _logger = logger;
        }

        public async Task HandleAsync(DeleteAppointmentDetailCommand command)
        {
            var appointment = await _appointmentRepository.GetAppointmentById(command.AppointmentId);

            if (appointment.CreatorId != command.UserId)
            {
                _logger.LogCritical($"User missmatch user with the Id: {command.UserId} tried deleting appointment details {command.AppointmentId}");
                throw new AppointmentsApplicationException("Only the appointment creator can delete the appointment details");
            }

            var result = await _detailsRepository.DeleteAsync(command.AppointmentId);
            if (!result.IsAcknowledged)
            {
                _logger.LogError($"Faild deleting appointment details. DeletedCount = {result.DeletedCount}, Command = {command}");
                throw new AppointmentsApplicationException($"No match found for given appointment details {command}");
            }
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((DeleteAppointmentDetailCommand)command);
        }
    }
}