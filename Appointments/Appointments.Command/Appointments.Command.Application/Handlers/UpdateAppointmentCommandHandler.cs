using Appointments.Command.Application.Commands;
using Appointments.Command.Application.EventProducer;
using Appointments.Command.Application.Exceptions;
using Appointments.Domain.Entities;
using Appointments.Domain.Repositories;
using Core.DTOs;
using Core.Events.PublicEvents;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Appointments.Command.Application.Handlers
{
    public class UpdateAppointmentCommandHandler : IMessageHandler<UpdateAppointmentCommand>
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

        public async Task<BaseResponse> HandleAsync(UpdateAppointmentCommand command)
        {
            using var session = await _appointmentRepository.GetSession();

            session.StartTransaction();

            var appointment = await _appointmentRepository.GetAppointmentById(command.Id);
            if (appointment.CreatorId != command.UpdaterId)
            {
                _logger.LogCritical("User mismatch user with the Id: {UserId} tried updating appointment {AppointmentId}", command.UpdaterId, command.Id );
                return new BaseResponse { ResponseCode = 400, Message = "Only the appointment creator can update the appointment" };
            }

            command.JsonPatchDocument.ApplyTo(appointment);
            var result = await _appointmentRepository.UpdateAppointment(appointment);

            if (!result.IsAcknowledged)
            {
                session.AbortTransaction();
                _logger.LogError("Failed updating appointment. Matched count: {MatchedCount}, Modified count: {ModifiedCount}, Command body: {command}", result.MatchedCount, result.ModifiedCount, command );
                if (result.MatchedCount == 0) return new BaseResponse { ResponseCode = 400, Message = "Appointment not found." };
                else if (result.ModifiedCount == 0) return new BaseResponse { ResponseCode = 400, Message = "No changes detected." };
                else return new BaseResponse { ResponseCode = 500, Message = "Failed updating appointment." };
            }

            var result2 = await _mapRepository.DeleteByAppointmentIdAsync(command.Id);

            if (!result2.IsAcknowledged || result2.DeletedCount == 0)
            {
                session.AbortTransaction();
                _logger.LogError("Failed cleaning old circle mappings for appointment: {Id}", command.Id);
                throw new AppointmentsApplicationException("Fail updating appointment.");
            }

            var mappings = appointment.Circles.Select(
                    c => new CircleAppointmentMap
                    {
                        AppointmentId = appointment.Id,
                        CircleId = c,
                        Date = appointment.StartDate
                    }
                ).ToList();

            await _mapRepository.SaveManyAsync(mappings);

            await _eventProducer.ProduceAsync(
                new AppointmentChangePublicEvent(
                    appointment.Id,
                    appointment.Title,
                    command.UpdaterId,
                    appointment.StartDate,
                    appointment.Circles
                )
            );
            await session.CommitTransactionAsync();

            return new BaseResponse { ResponseCode = 200, Data = appointment };
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((UpdateAppointmentCommand)command);
        }
    }
}