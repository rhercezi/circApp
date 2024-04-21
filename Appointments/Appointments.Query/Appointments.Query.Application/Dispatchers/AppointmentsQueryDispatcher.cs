using System.Reflection;
using Appointments.Query.Application.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Appointments.Query.Application.Dispatchers
{
    public class AppointmentsQueryDispatcher : IQueryDispatcher<AppointmentsDto>
    {
        private Dictionary<Type, Type> _handlers = new();
        private readonly ILogger<AppointmentsQueryDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHandlerService _handlerService;
        public AppointmentsQueryDispatcher(ILogger<AppointmentsQueryDispatcher> logger,
                                           IServiceProvider serviceProvider,
                                           IHandlerService handlerService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _handlerService = handlerService;
        }

        public async Task<AppointmentsDto> DispatchAsync(BaseQuery query)
        {
            _handlers = _handlerService.RegisterHandler<BaseQuery, AppointmentsDto>(query, Assembly.GetExecutingAssembly(), typeof(AppointmentsDto));

            if (_handlers.TryGetValue(query.GetType(), out Type? handlerType))
            {
                try
                {
                    var handler = (IQueryHandler)_serviceProvider.GetRequiredService(handlerType);
                    return (AppointmentsDto)await handler.HandleAsync(query);
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                    throw;
                }
            }
            _logger.LogError($"Could not find handler for query. Type: {query.GetType().Name}");
            return new AppointmentsDto();
        }
    }
}