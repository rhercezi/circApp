using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTOs;

namespace Appointments.Query.Application.DTOs
{
    public class AppointmentsDto : BaseDto
    {
        public List<AppointmentDto>? Appointments { get; set; }
    }
}