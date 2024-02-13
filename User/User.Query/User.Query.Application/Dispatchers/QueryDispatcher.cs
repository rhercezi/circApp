using System.Reflection;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;

namespace User.Query.Application.Dispatchers
{
    public class QueryDispatcher : IQueryDispatcher<UserDto>
    {
        private Dictionary<Type, Type> _handlers = new();
        private readonly ILogger<QueryDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHandlerService _handlerService;

        public QueryDispatcher(ILogger<QueryDispatcher> logger, IServiceProvider serviceProvider, IHandlerService handlerService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _handlerService = handlerService;
        }

        public async Task<UserDto> DispatchAsync(BaseQuery query)
        {
            _handlers = _handlerService.RegisterHandler<BaseQuery,UserDto>(query, Assembly.GetExecutingAssembly(), typeof(UserDto));

            if (_handlers.TryGetValue(query.GetType(), out Type? handlerType))
            {
                var handler = Activator.CreateInstance(handlerType, new object[] {_serviceProvider});
                if (handler is not null)
                {
                    var userDto = (Task<UserDto>)handlerType.GetMethod("HandleAsync").Invoke(handler, new object[] { query });
                    return await userDto;
                }
                return null;
            }
            return null;
        }
    }
}