using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Aggregate;
using Core.Messages;

namespace Core.MessageHandling
{
    public interface IAggregateAction<T,G> where T : BaseEvent where G : AbstractAggregate
    {
        void ExecuteAsync(T xEvent, G instance);
    }
}