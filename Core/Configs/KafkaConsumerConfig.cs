using Confluent.Kafka;

namespace Core.Configs
{
    public class KafkaConsumerConfig : ConsumerConfig
    {
        public required string Topic { get; set; }
    }
}