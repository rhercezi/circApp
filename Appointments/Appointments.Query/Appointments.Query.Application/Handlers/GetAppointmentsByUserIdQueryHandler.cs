using System.Linq;
using Appointments.Domain.Entities;
using Appointments.Domain.Repositories;
using Appointments.Query.Application.Config;
using Appointments.Query.Application.DTOs;
using Appointments.Query.Application.Queries;
using Core.Configs;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Core.Utilities;
using Microsoft.Extensions.Options;

namespace Appointments.Query.Application.Handlers
{
    public class GetAppointmentsByUserIdQueryHandler : IQueryHandler<GetAppointmentsByUserIdQuery, AppointmentsDto>
    {
        private readonly InternalHttpClient<AppUserDto> _internalHttp;
        private readonly IOptions<CirclesServuceConfig> _config;
        private readonly AppointmentRepository _appointmentRepository;
        private readonly CAMapRepository _mapRepository;
        private readonly AppointmentDetailsRepository _detailsRepository;
        public GetAppointmentsByUserIdQueryHandler(
                                                   InternalHttpClient<AppUserDto> internalHttp,
                                                   IOptions<CirclesServuceConfig> config,
                                                   AppointmentRepository appointmentRepository,
                                                   CAMapRepository mapRepository,
                                                   AppointmentDetailsRepository detailsRepository
                                                   )
        {
            _internalHttp = internalHttp;
            _config = config;
            _appointmentRepository = appointmentRepository;
            _mapRepository = mapRepository;
            _detailsRepository = detailsRepository;
        }

        public async Task<AppointmentsDto> HandleAsync(GetAppointmentsByUserIdQuery query)
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
                        SetAppointmnetsDetails(ref appointments, circles);
                        return ToDtoList(appointments);
                    }
                }
            }
            
            return new AppointmentsDto();
        }

        private void SetAppointmnetsDetails(ref List<AppointmentModel> appointments, List<Guid> circles)
        {
            foreach (var a in appointments)
            {
                if (a.DetailsInCircles != null && a.DetailsInCircles.Intersect(circles).Count() > 0)
                {
                    a.Details = _detailsRepository.FindAsync(a.Id).Result;
                }   
            }
        }

        private AppointmentsDto ToDtoList(List<AppointmentModel> appointments)
        {
            AppointmentsDto appointmentsDto = new()
            {
                Appointments = new()
            };

            appointments.ForEach(
                a => {
                    appointmentsDto.Appointments.Add(DtoConverter.Convert(a));
                }
            );

            return appointmentsDto;
        }

        public async Task<BaseDto> HandleAsync(BaseQuery query)
        {
            return await HandleAsync((GetAppointmentsByUserIdQuery)query);
        }
    }
}