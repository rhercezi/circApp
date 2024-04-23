using Core.Configs;
using Core.Events;
using Microsoft.Extensions.Options;

namespace Tasks.Command.Application.Events
{
    public class TasksEventProducer : AbstractEventProducer
    {
        public TasksEventProducer(IOptions<KafkaProducerConfig> config) : base(config.Value)
        {
        }
    }
}