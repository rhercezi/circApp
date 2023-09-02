using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Messages;

namespace User.Common.Events
{
    public record UserDeletedEvent(Guid Id, int Version) : BaseEvent(Id, Version);
}