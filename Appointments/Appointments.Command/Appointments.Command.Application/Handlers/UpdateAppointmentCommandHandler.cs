using Appointments.Command.Application.Commands;
using Appointments.Command.Application.DTOs;
using Appointments.Command.Application.EventProducer;
using Appointments.Command.Application.Exceptions;
using Appointments.Domain.Entities;
using Appointments.Domain.Repositories;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Appointments.Command.Application.Handlers
{
    public class UpdateAppointmentCommandHandler : ICommandHandler<UpdateAppointmentCommand>
    {
        private readonly AppointmentRepository _appointmentRepository;
        private readonly CAMapRepository _mapRepository;
        private readonly ILogger<UpdateAppointmentCommandHandler> _logger;
        private readonly AppointmentEventProducer _eventProducer;
        public UpdateAppointmentCommandHandler(AppointmentRepository appointmentRepository,
                                               ILogger<UpdateAppointmentCommandHandler> logger,
                                               CAMapRepository mapRepository,
                                               AppointmentEventProducer eventProducer)
        {
            _appointmentRepository = appointmentRepository;
            _logger = logger;
            _mapRepository = mapRepository;
            _eventProducer = eventProducer;
        }

        public async Task HandleAsync(UpdateAppointmentCommand command)
        {
            using var session = await _appointmentRepository.GetSession();

            session.StartTransaction();

            var result = await _appointmentRepository.UpdateAppointment(
                DtoConverter.Convert(command)
            );

            if (!result.IsAcknowledged)
            {
                session.AbortTransaction();
                _logger.LogError($"Fail updating appointment. Matched count: {result.MatchedCount}, Modified count: {result.ModifiedCount}, Command body: {command}");
                if (result.MatchedCount == 0) throw new AppointmentsApplicationException("No matching appointment found for user.");
                else if (result.ModifiedCount == 0) throw new AppointmentsApplicationException("Fail updating appointment.");
            }

            var reasult = await _mapRepository.DeleteByAppointmentIdAsync(command.Id);

            if (!result.IsAcknowledged || reasult.DeletedCount == 0)
            {
                session.AbortTransaction();
                _logger.LogError($"Faild cleaning old circle mappings for appointment: {command.Id}");
                throw new AppointmentsApplicationException("Fail updating appointment.");
            }

            var mappings = command.Circles.Select(
                    c => new CircleAppointmentMap
                    {
                        AppointmentId = command.Id,
                        CircleId = c
                    }
                ).ToList();

            await _mapRepository.SaveManyAsync(mappings);

            await _eventProducer.ProduceAsync(
                new AppointmentChangePublicEvent(
                    command.Id,
                    command.UpdaterId,
                    command.Date,
                    command.Circles
                )
            );
            await session.CommitTransactionAsync();
        }

        public async Task HandleAsync(BaseCommand command)
        {
            await HandleAsync((UpdateAppointmentCommand)command);
        }
    }
}