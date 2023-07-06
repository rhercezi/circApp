using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DAOs;

namespace User.Command.Domain.Repositories
{
    public interface IEventStoreRepository
    {
        Task SaveAsync(EventModel model);
        Task<List<EventModel>> FindByAgregateId(Guid id);
        
    }
}