using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Appointments.Query.Application.Dispatchers
{
    public class AppointmentsQueryDispatcher : IMessageDispatcher
    {
        private readonly ILogger<AppointmentsQueryDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        public AppointmentsQueryDispatcher(ILogger<AppointmentsQueryDispatcher> logger,
                                           IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
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