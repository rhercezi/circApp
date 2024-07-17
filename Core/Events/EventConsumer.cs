using Confluent.Kafka;
using Core.Configs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Core.Events
{
    public class EventConsumer : IEventConsumer
    {
        private readonly KafkaConsumerConfig _config;
        private ILogger<EventConsumer> _logger;

        public EventConsumer(IOptions<KafkaConsumerConfig> config, ILogger<EventConsumer> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task Consume(IMessageDispatcher eventDispatcher, string topic = "")
        {
            using var consumer = new ConsumerBuilder<string, string>(_config)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(Deserializers.Utf8)
                .Build();
            consumer.Subscribe(_config.Topic);

            var consumeResult = consumer.Consume();

            var xEvent = DeserializeMessage(consumeResult.Message.Value);

            if (xEvent != null)
            {
                var result = await eventDispatcher.DispatchAsync(xEvent);
                if (result.ResponseCode < 300)
                {
                    consumer.Commit(consumeResult);
                }
                else
                {
                    _logger.LogError("Dispatching failed for event of type {eventName}", xEvent.GetType().FullName);
                }
            }
            else
            {
                _logger.LogError("Failed to deserialize event:\n {eventBody}", consumeResult.Message.Value);
            }
        }

        private static BaseEvent? DeserializeMessage(string message)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

            if (values == null)
        {
                return null;
            }

            values.TryGetValue("EventType", out var typeName);

            var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                a => a.GetTypes().Where(
                    t => t.IsClass && t.FullName!.Equals(typeName)
                )
            ).First();

            var xEvent = JsonConvert.DeserializeObject(message, type, settings);
            if (xEvent == null)
            {
                return null;
            }

            return (BaseEvent) xEvent;
        }
    }
}
