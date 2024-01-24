using Core.DTOs;
using Core.Messages;

namespace Core.MessageHandling
{
    public interface IQueryHandler<T,R> where T : BaseQuery where R : BaseDto
    {
        Task<R> HandleAsync(T query);
    }
}