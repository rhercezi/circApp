using Core.Messages;
using Microsoft.AspNetCore.JsonPatch;

namespace User.Command.Application.Commands
{
    public class EditUserCommand : BaseCommand
    {
        public EditUserCommand()
        {
            Updated = DateTime.Now;
        }

        public DateTime Updated { get; set; }
        public required JsonPatchDocument UpdateJson { get; set; }
    }
}