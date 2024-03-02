using Core.Configs;
using Core.Events;
using Microsoft.Extensions.Options;

namespace Circles.Command.Application.EventProducer
{
    public class JoinCircleEventProducer : AbstractEventProducer
    {
        public JoinCircleEventProducer(IOptions<KafkaProducerConfig> config) : base(config.Value)
        {
        }
    }
}