using Core.Configs;
using Core.Events;
using Microsoft.Extensions.Options;

namespace Appointments.Command.Application.EventProducer
{
    public class AppointmentEventProducer : AbstractEventProducer
    {
        public AppointmentEventProducer(IOptions<KafkaProducerConfig> config) : base(config.Value)
        {
        }
    }
}