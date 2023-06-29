using System.Reflection;
using Core.Aggregate;
using Core.MessageHandling;
using Core.Messages;
using User.Common.Events;

namespace User.Command.Domain.Aggregates
{
    public class UserAggreate : AbstractAggregate
    {
        public UserAggreate()
        {
        }

        public UserAggreate(UserCreatedEvent xEvent)
        {
            InvokeAction<UserCreatedEvent, UserAggreate>(xEvent, this);
        }
        
        public void InvokAction<T>(BaseEvent xEvent) where T : BaseEvent
        {
            InvokeAction<T,UserAggreate>(xEvent, this);
        }
    }
}