using Core.DTOs;
using Core.Messages;

namespace Core.MessageHandling
{
    public interface IQueryDispatcher<T> where T : BaseDto
    {
        Task<T> DispatchAsync(BaseQuery query);
    }
}