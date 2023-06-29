using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.MessageHandling;
using User.Common.Events;

namespace User.Command.Domain.Aggregates.AggregateActions
{
    public class UpdateUserAction : IAggregateAction<UserEditedEvent, UserAggreate>
    {
        public void ExecuteAsync(UserEditedEvent xEvent, UserAggreate instance)
        {
            instance._id = xEvent.Id;
            instance._version = xEvent.Version;
            instance._events.Add(xEvent);
        }
    }
}