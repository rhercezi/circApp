using Appointments.Command.Application.Commands;
using Appointments.Command.Application.DTOs;
using Appointments.Command.Application.EventProducer;
using Appointments.Command.Application.Exceptions;
using Appointments.Domain.Repositories;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Appointments.Command.Application.Handlers
{
    public class UpdateAppointmentDetailCommandHandler : ICommandHandler<UpdateAppointmentDetailCommand>
    {
        private readonly AppointmentDetailsRepository _detailsRepository;
        private readonly AppointmentRepository _appointmentRepository;
        private readonly ILogger<UpdateAppointmentDetailCommandHandler> _logger;
        private readonly AppointmentEventProducer _eventProducer;
        public UpdateAppointmentDetailCommandHandler(AppointmentDetailsRepository detailsRepository,
                                                     ILogger<UpdateAppointmentDetailCommandHandler> logger,
                                                     AppointmentRepository appointmentRepository,
                                                     AppointmentEventProducer eventProducer)
        {
            _detailsRepository = detailsRepository;
            _logger = logger;
            _appointmentRepository = appointmentRepository;
            _eventProducer = eventProducer;
        }

        public async Task HandleAsync(UpdateAppointmentDetailCommand command)
        {
            if (command.Details != null)
            {
                var appointment = await _appointmentRepository.GetAppointmentById(command.Details.AppointmentId);

                if (appointment.CreatorId != command.UserId)
                {
                    _logger.LogCritical($"User missmatch user with the Id: {command.UserId} tried updating appointment details {command.Details}");
                    throw new AppointmentsApplicationException("Only the appointment creator can update the appointment");
                }

                var result = await _detailsRepository.UpdateAsync(DtoConverter.Convert(command.Details));
                if (!result.IsAcknowledged)
                {
                    _logger.LogError($"Faild updating appointment details. MatchedCount = {result.MatchedCount}, ModifiedCount = {result.ModifiedCount}, Command = {command}");
                    if (result.MatchedCount == 0) throw new AppointmentsApplicationException($"No match found for given appointment details {command}");
                    if (result.ModifiedCount == 0) throw new AppointmentsApplicationException($"Faild updating appointment details with command {command}");
                }

                await _eventProducer.ProduceAsync(
                    new AppointmentChangePublicEvent(
                        appointment.Id,
                        appointment.CreatorId,
                        appointment.Date,
                        appointment.DetailsInCircles
                    )
                );
            }
            else throw new AppointmentsApplicationException("Can not update deteails with null value.");
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((UpdateAppointmentDetailCommand)command);
        }
    }
}