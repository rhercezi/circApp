
using Core.DTOs;
using Core.Messages;

namespace Core.MessageHandling
{
    public interface IMessageHandler
    {
        Task<BaseResponse> HandleAsync(BaseMessage message);
    }
    public interface IMessageHandler<T> : IMessageHandler where T : BaseMessage
    {
        Task<BaseResponse> HandleAsync(T message);
    }
}