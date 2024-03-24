using System.Linq;
using Appointments.Domain.Entities;
using Appointments.Domain.Repositories;
using Appointments.Query.Application.DTOs;
using Appointments.Query.Application.Queries;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;

namespace Appointments.Query.Application.Handlers
{
    public class GetAppointmentsByUserIdQueryHandler : IQueryHandler<GetAppointmentsByUserIdQuery, AppointmentsDto>
    {
        private readonly UserCircleRepository _UCRepository;
        private readonly AppointmentRepository _appointmentRepository;
        private readonly CAMapRepository _mapRepository;
        private readonly AppointmentDetailsRepository _detailsRepository;
        public GetAppointmentsByUserIdQueryHandler(UserCircleRepository uCRepository,
                                                   AppointmentRepository appointmentRepository,
                                                   CAMapRepository mapRepository,
                                                   AppointmentDetailsRepository detailsRepository)
        {
            _UCRepository = uCRepository;
            _appointmentRepository = appointmentRepository;
            _mapRepository = mapRepository;
            _detailsRepository = detailsRepository;
        }

        public async Task<AppointmentsDto> HandleAsync(GetAppointmentsByUserIdQuery query)
        {
            var userCirclePairs = await _UCRepository.FindAsync(uc => uc.UserId == query.UserId);
            if (userCirclePairs.Count > 0)
            { 
                var circles = userCirclePairs.Select(uc => uc.CircleId).ToList();
                if (circles.Count > 0)
                {
                    var appointmentIds = await _mapRepository.GetAppointmentsByCircles(circles);
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