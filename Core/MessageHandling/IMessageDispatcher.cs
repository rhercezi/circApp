using Core.DTOs;
using Core.Messages;

namespace Core.MessageHandling
{
    public interface IMessageDispatcher
    {
        public Task<BaseResponse> DispatchAsync(BaseMessage message);
    }
}