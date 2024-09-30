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
    public class UpdateAppointmentDetailCommandHandler : IMessageHandler<UpdateAppointmentDetailCommand>
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

        public async Task<BaseResponse> HandleAsync(UpdateAppointmentDetailCommand command)
        {
            try
            {
                var appointment = await _appointmentRepository.GetAppointmentById(command.AppointmentId);

                if (appointment.CreatorId != command.UserId)
                {
                    _logger.LogCritical("User mismatch user with the Id: {userId} tried updating appointment details {details}",
                                        command.UserId,
                                        command.PatchDocument);
                    return new BaseResponse { ResponseCode = 400, Message = "Only the appointment creator can update the appointment details" };
                }

                var details = await _detailsRepository.FindAsync(command.AppointmentId);

                command.PatchDocument.ApplyTo(details);

                var result = await _detailsRepository.UpdateAsync(details);
                if (!result.IsAcknowledged)
                {
                    _logger.LogError("Failed updating appointment details. MatchedCount = {matchedCount}, ModifiedCount = {modifiedCount}, Command = {command}",
                                     result.MatchedCount,
                                     result.ModifiedCount,
                                     command);
                    if (result.MatchedCount == 0) return new BaseResponse { ResponseCode = 400, Message = "Appointment details not found." };
                    else if (result.ModifiedCount == 0) return new BaseResponse { ResponseCode = 400, Message = "No changes detected." };
                    else return new BaseResponse { ResponseCode = 500, Message = "Failed updating appointment details." };
                }

                await _eventProducer.ProduceAsync(
                    new AppointmentChangePublicEvent(
                        appointment.Id,
                        appointment.Title,
                        appointment.CreatorId,
                        appointment.StartDate,
                        appointment.DetailsInCircles!
                    )
                );

                appointment.Details = details;
                return new BaseResponse { ResponseCode = 200, Data = appointment };
            }
            catch (Exception e)
            {
                _logger.LogError("Failed updating appointment details with command {command}\n{stackTrace}", command, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Message = "Failed updating appointment details." };
            }
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((UpdateAppointmentDetailCommand)command);
        }
    }
}