using Appointments.Command.Application.Commands;
using Appointments.Command.Application.DTOs;
using Appointments.Command.Application.EventProducer;
using Appointments.Domain.Entities;
using Appointments.Domain.Repositories;
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
                        _logger.LogCritical($"User missmatch user with the Id: {message.UserId} tried adding appointment details {message.Details}");
                        return new BaseResponse { ResponseCode = 400, Message = "Only the appointment creator can add the appointment details" };
                    }

                    await _detailsRepository.SaveAsync(DtoConverter.Convert(message.Details));

                    await _eventProducer.ProduceAsync(
                        new AppointmentChangePublicEvent(
                            appointment.Id,
                            appointment.CreatorId,
                            appointment.Date,
                            appointment.DetailsInCircles
                        )
                    );

                    appointment.Details = new AppointmentDetailsModel{
                        AppointmentId = message.Details.AppointmentId,
                        Note = message.Details.Note,
                        Address = message.Details.Address,
                        Reminders = message.Details.Reminders
                    };
                    
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