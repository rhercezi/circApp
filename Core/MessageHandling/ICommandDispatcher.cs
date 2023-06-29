using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Messages;

namespace Core.MessageHandling
{
    public interface ICommandDispatcher
    {
        void RegisterHandler<T>(T command) where T : BaseCommand;
        Task DispatchAsync(BaseCommand command);
    }
}