using Appointments.Command.Application.Commands;
using Appointments.Command.Application.DTOs;
using Appointments.Command.Application.EventProducer;
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
    public class AddAppointmentDetailsCommandHandler : IMessageHandler<AddAppointmentDetailsCommand>
    {
        private readonly AppointmentDetailsRepository _detailsRepository;
        private readonly AppointmentRepository _appointmentRepository;
        private readonly ILogger<AddAppointmentDetailsCommandHandler> _logger;
        private readonly AppointmentEventProducer _eventProducer;
        private readonly ReminderRepository _reminderRepository;
        public AddAppointmentDetailsCommandHandler(AppointmentDetailsRepository detailsRepository,
                                                   AppointmentRepository appointmentRepository,
                                                   ILogger<AddAppointmentDetailsCommandHandler> logger,
                                                   AppointmentEventProducer eventProducer,
                                                   ReminderRepository reminderRepository)
        {
            _detailsRepository = detailsRepository;
            _appointmentRepository = appointmentRepository;
            _logger = logger;
            _eventProducer = eventProducer;
            _reminderRepository = reminderRepository;
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage message)
        {
            return await HandleAsync((AddAppointmentDetailsCommand)message);
        }

        public async Task<BaseResponse> HandleAsync(AddAppointmentDetailsCommand message)
        {
            if (message.Details != null)
            {
                try
                {
                    var appointment = await _appointmentRepository.GetAppointmentById(message.Details.AppointmentId);

                    if (appointment.CreatorId != message.UserId)
                    {
                        _logger.LogCritical($"User mismatch user with the Id: {message.UserId} tried adding appointment details {message.Details}");
                        return new BaseResponse { ResponseCode = 400, Message = "Only the appointment creator can add the appointment details" };
                    }

                    await _detailsRepository.SaveAsync(DtoConverter.Convert(message.Details));

                    await _eventProducer.ProduceAsync(
                        new AppointmentChangePublicEvent(
                            appointment.Id,
                            appointment.Title,
                            EventType.Update,
                            appointment.CreatorId,
                            appointment.StartDate,
                            appointment.DetailsInCircles ?? new List<Guid>()
                        )
                    );

                    appointment.Details = new AppointmentDetailsModel{
                        AppointmentId = message.Details.AppointmentId,
                        Note = message.Details.Note,
                        Address = message.Details.Address
                    };

                    appointment.Details.Reminders?.ForEach(
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
                                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                            }
                        }
                    );
                    
                    return new BaseResponse { ResponseCode = 200, Data = appointment };
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}\n{details}", e.Message, e.StackTrace, message.Details);
                    return new BaseResponse { ResponseCode = 500, Message = "Something went wrong, please try again later." };
                }
            }
            _logger.LogCritical("Can not add null value to appointment details.\n{details}", message.Details);
            return new BaseResponse { ResponseCode = 422, Message = "Details not valid." };
        }
    }
}