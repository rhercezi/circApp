using Appointments.Domain.Entities;
using Appointments.Domain.Repositories;
using Appointments.Query.Application.Queries;
using Appointments.Query.Application.Utilities;
using Core.DAOs;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace Appointments.Query.Application.Handlers
{
    public class GetAppointmentByIdQueryHandler : IMessageHandler<GetAppointmentByIdQuery>
    {
        private readonly AppointmentRepository _appointmentRepository;

        private readonly AppointmentDetailsRepository _detailsRepository;

        private readonly ILogger<GetAppointmentByIdQueryHandler> _logger;
        private readonly ReminderRepository _reminderRepository;

        public GetAppointmentByIdQueryHandler(AppointmentRepository appointmentRepository,
                                              AppointmentDetailsRepository detailsRepository,
                                              ILogger<GetAppointmentByIdQueryHandler> logger,
                                              ReminderRepository reminderRepository)
        {
            _appointmentRepository = appointmentRepository;
            _detailsRepository = detailsRepository;
            _logger = logger;
            _reminderRepository = reminderRepository;
        }
        public async Task<BaseResponse> HandleAsync(GetAppointmentByIdQuery message)
        {
            try
            {
                var appointment = await _appointmentRepository.GetAppointmentById(message.Id);
                if (appointment != null)
                {
                    appointment.Details = await _detailsRepository.FindAsync(appointment.Id);
                    appointment.Details.Reminders = ConvertModelsToDtos(await _reminderRepository.FindManyAsync(appointment.Id));
                    return new BaseResponse { ResponseCode = 200, Data = appointment };
                }
                return new BaseResponse { ResponseCode = 404 };
            }
            catch (Exception e)
            {
                _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                return new BaseResponse { ResponseCode = 500, Message = "Something went wrong, please contact support using support page." };
            }
        }

        private static List<Reminder>? ConvertModelsToDtos(List<ReminderModel>? reminders)
        {
            return reminders?.Select(r => ModelToDtoConverter.ConvertToDto<Reminder>(r)).ToList();
        }

        public Task<BaseResponse> HandleAsync(BaseMessage message)
        {
            return HandleAsync((GetAppointmentByIdQuery)message);
        }
    }
}