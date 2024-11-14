using Appointments.Domain.Entities;
using Appointments.Domain.Repositories;
using Appointments.Query.Application.Config;
using Appointments.Query.Application.Queries;
using Appointments.Query.Application.Utilities;
using Core.Configs;
using Core.DAOs;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Core.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appointments.Query.Application.Handlers
{
    public class GetAppointmentsByUserIdQueryHandler : IMessageHandler<GetAppointmentsByUserIdQuery>
    {
        private readonly InternalHttpClient<AppUserDto> _internalHttp;
        private readonly IOptions<CirclesServiceConfig> _config;
        private readonly AppointmentRepository _appointmentRepository;
        private readonly CAMapRepository _mapRepository;
        private readonly AppointmentDetailsRepository _detailsRepository;
        private readonly ReminderRepository _reminderRepository;
        private readonly ILogger<GetAppointmentsByUserIdQueryHandler> _logger;

        public GetAppointmentsByUserIdQueryHandler(
                                                   InternalHttpClient<AppUserDto> internalHttp,
                                                   IOptions<CirclesServiceConfig> config,
                                                   AppointmentRepository appointmentRepository,
                                                   CAMapRepository mapRepository,
                                                   AppointmentDetailsRepository detailsRepository,
                                                   ReminderRepository reminderRepository,
                                                   ILogger<GetAppointmentsByUserIdQueryHandler> logger)
        {
            _internalHttp = internalHttp;
            _config = config;
            _appointmentRepository = appointmentRepository;
            _mapRepository = mapRepository;
            _detailsRepository = detailsRepository;
            _reminderRepository = reminderRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> HandleAsync(GetAppointmentsByUserIdQuery query)
        {
            try
            {
                var clientConfig = new HttpClientConfig
                {
                    BaseUrl = _config.Value.BaseUrl,
                    Path = _config.Value.Path + query.UserId.ToString(),
                    Port = _config.Value.Port
                };

                var appUserDto = await _internalHttp.GetResource(clientConfig);

                if (appUserDto.Circles != null && appUserDto.Circles.Count > 0)
                {
                    var circles = appUserDto.Circles.Select(c => c.Id).ToList();
                    if (circles.Count > 0)
                    {
                        var appointmentIds = await _mapRepository.GetAppointmentsByCircles(circles, query.DateFrom, query.DateTo);
                        if (appointmentIds.Count > 0)
                        {
                            var appointments = await _appointmentRepository.GetAppointments(appointmentIds);
                            SetAppointmentsDetails(ref appointments, circles);
                            return new BaseResponse { ResponseCode = 200, Data = appointments };
                        }
                    }
                }

                return new BaseResponse { ResponseCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling GetAppointmentsByUserIdQuery for UserId: {UserId}", query.UserId);
                return new BaseResponse { ResponseCode = 500, Message = "An error occurred while processing your request." };
            }
        }

        private void SetAppointmentsDetails(ref List<AppointmentModel> appointments, List<Guid> circles)
        {
            foreach (var a in appointments)
            {
                try
                {
                    if (a.DetailsInCircles != null && a.DetailsInCircles.Intersect(circles).Count() > 0)
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
            return await HandleAsync((GetAppointmentsByUserIdQuery)query);
        }
    }
}