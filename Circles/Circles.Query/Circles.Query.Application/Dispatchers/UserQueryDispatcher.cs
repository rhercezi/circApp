using System.Reflection;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Circles.Query.Application.Dispatchers
{
    public class UserQueryDispatcher : IQueryDispatcher<AppUserDto>
    {
        private Dictionary<Type, Type> _handlers = new();
        private readonly ILogger<UserQueryDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHandlerService _handlerService;

        public UserQueryDispatcher(ILogger<UserQueryDispatcher> logger,
                                   IServiceProvider serviceProvider,
                                   IHandlerService handlerService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _handlerService = handlerService;
        }

        public async Task<AppUserDto> DispatchAsync(BaseQuery query)
        {
            _handlers = _handlerService.RegisterHandler<BaseQuery, AppUserDto>(query, Assembly.GetExecutingAssembly(), typeof(AppUserDto));

            if (_handlers.TryGetValue(query.GetType(), out Type? handlerType))
            {
                try
                {
                    var handler = (IQueryHandler)_serviceProvider.GetRequiredService(handlerType);
                    return (AppUserDto)await handler.HandleAsync(query);
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                    throw;
                }
            }
            _logger.LogError($"Could not find handler for query. Type: {query.GetType().Name}");
            return new AppUserDto();
        }
    }
}