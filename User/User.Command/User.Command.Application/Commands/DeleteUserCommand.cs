using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Messages;

namespace User.Command.Api.Commands
{
    public class DeleteUserCommand : BaseCommand
    {
        public DeleteUserCommand(Guid id)
        {
            Id = id;
        }
    }
}