using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Messages;

namespace Core.Events.PublicEvents
{
    public class AppointmentChangePublicEvent : BaseEvent
    {
        public Guid AppointmentId { get; set; }
        public Guid UserId { get => Id; set => Id = value; }
        public DateTime Date { get; set; }
        public List<Guid> Circles { get; set; }
        public AppointmentChangePublicEvent(
                                            Guid appointmentId,
                                            Guid userId,
                                            DateTime date,
                                            List<Guid> circles
                                            ) : base(userId, 0, typeof(AppointmentChangePublicEvent).FullName)
        {
            AppointmentId = appointmentId;
            UserId = userId;
            Date = date;
            Circles = circles;
        }
    }
}