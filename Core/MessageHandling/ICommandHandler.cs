using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Messages;

namespace Core.MessageHandling
{
    public interface ICommandHandler<T> where T : BaseCommand
    {
        Task HandleAsync(T command);
    }
}