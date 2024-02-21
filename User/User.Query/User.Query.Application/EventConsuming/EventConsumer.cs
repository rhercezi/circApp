using System.Reflection;
using Confluent.Kafka;
using Core.Configs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using User.Query.Domain.Repositories;

namespace User.Query.Application.EventConsuming
{
    public class EventConsumer
    {
        private readonly KafkaConsumerConfig _config;
        private ILogger<EventConsumer> _logger;

        public EventConsumer(IOptions<KafkaConsumerConfig> config,
                             ILogger<EventConsumer> logger,
                             IEventDispatcher eventDispatcher)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task Consume(IEventDispatcher eventDispatcher)
        {
            using var consumer = new ConsumerBuilder<string, string>(_config)
                    .SetKeyDeserializer(Deserializers.Utf8)
                    .SetValueDeserializer(Deserializers.Utf8)
                    .Build();
            consumer.Subscribe(_config.Topic);

            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume();

                    var xEvent = DeserializeMessage(consumeResult.Message.Value);

                    if (xEvent != null)
                    {

                        var resault = await eventDispatcher.DispatchAsync(xEvent);
                        if (resault)
                        {
                            consumer.Commit(consumeResult);
                        }
                        else
                        {
                            _logger.LogError($"Dispatching failed for event of type {xEvent.GetType().FullName}");
                        }
                    }
                    else
                    {
                        _logger.LogError($"Failed to deserialize event:\n {consumeResult.Message.Value}");
                    }
                }
                catch (ConsumeException e)
                {
                    _logger.LogError("Error consuming event\n" + e.StackTrace);
                }
                catch (OperationCanceledException e)
                {
                    _logger.LogError("Event consuming canceled\n" + e.StackTrace);
                }
                catch (Exception e)
                {
                    _logger.LogError("Error dispatching consumed event\n" + e.StackTrace);
                }

            }
        }

        private BaseEvent DeserializeMessage(string message)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

            values.TryGetValue("EventType", out var typeName);

            var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                a => a.GetTypes().Where(
                    t => t.IsClass && t.FullName.Equals(typeName)
                )
            ).First();

            return (BaseEvent)JsonConvert.DeserializeObject(message, type, settings);
        }
    }
}