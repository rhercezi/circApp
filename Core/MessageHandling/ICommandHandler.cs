using Core.Messages;

namespace Core.MessageHandling
{
    public interface ICommandHandler
    {
        Task HandleAsync(BaseCommand command);
    }
    public interface ICommandHandler<T> : ICommandHandler where T : BaseCommand
    {
        Task HandleAsync(T command);
    }
}