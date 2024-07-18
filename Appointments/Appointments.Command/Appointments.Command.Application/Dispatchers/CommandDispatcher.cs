using Appointments.Command.Application.Exceptions;
using Core.DTOs;
using Core.MessageHandling;
using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Appointments.Command.Application.Dispatchers
{
    public class CommandDispatcher : IMessageDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandDispatcher> _logger;

        public CommandDispatcher(IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<BaseResponse> DispatchAsync(BaseMessage command)
        {
           try
                {
                    var genericType = typeof(IMessageHandler<>).MakeGenericType(command.GetType());
                    var handler = (IMessageHandler)_serviceProvider.GetRequiredService(genericType);
                    return await handler.HandleAsync(command);
                }
                catch (AppointmentsApplicationException e)
                {
                    return new BaseResponse { ResponseCode = 400, Message = e.Message };
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred: {Message}\n{StackTrace}", e.Message, e.StackTrace);
                    return new BaseResponse { ResponseCode = 500, Message = "Something went wrong, please contact support via support page." };
                }
        }
    }
}