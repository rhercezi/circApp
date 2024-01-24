using Core.Messages;

namespace Core.MessageHandling
{
    public interface ICommandHandler<T> where T : BaseCommand
    {
        Task HandleAsync(T command);
    }
}