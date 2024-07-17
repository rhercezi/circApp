using Core.MessageHandling;

namespace Core.Events
{
    public interface IEventConsumer
    {
        public Task Consume(IMessageDispatcher eventDispatcher, string topic = "");
    }
}