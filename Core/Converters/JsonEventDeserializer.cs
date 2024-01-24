using Core.Messages;

namespace Core.Converters
{
    public class JsonEventDeserializer
    {


        public static BaseEvent Convert(object? xEvent)
        {
            List<Type> _supportedTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                a => a.GetTypes().Where(
                    t => t.IsClass && t.IsSubclassOf(typeof(BaseEvent))
                )
            ).ToList();

            return (BaseEvent)System.Convert.ChangeType(xEvent, _supportedTypes[2]);
        }
    }
}