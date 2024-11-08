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

            if (xEvent == null)
            {
                throw new ArgumentNullException(nameof(xEvent));
            }

            if (_supportedTypes.Count < 3 || _supportedTypes[2] == null)
            {
                throw new InvalidOperationException("The required type is not available in the supported types.");
            }

            return (BaseEvent)System.Convert.ChangeType(xEvent, _supportedTypes[2]);
        }
    }
}