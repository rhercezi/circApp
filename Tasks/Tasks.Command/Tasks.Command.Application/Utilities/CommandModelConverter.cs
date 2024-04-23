namespace Tasks.Command.Application.Utilities
{
    public class CommandModelConverter
    {
        public static T ConvertToModel<T>(object command)
        {
            var model = Activator.CreateInstance<T>();
            var commandProperties = command.GetType().GetProperties();
            var modelProperties = model?.GetType().GetProperties();

            foreach (var commandProperty in commandProperties)
            {
                var modelProperty = modelProperties?.FirstOrDefault(x => x.Name == commandProperty.Name); 
                modelProperty?.SetValue(model, commandProperty.GetValue(command));
            }

            return model;
        }
    }
}