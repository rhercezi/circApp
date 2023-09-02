using Core.DAOs;
using Core.MessageHandling;
using User.Common.Events;

namespace User.Command.Domain.Aggregates.AggregateActions
{
    public class UpdateUserAction : IAggregateAction<UserEditedEvent, UserAggregate>
    {
        public async void ExecuteAsync(UserEditedEvent xEvent, UserAggregate instance, bool isReplay)
        {

            if (isReplay)
            {
                instance._id = xEvent.Id;
                instance._version = xEvent.Version;
                instance._events.Add(xEvent);
            }
             else
            {
                var model = new EventModel
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