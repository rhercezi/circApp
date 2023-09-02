using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Messages
{
    public abstract record BaseEvent(Guid Id, int Version);
}