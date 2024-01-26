using System.Reflection;
using Confluent.Kafka;
using Core.Configs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using User.Common.Utility;
using User.Query.Domain.Repositories;

namespace User.Query.Application.EventConsuming
{
    public class EventConsumer
    {
        private readonly KafkaConsumerConfig _config;
        private ILogger<EventConsumer> _logger;
        private readonly IHandlerService _handlerService;

        public EventConsumer(IOptions<KafkaConsumerConfig> config,
                             ILogger<EventConsumer> logger,
                             IHandlerService handlerService)
        {
            _config = config.Value;
            _logger = logger;
            _handlerService = handlerService;
        }

        public async Task Consume(UserRepository repository, IServiceProvider serviceProvider)
        {
            using (var consumer = new ConsumerBuilder<string, string>(_config)
                    .SetKeyDeserializer(Deserializers.Utf8)
                    .SetValueDeserializer(Deserializers.Utf8)
                    .Build())
                    {
                        consumer.Subscribe(_config.Topic);

                        while(true)
                        {
                            try
                            {
                                var consumeResult = consumer.Consume();
                                
                                var xEvent = DeserializeMessage(consumeResult.Message.Value);

                                if ( xEvent != null)
                                {
                                    
                                    var resault = await DispatchEvent(xEvent, repository, serviceProvider);
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
                                _logger.LogError("Error consuming event", e);
                            }
                            catch (OperationCanceledException e)
                            {
                                _logger.LogError("Event consuming canceled", e);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError("Error dispatching consumed event", e);
                            }

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

        private async Task<bool> DispatchEvent(BaseEvent xEvent, UserRepository repository, IServiceProvider serviceProvider)
        {
            var handlers = _handlerService.RegisterHandler<BaseEvent>(xEvent, Assembly.GetExecutingAssembly());

            if (handlers.TryGetValue(xEvent.GetType(), out Type? handlerType))
            {
                var handler = Activator.CreateInstance(handlerType, new object[] {serviceProvider, repository});
                if (handler is not null)
                {
                    try
                    {
                        var task = (Task)handlerType.GetMethod("HandleAsync").Invoke(handler, new object[] { xEvent });
                        await task.ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex.StackTrace);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}