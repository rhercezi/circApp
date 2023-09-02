using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Core.MessageHandling;
using Core.Messages;

namespace Core.Aggregate
{
    public abstract class AbstractAggregate
    {
        public Guid _id { get; set; }
        public int _version { get; set; }
        public bool _isDeleted { get; set; }
        public List<BaseEvent> _events = new();

        public void ClearEvents()
        {
            _events.Clear();
        }

        protected void InvokeAction<T,G>(BaseEvent xEvent, AbstractAggregate instance, bool isReplay) where T : BaseEvent  where G : AbstractAggregate
        {
            Type genericType = typeof(IAggregateAction<,>).MakeGenericType(xEvent.GetType(), instance.GetType());

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                a => a.GetTypes().Where(
                    t => t.IsClass && t.GetInterfaces().Contains(genericType)
                )
            ).ToList();

            if (types.Any() && types.First() != null)
            {
                var action = Activator.CreateInstance(types.First());
                types.First().GetMethod("ExecuteAsync").Invoke(action, new object[] { xEvent, instance, isReplay });
            }
            else
            {
                throw new TargetException($"Faild to invoke aggregate action for event of type: {xEvent.GetType().FullName}");
            }
        }
        
        protected void ReplayEvents(List<BaseEvent> events, AbstractAggregate instance, bool IsReplay)
        {
            foreach (var xEvent in events)
            {
                InvokeAction<BaseEvent,AbstractAggregate>(xEvent, instance, IsReplay);
            }
        }
    }
}