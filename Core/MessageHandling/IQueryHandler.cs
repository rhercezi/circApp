using Core.DTOs;
using Core.Messages;

namespace Core.MessageHandling
{
    /// <summary>
    /// Query message handler without type specified. Exposes HandleAsync method.
    /// </summary>
    public interface IQueryHandler
    {
        Task<BaseDto> HandleAsync(BaseQuery query);
    }
    /// <summary>
    /// Query message handler interface with specified types
    /// </summary>
    /// <typeparam name="T">Concreat query type. Inherets from BaseQuery</typeparam>
    /// <typeparam name="R">Concreat returned DTO. Inherets from BaseDto</typeparam>
    public interface IQueryHandler<T,R> : IQueryHandler where T : BaseQuery where R : BaseDto
    {
        Task<R> HandleAsync(T query);
    }
}