using System.Reflection;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Circles.Query.Application.Dispatchers
{
    public class UserSearchDispatcher : IQueryDispatcher<AppUsersDto>
    {
        private Dictionary<Type, Type> _handlers = new();
        private readonly ILogger<UserSearchDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHandlerService _handlerService;
        public UserSearchDispatcher(ILogger<UserSearchDispatcher> logger,
                                    IServiceProvider serviceProvider,
                                    IHandlerService handlerService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _handlerService = handlerService;
        }

        public async Task<AppUsersDto> DispatchAsync(BaseQuery query)
        {
            _handlers = _handlerService.RegisterHandler<BaseQuery, AppUsersDto>(query, Assembly.GetExecutingAssembly(), typeof(AppUsersDto));

            if (_handlers.TryGetValue(query.GetType(), out Type? handlerType))
            {
                try
                {
                    var handler = (IQueryHandler)_serviceProvider.GetRequiredService(handlerType);
                    return (AppUsersDto)await handler.HandleAsync(query);
                }
                catch (Exception e)
                {
                    _logger.LogError($"{e.Message}\n{e.StackTrace}");
                    throw;
                }
            }
            _logger.LogError($"Could not find handler for query. Type: {query.GetType().Name}");
            return new AppUsersDto();
        }
    }
}