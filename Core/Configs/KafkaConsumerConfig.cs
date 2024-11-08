using Confluent.Kafka;

namespace Core.Configs
{
    public class KafkaConsumerConfig : ConsumerConfig
    {
        public required string[] Topics { get; set; }
    }
}