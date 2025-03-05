using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appointments.Command.Application.Utilities
{
    public class ModelToDtoConverter
    {
        public static T ConvertToDto<T>(object model)
        {
            var dto = Activator.CreateInstance<T>();
            var modelProperties = model.GetType().GetProperties();
            var dtoProperties = dto?.GetType().GetProperties();

            foreach (var dtoProperty in dtoProperties)
            {
                var modelProperty = modelProperties?.FirstOrDefault(x => x.Name == dtoProperty.Name);
                dtoProperty?.SetValue(dto, modelProperty?.GetValue(model));
            }

            return dto;
        }
    }
}