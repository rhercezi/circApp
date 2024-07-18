using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tasks.Query.Application.Dispatchers
{
    public class TasksQueryDispatcher : IMessageDispatcher
    {
        private readonly ILogger<TasksQueryDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        public TasksQueryDispatcher(ILogger<TasksQueryDispatcher> logger,
                               IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<BaseResponse> DispatchAsync(BaseMessage query)
        {
                try
                {
                    var handlerType = typeof(IMessageHandler<>).MakeGenericType(query.GetType());
                    var handler = (IMessageHandler)_serviceProvider.GetRequiredService(handlerType);
                    return await handler.HandleAsync(query);
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                    return new BaseResponse{ ResponseCode = 500, Data = "Something went wrong, please try again later." };
                }
        }
    }
}