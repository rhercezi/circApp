using Appointments.Domain.Entities;
using Appointments.Domain.Repositories;
using Appointments.Query.Application.DTOs;
using Appointments.Query.Application.Queries;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;

namespace Appointments.Query.Application.Handlers
{
    public class GetAppointmentsByCircleIdQueryHandler : IQueryHandler<GetAppointmentsByCircleIdQuery, AppointmentsDto>
    {
        private readonly AppointmentRepository _appointmentRepository;
        private readonly CAMapRepository _mapRepository;
        private readonly AppointmentDetailsRepository _detailsRepository;
        public GetAppointmentsByCircleIdQueryHandler(AppointmentRepository appointmentRepository,
                                                     CAMapRepository mapRepository,
                                                     AppointmentDetailsRepository detailsRepository)
        {
            _appointmentRepository = appointmentRepository;
            _mapRepository = mapRepository;
            _detailsRepository = detailsRepository;
        }

        public async Task<AppointmentsDto> HandleAsync(GetAppointmentsByCircleIdQuery query)
        {
            var appointmentIds = await _mapRepository.GetAppointmentsByCircleId(query.CircleId, query.DareFrom, query.DateTo);
            if (appointmentIds.Count > 0)
            {
                var appointments = await _appointmentRepository.GetAppointments(appointmentIds);
                SetAppointmnetsDetails(ref appointments, query.CircleId);
                return ToDtoList(appointments);
            }
            
            return new AppointmentsDto();
        }

        private void SetAppointmnetsDetails(ref List<AppointmentModel> appointments, Guid circleId)
        {
            foreach (var a in appointments)
            {
                if (a.DetailsInCircles != null && a.DetailsInCircles.Contains(circleId))
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
                a => appointmentsDto.Appointments.Add(DtoConverter.Convert(a))
            );

            return appointmentsDto;
        }

        public async Task<BaseDto> HandleAsync(BaseQuery query)
        {
            return await HandleAsync((GetAppointmentsByCircleIdQuery)query);
        }
    }
}