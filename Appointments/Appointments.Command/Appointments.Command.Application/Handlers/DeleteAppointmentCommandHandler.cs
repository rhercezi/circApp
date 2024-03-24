using Appointments.Command.Application.Commands;
using Appointments.Command.Application.Exceptions;
using Appointments.Domain.Repositories;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Appointments.Command.Application.Handlers
{
    public class DeleteAppointmentCommandHandler : ICommandHandler<DeleteAppointmentCommand>
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

        public async Task HandleAsync(DeleteAppointmentCommand command)
        {
            using var session = await _appointmentRepository.GetSession();
            session.StartTransaction();

            var result = await _appointmentRepository.DeleteAppointment(command.AppointmentId, command.UserId);
            
            if (!result.IsAcknowledged || result.DeletedCount == 0)
            {
                session.AbortTransaction();
                _logger.LogInformation($"No matching appointment found for user. Deleted count: {result.DeletedCount}, Command body: {command}");
                throw new AppointmentsApplicationException("No matching appointment found for user.");
            }

            var reasult = await _mapRepository.DeleteByAppointmentIdAsync(command.AppointmentId);

            if (!result.IsAcknowledged || reasult.DeletedCount == 0)
            {
                session.AbortTransaction();
                _logger.LogError($"Faild cleaning circle mappings for appointment: {command.Id}");
                throw new AppointmentsApplicationException("Fail deleting appointment.");
            }

            await _detailsRepository.DeleteAsync(command.AppointmentId);

            await session.CommitTransactionAsync();

        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((DeleteAppointmentCommand)command);
        }
    }
}