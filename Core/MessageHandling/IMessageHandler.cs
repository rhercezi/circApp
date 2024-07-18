
using Core.DTOs;
using Core.Messages;

namespace Core.MessageHandling
{
    public interface IMessageHandler
    {
        public Task<BaseResponse> HandleAsync(BaseMessage message);
    }
    public interface IMessageHandler<T> : IMessageHandler where T : BaseMessage
    {
        public Task<BaseResponse> HandleAsync(T message);
    }
}