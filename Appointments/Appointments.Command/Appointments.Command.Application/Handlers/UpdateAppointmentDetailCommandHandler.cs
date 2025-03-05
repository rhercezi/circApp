using Appointments.Command.Application.Commands;
using Appointments.Command.Application.EventProducer;
using Appointments.Command.Application.Utilities;
using Appointments.Domain.Entities;
using Appointments.Domain.Repositories;
using Core.DAOs;
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
        private readonly ReminderRepository _reminderRepository;
        public UpdateAppointmentDetailCommandHandler(AppointmentDetailsRepository detailsRepository,
                                                     ILogger<UpdateAppointmentDetailCommandHandler> logger,
                                                     AppointmentRepository appointmentRepository,
                                                     AppointmentEventProducer eventProducer,
                                                     ReminderRepository reminderRepository)
        {
            _detailsRepository = detailsRepository;
            _logger = logger;
            _appointmentRepository = appointmentRepository;
            _eventProducer = eventProducer;
            _reminderRepository = reminderRepository;
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
                if (details != null) {
                    details.Reminders = ConvertModelsToDtos(_reminderRepository.FindManyAsync(command.AppointmentId).Result);
                }

                try
                {
                    command.PatchDocument.ApplyTo(details);
                }
                catch (Exception e)
                {
                    _logger.LogError("Failed applying patch document {document} to appointment details {details}\n{stackTrace}", command.PatchDocument, details, e.StackTrace);
                    return new BaseResponse { ResponseCode = 400, Message = "Failed applying patch document to appointment details." };
                }

                await _reminderRepository.DeleteAsync(command.AppointmentId);
                details.Reminders?.ForEach(
                    async r =>
                    {
                        try
                        {
                            var reminder = new ReminderModel
                            {
                                Id = Guid.NewGuid(),
                                InCircles = r.JustForUser ? null : appointment.DetailsInCircles,
                                UserId = r.JustForUser ? appointment.CreatorId : null,
                                TargetType = ReminderTargetType.Appointment,
                                TargetId = appointment.Id,
                                Message = r.Message,
                                Time = r.Time,
                                JustForUser = r.JustForUser
                            };
                            await _reminderRepository.SaveAsync(reminder);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError("Failed saving reminder for appointment {appointmentId}\n{stackTrace}", appointment.Id, e.StackTrace);
                        }
                    }
                );

                details.Reminders?.Clear();

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
                        EventType.Update,
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

        private static List<Reminder>? ConvertModelsToDtos(List<ReminderModel>? reminders)
        {
            return reminders?.Select(r => ModelToDtoConverter.ConvertToDto<Reminder>(r)).ToList();
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage command)
        {
            return await HandleAsync((UpdateAppointmentDetailCommand)command);
        }
    }
}