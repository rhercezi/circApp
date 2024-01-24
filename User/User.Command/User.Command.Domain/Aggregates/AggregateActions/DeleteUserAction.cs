using Core.MessageHandling;
using User.Common.DAOs;
using User.Common.Events;

namespace User.Command.Domain.Aggregates.AggregateActions
{
    public class DeleteUserAction : IAggregateAction<UserDeletedEvent, UserAggregate>
    {
        public async Task ExecuteAsync(UserDeletedEvent xEvent, UserAggregate instance, bool isReplay)
        {
            if (isReplay)
            {
                instance._id = xEvent.Id;
                instance._version = xEvent.Version;
                instance._events.Add(xEvent);
                instance._isDeleted = true;
            }
             else
            {
                var model = new UserEventModel
                {
                    TimeStamp = DateTime.Now,
                    AggregateId = xEvent.Id,
                    AggregateType = instance.GetType().Name,
                    Version = xEvent.Version,
                    EventType = xEvent.GetType().Name,
                    Event = xEvent
                };

                await instance._eventStore.SaveEventAsync(model);
            }
        }
    }
}