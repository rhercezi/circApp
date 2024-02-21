using Core.DTOs;
using Core.Messages;

namespace Core.MessageHandling
{
    public interface IQueryHandler
    {
        Task<BaseDto> HandleAsync(BaseQuery query);
    }
    public interface IQueryHandler<T,R> : IQueryHandler where T : BaseQuery where R : BaseDto
    {
        Task<R> HandleAsync(T query);
    }
}