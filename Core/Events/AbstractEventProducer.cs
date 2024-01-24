using Confluent.Kafka;
using Core.Configs;
using Core.Messages;
using Newtonsoft.Json;

namespace Core.Events
{
    public abstract class AbstractEventProducer
    {
        private readonly KafkaProducerConfig _config;

        protected AbstractEventProducer(KafkaProducerConfig config)
        {
            _config = config;
        }

        public async Task ProduceAsync<T>(T xEvent) where T : BaseEvent
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto };
            using (var producer = new ProducerBuilder<string, string>(_config)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .Build())
                {
                    var message = new Message<string, string>
                    {
                        Key = xEvent.Id.ToString(),
                        Value = JsonConvert.SerializeObject(xEvent, xEvent.GetType(), settings)
                    };

                    var resault = await producer.ProduceAsync(_config.Topic, message);

                    if (resault.Status == PersistenceStatus.NotPersisted)
                    {
                        throw new CoreException("Event producing failed");
                    }
                }

           
        }
    }
}