using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Appointments.Query.Application.Dispatchers
{
    public class AppointmentsQueryDispatcher : IMessageDispatcher
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

        public async Task<BaseResponse> DispatchAsync(BaseMessage query)
        {
            
                try
                {
                    var genericType = typeof(IMessageHandler<>).MakeGenericType(query.GetType());
                    var handler = (IMessageHandler)_serviceProvider.GetRequiredService(genericType);
                    return await handler.HandleAsync(query);
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                    return new BaseResponse { ResponseCode = 500, Message = "Something went wrong, please contact support via support page." };
                }
        }
    }
}