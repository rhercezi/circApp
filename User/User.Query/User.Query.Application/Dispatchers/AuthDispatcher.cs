using System.Reflection;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.Logging;
using User.Query.Application.DTOs;

namespace User.Query.Application.Dispatchers
{
    public class AuthDispatcher : IQueryDispatcher<TokenDto>
    {
        private Dictionary<Type, Type> _handlers = new();
        private readonly ILogger<AuthDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHandlerService _handlerService;

        public AuthDispatcher(ILogger<AuthDispatcher> logger, IServiceProvider serviceProvider, IHandlerService handlerService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _handlerService = handlerService;
        }

        public async Task<TokenDto> DispatchAsync(BaseQuery query)
        {
            _handlers = _handlerService.RegisterHandler<BaseQuery,TokenDto>(query, Assembly.GetExecutingAssembly(), typeof(TokenDto));

            if (_handlers.TryGetValue(query.GetType(), out Type? handlerType))
            {
                var handler = Activator.CreateInstance(handlerType, new object[] {_serviceProvider});
                if (handler is not null)
                {
                    var tokenDto = (Task<TokenDto>)handlerType.GetMethod("HandleAsync").Invoke(handler, new object[] { query });
                    return await tokenDto;
                }
                return null;
            }
            return null;
        }
    }
}