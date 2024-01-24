using Confluent.Kafka;

namespace Core.Configs
{
    public class KafkaProducerConfig : ProducerConfig
    {
        public required string Topic { get; set; }
    }
}