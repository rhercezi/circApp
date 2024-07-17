using Core.DTOs;
using Core.Messages;

namespace Core.MessageHandling
{
    public interface IMessageDispatcher
    {
        Task<BaseResponse> DispatchAsync(BaseMessage message);
    }
}