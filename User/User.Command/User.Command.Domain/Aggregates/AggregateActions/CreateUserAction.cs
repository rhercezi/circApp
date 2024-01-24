using Core.MessageHandling;
using User.Common.DAOs;
using User.Common.Events;

namespace User.Command.Domain.Aggregates.AggregateActions
{
    public class CreateUserAction : IAggregateAction<UserCreatedEvent, UserAggregate>
    {
        public async Task ExecuteAsync(UserCreatedEvent xEvent, UserAggregate instance, bool isReplay)
        {
            
            instance._id = xEvent.Id;
            instance._version = xEvent.Version;
            instance._events.Add(xEvent);
            
            if (!isReplay)
            {
                var model = new UserEventModel
                {
                    UserName = xEvent.UserName,
                    Email = xEvent.Email,
                    TimeStamp = DateTime.Now,
                    AggregateId = instance._id,
                    AggregateType = instance.GetType().Name,
                    Version = instance._version,
                    EventType = xEvent.GetType().Name,
                    Event = xEvent
                };

                await instance._eventStore.SaveEventAsync(model);
            }
        }
    }
}