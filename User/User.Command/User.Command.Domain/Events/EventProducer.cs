using Core.Configs;
using Core.Events;
using Microsoft.Extensions.Options;

namespace User.Command.Domain.Events
{
    public class EventProducer : AbstractEventProducer
    {
        public EventProducer(IOptions<KafkaProducerConfig> config) : base(config.Value)
        {
        }
    }
}