using System.Reflection;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tasks.Query.Application.Dispatchers
{
    public class TasksQueryDispatcher : IQueryDispatcher<TasksDto>
    {
        private Dictionary<Type, Type> _handlers = new();
        private readonly ILogger<TasksQueryDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHandlerService _handlerService;
        public TasksQueryDispatcher(ILogger<TasksQueryDispatcher> logger,
                               IServiceProvider serviceProvider,
                               IHandlerService handlerService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _handlerService = handlerService;
        }

        public async Task<TasksDto> DispatchAsync(BaseQuery query)
        {
            _handlers = _handlerService.RegisterHandler<BaseQuery, TasksDto>(query, Assembly.GetExecutingAssembly(), typeof(TasksDto));

            if (_handlers.TryGetValue(query.GetType(), out Type? handlerType))
            {
                try
                {
                    var handler = (IQueryHandler)_serviceProvider.GetRequiredService(handlerType);
                    return (TasksDto)await handler.HandleAsync(query);
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                    throw;
                }
            }
            _logger.LogError($"Could not find handler for query. Type: {query.GetType().Name}");
            return new TasksDto();
        }
    }
}