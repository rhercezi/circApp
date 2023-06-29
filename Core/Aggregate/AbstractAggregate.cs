using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.MessageHandling;
using Core.Messages;

namespace Core.Aggregate
{
    public abstract class AbstractAggregate
    {
        public Guid _id { get; set; }
        public int _version { get; set; }
        public List<BaseEvent> _events = new();
        public void ClearEvents()
        {
            _events.Clear();
        }
        protected void InvokeAction<T, G>(BaseEvent xEvent, AbstractAggregate instance) where T : BaseEvent  where G : AbstractAggregate
        {
            Type genericType = typeof(IAggregateAction<T,G>).MakeGenericType(xEvent.GetType(), instance.GetType());

            var types = Assembly.GetExecutingAssembly().GetTypes().Where(
                t => t.IsClass && t.GetInterfaces().Contains(genericType)
            ).ToList();

            if (types.Any() && types.First() != null)
            {
                var action = Activator.CreateInstance(types.First()) as IAggregateAction<BaseEvent, AbstractAggregate>;
                action?.ExecuteAsync(xEvent, this);
            }
            else
            {
                throw new TargetException($"Faild to invoke aggregate action for event of type: {xEvent.GetType().FullName}");
            }
        }
    }
}