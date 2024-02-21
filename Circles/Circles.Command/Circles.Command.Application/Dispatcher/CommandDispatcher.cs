using Core.MessageHandling;
using Core.Messages;

namespace Circles.Command.Application.Dispatcher
{
    public class CommandDispatcher : ICommandDispatcher
    {
        public Task<(int code, string message)> DispatchAsync(BaseCommand command)
        {
            throw new NotImplementedException();
        }
    }
}