using Core.MessageHandling;

namespace Core.Events
{
    public interface IEventConsumer
    {
        public Task Consume(IEventDispatcher eventDispatcher, string topic = "");
    }
}