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
    public class AddAppointmentDetailsCommandHandler : ICommandHandler<AddAppointmentDetailsCommand>
    {
        private readonly AppointmentDetailsRepository _detailsRepository;
        private readonly AppointmentRepository _appointmentRepository;
        private readonly ILogger<AddAppointmentDetailsCommandHandler> _logger;
        private readonly AppointmentEventProducer _eventProducer;
        public AddAppointmentDetailsCommandHandler(AppointmentDetailsRepository detailsRepository,
                                                   AppointmentRepository appointmentRepository,
                                                   ILogger<AddAppointmentDetailsCommandHandler> logger,
                                                   AppointmentEventProducer eventProducer)
        {
            _detailsRepository = detailsRepository;
            _appointmentRepository = appointmentRepository;
            _logger = logger;
            _eventProducer = eventProducer;
        }

        public async Task HandleAsync(AddAppointmentDetailsCommand command)
        {
            if (command.Details != null)
            {
                var appointment = await _appointmentRepository.GetAppointmentById(command.Details.AppointmentId);

                if (appointment.CreatorId != command.UserId)
                {
                    _logger.LogCritical($"User missmatch user with the Id: {command.UserId} tried adding appointment details {command.Details}");
                    throw new AppointmentsApplicationException("Only the appointment creator can add the appointment details");
                }

                await _detailsRepository.SaveAsync(DtoConverter.Convert(command.Details));

                await _eventProducer.ProduceAsync(
                    new AppointmentChangePublicEvent(
                        appointment.Id,
                        appointment.CreatorId,
                        appointment.Date,
                        appointment.DetailsInCircles
                    )
                );
            }
            else throw new AppointmentsApplicationException("Can not add null value to appointment details.");
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((AddAppointmentDetailsCommand)command);
        }
    }
}