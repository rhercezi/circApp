namespace Core.Messages
{
    public class TypeResolutionService
    {
        Dictionary<string, Dictionary<string, Type>> messageTypeDict = new();
        
        public void SetMapping()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                a => a.GetTypes().Where(
                    t => t.IsClass && t.BaseType == typeof(BaseEvent)
                )
            ).ToList();

            messageTypeDict.Add("Events", new());
            var eventDict = messageTypeDict.GetValueOrDefault("Events");
            types.ForEach(t => eventDict.TryAdd(t.FullName, t));
        }

        public Dictionary<string, Type> GetMappings(string messageType)
        {
            return messageTypeDict.GetValueOrDefault("Events");
        }
    }
}