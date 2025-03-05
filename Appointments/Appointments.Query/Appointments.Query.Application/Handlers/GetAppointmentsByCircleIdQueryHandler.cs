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
    public class GetAppointmentsByCircleIdQueryHandler : IMessageHandler<GetAppointmentsByCircleIdQuery>
    {
        private readonly AppointmentRepository _appointmentRepository;
        private readonly CAMapRepository _mapRepository;
        private readonly AppointmentDetailsRepository _detailsRepository;
        private readonly ILogger<GetAppointmentsByCircleIdQueryHandler> _logger;
        private readonly ReminderRepository _reminderRepository;

        public GetAppointmentsByCircleIdQueryHandler(AppointmentRepository appointmentRepository,
                                                     CAMapRepository mapRepository,
                                                     AppointmentDetailsRepository detailsRepository,
                                                     ILogger<GetAppointmentsByCircleIdQueryHandler> logger,
                                                     ReminderRepository reminderRepository)
        {
            _appointmentRepository = appointmentRepository;
            _mapRepository = mapRepository;
            _detailsRepository = detailsRepository;
            _logger = logger;
            _reminderRepository = reminderRepository;
        }

        public async Task<BaseResponse> HandleAsync(GetAppointmentsByCircleIdQuery query)
        {
            try
            {
                var appointmentIds = await _mapRepository.GetAppointmentsByCircleId(query.CircleId, query.DateFrom, query.DateTo);
                if (appointmentIds.Count > 0)
                {
                    var appointments = await _appointmentRepository.GetAppointments(appointmentIds);
                    SetAppointmentsDetails(ref appointments, query.CircleId);
                    return new BaseResponse { ResponseCode = 200, Data = appointments };
                }

                return new BaseResponse { ResponseCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling GetAppointmentsByCircleIdQuery for CircleId: {CircleId}", query.CircleId);
                return new BaseResponse { ResponseCode = 500, Message = "An error occurred while processing your request." };
            }
        }

        private void SetAppointmentsDetails(ref List<AppointmentModel> appointments, Guid circleId)
        {
            foreach (var a in appointments)
            {
                try
                {
                    if (a.DetailsInCircles != null && a.DetailsInCircles.Contains(circleId))
                    {
                        a.Details = _detailsRepository.FindAsync(a.Id).Result;
                        a.Details.Reminders = ConvertModelsToDtos(_reminderRepository.FindManyAsync(a.Id).Result);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while setting appointment details for AppointmentId: {AppointmentId}", a.Id);
                }
            }
        }

        private static List<Reminder>? ConvertModelsToDtos(List<ReminderModel>? reminders)
        {
            return reminders?.Select(r => ModelToDtoConverter.ConvertToDto<Reminder>(r)).ToList();
        }

        public async Task<BaseResponse> HandleAsync(BaseMessage query)
        {
            return await HandleAsync((GetAppointmentsByCircleIdQuery)query);
        }
    }
}