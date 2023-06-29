using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.MessageHandling;
using User.Common.Events;

namespace User.Command.Domain.Aggregates.AggregateActions
{
    public class DeleteUserAction : IAggregateAction<UserDeletedEvent, UserAggreate>
    {
        public void ExecuteAsync(UserDeletedEvent xEvent, UserAggreate instance)
        {
            instance._id = xEvent.Id;
            instance._version = xEvent.Version;
            instance._events.Add(xEvent);
        }
    }
}